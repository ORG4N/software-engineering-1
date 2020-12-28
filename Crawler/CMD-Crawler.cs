using System;
using System.IO;
using System.Collections.Generic;

namespace Crawler
{
    /**
     * The main class of the Dungeon Crawler Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already exist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */
    public class CMDCrawler
    {
        //use the following to store and control the next movement of the user
        public enum PlayerActions {NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, QUIT };
        private PlayerActions action = PlayerActions.NOTHING;

        private bool active = true;                         // Tracks if the game is running

        private FileStream stream = null;                   // Persistent filestream
        private List<string> rows = new List<string>();     // List of all lines read from file
        private char[][] map = new char[0][];               // Persistent map variable
        private char[][] mapCopy;                           // This is a copy of the original map - changes will be applied to this version
        private int height = 0;                             // Vertical length of map
        private int width = 0;                              // Horizontal length of the map

        private bool gameOver = false;                      // Track if game has ended
        private bool mapLoaded = false;                     // Has user selected map?
        private bool mapPlaying = false;                    // Has user input 'play'?
        private bool currentMapLoaded = false;              // Track if the map has been loaded

        private int gold = 0;                               // Currency - applies a crit effect to player damage

        private int[] position = { 0, 0 };                  // Store player position
        private int[] positionCopy = { 0, 0 };              // Temp player position - used to check for collisions
        private bool setPosition = false;                   // Track if the starting position has been set
        private char currentChar = '.';                     // Store the char that the player symbol has replaced
        private char charAtPos;                             // Store the char that the player wants to move to

        private int mHealth;                                // Monster health  
        private int mDamage;                                // Monster attack damage
        private int pHealth = 20;                           // Player health
        private int pDamage = 10;                           // player attack damage

        private Random randNum = new Random();              // using the random class I can generate random numbers

        private List<int> monsterPositions = new List<int>();   // Stores x and y positions of every monster
        private int monsterMoveCount = 0;                   // Limit the monsters ability to freely move

        /**
         * Reads user input from the Console
         * Please use and implement this method to read the user input.
         * Return the input as string to be further processed
         */
        private string ReadUserInput()
        {
            string inputRead = string.Empty;

            if (mapPlaying == true)
            {
                char key = Console.ReadKey().KeyChar;
                inputRead = key.ToString();
            }

            else
            {
                inputRead = Console.ReadLine();                 // Player must type an input and press 'Enter' to submit action.
            }
   
            return inputRead;
        }

        /**
         * Processed the user input string
         * takes apart the user input and does control the information flow
         *  * initializes the map ( you must call InitializeMap)
         *  * starts the game when user types in Play
         *  * sets the correct playeraction which you will use in the GameLoop
         */
        public void ProcessUserInput(string input)
        {
            input = input.ToLower();                        // Inputs will therefore not be case sensitive - which can be annoying!
            bool quitting = false;

            // User can quit or close the game via this input
            if (input == "quit" || input == "q")
            {
                action = PlayerActions.QUIT;
                quitting = true;
            }

            if (input == "help" || input == "h")
            {
                string[] allMaps = Directory.GetFiles(@"..\..\..\maps\");       // Get all maps within this specific directory


                Console.WriteLine("╔═════════════════════════════════════╗");
                Console.WriteLine("║           GETTING STARTED           ║");
                Console.WriteLine("╚═════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("        Welcome to CMD Dungeon!");
                Console.WriteLine();
                Console.WriteLine(" ├───────────────────────────────────┤");
                Console.WriteLine();
                Console.WriteLine("   1. Load map using 'load <mapname>'");
                Console.WriteLine("   2. Start game using 'play'");
                Console.WriteLine();
                Console.WriteLine(" ├───────────────────────────────────┤");
                Console.WriteLine();
                Console.WriteLine("            Available maps:");
                Console.WriteLine();

                // This loop will print all maps found within the maps folder
                for (int i=0; i<allMaps.Length; i++)
                {
                    allMaps[i] = (allMaps[i]).Remove(0, 14);                    // Extract only the filename
                    Console.WriteLine("   ~. {0}",allMaps[i]);
                }

                Console.WriteLine();
                Console.WriteLine(" ├───────────────────────────────────┤");
                Console.WriteLine();
                Console.WriteLine("   #. Move using             W A S D");
                Console.WriteLine("   #. Pick up Gold using     E");
                Console.WriteLine("   #. Quit game using        Q");
                Console.WriteLine();
                Console.WriteLine(" ├───────────────────────────────────┤");
                Console.WriteLine("\n\n");
            }

            // If a map isnt already loaded then when the user inputs a load command, initialize the corresponding map file.
            else if (!mapLoaded)
            {
                // First choice of map options
                if (input == "load simple.map")
                {
                    string fileName = "Simple.map";
                    Console.WriteLine("Selected map file is: {0}", fileName);
                    InitializeMap(fileName);
                    mapLoaded = true;
                }

                // Second choice of map options *ADVANCED*
                else if (input == "load advanced.map")
                {
                    string fileName = "test.map";
                    Console.WriteLine("Selected map file is: {0}", fileName);
                    InitializeMap(fileName);
                    mapLoaded = true;
                }

                // If the user input is incorrect
                else 
                {
                    Console.WriteLine("-- INVALID INPUT --");
                    Console.WriteLine(" 1. Load a map");
                    Console.WriteLine(" 2. Input 'play'\n\n");
                }
            }

            // If the user inputs "play" then the map will be drawn else output error message.
            else if (mapLoaded && !mapPlaying)
            {
                if (input == "play")
                {
                    Console.WriteLine("Loading map...\n\n");
                    GetOriginalMap();
                    SetMonsterDamage();     // initialize monster damage
                    SetMonsterHealth();     // initialize monster health
                    mapPlaying = true;
                }

                // If the user input is incorrect
                else if (!quitting)
                {
                    Console.WriteLine("-- INVALID INPUT --");
                    Console.WriteLine(" #. Input 'play' to continue\n\n");
                }
            }

            if (mapPlaying)
            {
                if (input == "w") {action = PlayerActions.NORTH;}       // Player moves vertically up

                if (input == "a") {action = PlayerActions.WEST;}        // Player moves horizontally left

                if (input == "s") {action = PlayerActions.SOUTH;}       // Player moves vertically down

                if (input == "d") {action = PlayerActions.EAST;}        // Player moves horizontally right

                if (input == "q") { action = PlayerActions.QUIT; }      // Player quits game

                if (input == " ") { action = PlayerActions.ATTACK; }      // Player quits game



                // Player can collect gold if they are on the correct tile and input 'E'
                if (charAtPos == 'G')                                   
                {
                    if (input == "e")
                    {
                        action = PlayerActions.PICKUP;
                    }
                }

                if (input == "play") {action = PlayerActions.NOTHING;}  // This input should not work if the player is playing
            }
        }

        /**
         * The Main Game Loop. 
         * It updates the game state.
         * This is the method where you implement your game logic and alter the state of the map/game
         * use playeraction to determine how the character should move/act
         * the input should tell the loop if the game is active and the state should advance
         */
        public void GameLoop(bool active)
        {
            // If player starting position hasn't been set then set it
            if (setPosition == false)
            {
                GetPlayerPosition();
            }

            position.CopyTo(positionCopy, 0);               // Copy the current position to a temporary var


            int x = position[0];
            int y = position[1];


            if (GetPlayerAction() == 1)                     // Input = 'W', move North
            {
                positionCopy[1] = y - 1;
                MakeMove();
                MoveMonsters();
            }

            if (GetPlayerAction() == 2)                     // Input = 'A', move West
            {
                positionCopy[0] = x + 1;
                MakeMove();
                MoveMonsters();
            }

            if (GetPlayerAction() == 3)                     // Input = 'S', move South
            {
                positionCopy[1] = y + 1;
                MakeMove();
                MoveMonsters();
            }

            if (GetPlayerAction() == 4)                     // Input = 'D', move East
            {
                positionCopy[0] = x - 1;
                MakeMove();
                MoveMonsters();
            }

            if (GetPlayerAction() == 5)                     // Input = 'E', pick-up gold
            {
                gold += 1;
                currentChar = '.';
                pDamage = PlayerDamage();                   // Damage is determined by gold
            }

            if (GetPlayerAction() == 6)                     // Input = 'Spacebar', attack monster
            {
                if (charAtPos == 'M')
                {
                    Console.WriteLine("You swing your sword and have struck MONSTER FLESH!\n");
                    Combat();
                }

                else {Console.WriteLine("You swing your sword and have struck NOTHING...\n"); }
            }

            if (GetPlayerAction() == 7)                     // Input = quit, end game
            {
                this.active = false;
            }

            action = PlayerActions.NOTHING;                 // Reset 'action' after making an action.
        }

        /*
         * This method enables monsters to be able to move - it is called whenever the player moves
         */
        public void MoveMonsters()          
        {
            if (monsterMoveCount < 5)           // Monster can make 5 moves before being forced to stay still for 10 player movements        
            {
                GetMonsterPositions();          // Refresh current monsters on map

                for (int i = 0; i < monsterPositions.Count; i += 2)     // for each of the monsters found
                {
                    int direction = randNum.Next(0, 3);                 // Get the monster direction to move randomly
                    int x, y;

                    x = monsterPositions[i];
                    y = monsterPositions[i + 1];

                    int[] monsterPositionsCopy = { 0, 0 };              // This copy array is used to make a 'theoretical' movement
                    monsterPositionsCopy[0] = monsterPositions[i];
                    monsterPositionsCopy[1] = monsterPositions[i + 1];

                    if (direction == 0) { monsterPositionsCopy[1] = y + 1; }    // North
                    if (direction == 1) { monsterPositionsCopy[0] = x + 1; }        // East
                    if (direction == 2) { monsterPositionsCopy[1] = y - 1; }    // South
                    if (direction == 3) { monsterPositionsCopy[0] = x - 1; }        // West

                    // Assess whether the monster can move onto a specific tile
                    if (CanMove(monsterPositionsCopy[0], monsterPositionsCopy[1], "monster") == true)
                    {
                        int j, k;

                        // Replace the monster char with the tile.
                        j = monsterPositions[i];
                        k = monsterPositions[i + 1];
                        mapCopy[k][j] = '.';

                        j = monsterPositionsCopy[0];
                        k = monsterPositionsCopy[1];

                        // Move player char to next tile
                        mapCopy[k][j] = 'M';
                        monsterPositions[i] = monsterPositionsCopy[0];
                        monsterPositions[i + 1] = monsterPositionsCopy[1];
                    }
                }
            }

            // Means that between 5 and 15 the monsters are frozen in place
            else if (monsterMoveCount == 15)
            {
                monsterMoveCount = 0;
            }

            monsterMoveCount++;
        }

        /*
        * Attack sequence - inspired by turn based games, except it is very simple
        * Mainly a lot of writing to the console
        */
        public void Combat()
        {
            int surpriseAttack = randNum.Next(1, 100);      // Initiative - who attacks first 

            if (surpriseAttack <= 50)
            {
                Console.WriteLine("Your slash catches the monster off-guard!");


                while (mHealth > 0 && pHealth > 0)
                {
                    Console.WriteLine("Player  health: {0}", pHealth);
                    Console.WriteLine("Monster health: {0}\n", mHealth);

                    PlayerAttack();       // player attacks first
                    MonsterAttack();      // monster attacks second

                    Console.WriteLine();
                }
            }

            else
            {
                Console.WriteLine("The monster evades your attack!");

                while (mHealth > 0 && pHealth > 0)
                {
                    Console.WriteLine("Player  health: {0}", pHealth);
                    Console.WriteLine("Monster health: {0}\n", mHealth);

                    MonsterAttack();       // monster attacks first
                    PlayerAttack();      // player attacks second

                    Console.WriteLine();
                }
            }

            // player is dead
            if (pHealth <= 0) 
            {
                Console.WriteLine("\nYou have died and your journey has come to an end...\n");
                GameOver();
            }

            // monster is dead
            else if (mHealth <= 0) 
            { 
                Console.WriteLine("\nYou have slain the monster!\n");
                GetMonsterPositions();

                for(int i=0; i<monsterPositions.Count; i+=2)
                {
                    int x = monsterPositions[i];
                    int y = monsterPositions[i+1];

                    // checking if the monster is next to the player so that the right monster is deleted from map
                    if (mapCopy[y - 1][x] == '@') { mapCopy[y][x] = '.'; }
                    if (mapCopy[y + 1][x] == '@') { mapCopy[y][x] = '.'; }
                    if (mapCopy[y][x - 1] == '@') { mapCopy[y][x] = '.'; }
                    if (mapCopy[y][x + 1] == '@') { mapCopy[y][x] = '.'; }
                }

                SetMonsterDamage();
                SetMonsterHealth();
                DrawMap(mapCopy);
            }
        }

        /*
        * Get positions of all monsters - will work for maps with more than one monster
        */
        public void GetMonsterPositions()
        {
            monsterPositions.Clear();
            // Add all monster coordinates to a single list, can be found by incrementing a loop index by 2 at a time
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mapCopy[y][x] == 'M')               // Monster is recognised by the M symbol
                    {
                        monsterPositions.Add(x);
                        monsterPositions.Add(y);
                    }
                }
            }
        }

        public void PlayerAttack()
        {
            mHealth = mHealth - pDamage;
            Console.WriteLine("Damage dealt: {0}", pDamage);
        }

        public void MonsterAttack()
        {
            pHealth = pHealth - mDamage;
            Console.WriteLine("Damage recieved: {0}", mDamage);
        }

        /*
         * Looks at the char the player wants to move to and assesses it's validity
         */
        public bool CanMove(int x, int y, string entity)
        {
            charAtPos = mapCopy[y][x];
            bool canMove = false;

            if (charAtPos == '#') {canMove = false;}        // Player and Monster cannot move onto tiles that are Walls

            if (charAtPos == '@') { canMove = false; }      // Player and Monster cannot move onto tiles that are Players 

            if (charAtPos == 'M') {canMove = false; }       // Player and Monster cannot move onto tiles that are Monsters

            if (charAtPos == 'G' && entity != "monster") {canMove = true;}         // Player can move onto Gold

            if (charAtPos == '.') {canMove = true; }        // Player and Monster can freely move through empty spaces

            if (charAtPos == 'E' && entity == "player") {canMove = true; }        // Player can move onto ending tile which will finish the level.

            return canMove;
        }

        /*
         * Responds in context to CanMove - player will eiter move or recieve feedback
         */ 
        public void MakeMove()
        {
            if (CanMove(positionCopy[0], positionCopy[1], "player") == false)
            {
                Console.WriteLine("OUCH! You seem to have collided with something in the darkness...");

                if (charAtPos == 'M')
                {
                    Console.WriteLine("OH NO! ITS A MONSTER! PRESS SPACE TO ATTACK\n");
                }
            }

            else 
            {
                int x, y;

                // Replace the player char with the tile.
                x = position[0];
                y = position[1];
                mapCopy[y][x] = currentChar;

                x = positionCopy[0];
                y = positionCopy[1];

                // Store char at new pos before making a move
                currentChar = mapCopy[y][x];

                // Move player char to next tile
                mapCopy[y][x] = '@';
                positionCopy.CopyTo(position, 0);

                GetCurrentMapState();                       // Draw the map

                if (currentChar == 'E')                     // Game ends if player enters the 'E' tile
                {
                    GameOver();
                }
            }
        }

        /**
        * Output text to screen when the player has beaten or lost the game
        * Switch state of variables to change gamestate
        */ 
        public void GameOver()
        {
            if (pHealth <=0)
            {
                Console.WriteLine(" ╔═════════════════╗ ");
                Console.WriteLine(" ║                 ║ ");
                Console.WriteLine(" ║  YOU HAVE DIED  ║ ");
                Console.WriteLine(" ║                 ║ ");
                Console.WriteLine(" ╚═════════════════╝ ");
            }

            else
            {
                Console.WriteLine(" ╔═════════════════╗ ");
                Console.WriteLine(" ║ CONGRATULATIONS ║ ");
                Console.WriteLine(" ║ YOU HAVE BEATEN ║ ");
                Console.WriteLine(" ║    THE GAME!    ║ ");
                Console.WriteLine(" ╚═════════════════╝ ");
            }
            Console.WriteLine("\n\n");

            Console.WriteLine("\n\n");
            gameOver = true;
            active = false;
        }

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * Create a private object variable for storing the map in Crawler and using it in the game.
        */
        public bool InitializeMap(String mapName)
        {
            bool initSuccess = false;

            mapName = @"..\..\..\maps\" + mapName;          // maps will always be within the maps folder
            string path = Path.GetFullPath(mapName);

            if (stream == null)
            {
                // Create a new FileStream and read it into a list using the StreamReader.
                try
                {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(stream);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();    // Will read each line individually
                        width = line.Length;                // Used to measure the amount of chars within each line 
                        rows.Add(line);                     // Add each line within the file to a list.
                    }

                    reader.Close();

                    initSuccess = true;
                    Console.WriteLine("Initialisation worked!\n\n");
                }

                catch (IOException)                         // If there are any errors then provide feedback
                {
                    initSuccess = false;
                    Console.WriteLine("Error!");
                }

                List<char> rowChars = new List<char>();         // Each element contains one row of the map file
                height = rows.Count;                            // gets the total amount of lines of the map
                map = new char[height][];                       // Y value / vertical length of map
                int rowCharsCount = 0;                          // Count variable

                // Extract each char from each line that was read from the map file.
                for (int i = 0; i < rows.Count; i++)
                {
                    rowChars.AddRange(rows[i].ToCharArray());
                }

                // Replace 'starting position' with player symbol 
                if (rowChars.Contains('S'))
                {
                    rowChars[rowChars.IndexOf('S')] = '@';
                }

                // Construct a jagged array to store each 'tile'.
                for (int y = 0; y < height; y++)
                {
                    map[y] = new char[width];
                    for (int x = 0; x < width; x++)
                    {
                        map[y][x] = rowChars[rowCharsCount];
                        rowCharsCount++;
                    }
                }

                GetCurrentMapState();
            }

            return initSuccess;
        }

        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         */
        public char[][] GetOriginalMap()
        {
            // Draw map to screen
            DrawMap(this.map);

            return this.map;
        }

        /*
         * Returns the current map state 
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            // Create a copy of the original map
            if (currentMapLoaded == false)
            {
                mapCopy = new char[map.Length][];

                for (int y = 0; y < map.Length; y++)
                {
                    mapCopy[y] = (char[])map[y].Clone();
                }
                currentMapLoaded = true;
            }

            else { DrawMap(mapCopy); }

            // Draw the copy to the screen

            return mapCopy;
        }

        /*
         * A method that can be called to draw the map.
         */ 
        public char[][] DrawMap(char[][] map)
        {
            // Some code to just make the UI look neater
            string border1 = "   ╔";
            string border2 = "   ╚";

            for (int i=0; i<width-8; i++)
            {
                border1 += "═";
                border2 += "═";
            }

            border1 += "╗";
            border2 += "╝";

            Console.WriteLine(border1);
            Console.WriteLine("     GOLD: {0}    HEALTH: {1}", gold, pHealth);    // Draw gold
            Console.WriteLine(border2);
            Console.WriteLine();

            // Draw feedback to the screen
            if (charAtPos == 'G')
            {
                Console.WriteLine(" Press 'E' to collect the GOLD!");
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(map[y][x]);
                }
                Console.WriteLine();
            }
            return map;
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the x corrdinate and the second is the y coordinate on the map
         */
        public int[] GetPlayerPosition()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (mapCopy[y][x] == '@')               // Player is recognised by the @ symbol
                    {
                        position[0] = x;                    // First element of pos array is x
                        position[1] = y;                    // Second element of pos array is y
                        setPosition = true;
                    }
                }
            }

            return position;
        }

        /**
        * Returns the next player action
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {
            int action = (int)this.action;                  // Get the int value of the enum PlayerActions
            return action;
        }


        public bool GameIsRunning()
        {
            bool running = false;

            if (mapPlaying == true) { running = true; }     // Mad has been loaded and user has input 'play'

            if (gameOver == true) { running = false; }      // The player has reached the Exit on the map and has won.

            return running;
        }

        // Get a random number to determine how strong/powerful the monster is
        public void SetMonsterDamage()
        {
            int type = randNum.Next(1, 10);

            if (type <= 5) { mDamage = 3; }

            if (type >= 6 && type <= 8) { mDamage = 5; }

            if (type >= 9) { mDamage = 8; }
        }


        // Get a random number to determine how tanky/resistant the monster is
        public void SetMonsterHealth()
        {
            int type = randNum.Next(1, 5);

            if (type <= 2) { mHealth = 10; }

            if (type >= 3 && type <= 4) { mHealth = 12; }

            if (type == 5) { mHealth = 23; }
        }

        // Determine player damage via calculating the critical multiplyer (influenced by gold)
        public int PlayerDamage()
        {
            int crit = 1 + (gold/10);       // Gold will be used to modify the player's strength
            int damage = pDamage * crit;    // Damage = default damage * critical multiplyer

            return damage;
        }

        /**
         * Main method and Entry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            CMDCrawler crawler = new CMDCrawler();
            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" +Environment.NewLine+ 
                "May your Quest be filled with riches!"+Environment.NewLine);
            
            // Loops through the input and determines when the game should quit
            while (crawler.active && crawler.action != PlayerActions.QUIT)
            {
                Console.WriteLine("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);

                crawler.ProcessUserInput(input);
            
                crawler.GameLoop(crawler.active);
            }

            Console.WriteLine("See you again" +Environment.NewLine+ 
                "In the CMD Dungeon! ");


        }


    }
}
