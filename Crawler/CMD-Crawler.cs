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
        //use the following to store and control the next movement of the yser
        public enum PlayerActions {NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, QUIT };
        private PlayerActions action = PlayerActions.NOTHING;

        private bool active = true;                         // Tracks if the game is running
            
        private FileStream stream = null;                   // Persistent filestream
        private List<string> rows = new List<string>();     // List of all lines read from file
        private char[][] map = new char[0][];                               // Persistent map variable
        private char[][] mapCopy;                           // This is a copy of the original map - changes will be applied to this version
        private int height = 0;                             // Vertical length of map
        private int width = 0;                              // Horizontal length of the map

        private bool gameOver = false;                      // Track if game has ended
        private bool mapLoaded = false;                     // Has user selected map?
        private bool mapPlaying = false;                    // Has user input 'play'?
        private bool currentMapLoaded = false;              // Track if the map has been loaded

        private int gold = 0;                               // Currency - advanced feature

        private int[] position = { 0, 0 };                  // Store player position
        private int[] positionCopy = { 0, 0 };              // Temp player position - used to check for collisions
        private bool setPosition = false;                   // Track if the starting position has been set
        private char currentChar = '.';                     // Store the char that the player symbol has replaced
        private char charAtPos;                             // Store the char that the player wants to move to

        /**
         * Reads user input from the Console
         * Please use and implement this method to read the user input.
         * Return the input as string to be further processed
         */
        private string ReadUserInput()
        {
            string inputRead = string.Empty;
            inputRead = Console.ReadLine();                 // Player must type an input and press 'Enter' to submit action.

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

            // User can quit or close the game via this input
            if (input == "quit" || input == "q")
            {
                action = PlayerActions.QUIT;
            }

            // If a map isnt already loaded then when the user inputs a load command, initialize the corresponding map file.
            if (!mapLoaded)
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
                    string fileName = "Advanced.map";
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
                    mapPlaying = true;
                }

                // If the user input is incorrect
                else
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
            }

            if (GetPlayerAction() == 2)                     // Input = 'A', move West
            {
                positionCopy[0] = x + 1;
                MakeMove();
            }

            if (GetPlayerAction() == 3)                     // Input = 'S', move South
            {
                positionCopy[1] = y + 1;
                MakeMove();
            }

            if (GetPlayerAction() == 4)                     // Input = 'D', move East
            {
                positionCopy[0] = x - 1;
                MakeMove();
            }

            if (GetPlayerAction() == 5)                     // Input = 'E', pick-up gold
            {
                gold += 1;
                currentChar = '.';
            }

            if (GetPlayerAction() == 7)                     // Input = quit, end game
            {
                this.active = false;
            }

            action = PlayerActions.NOTHING;                 // Reset 'action' after making an action.
        }

        public bool CanMove()
        {
            int x = positionCopy[0];
            int y = positionCopy[1];
            charAtPos = mapCopy[y][x];
            bool canMove = false;

            if (charAtPos == '#') {canMove = false;}        // Player cannot move onto tiles that are Walls 

            if (charAtPos == 'M') {canMove = false;}        // Player cannot move onto tiles that are Monsters

            if (charAtPos == 'G') {canMove = true;}         // Player can move onto Gold

            if (charAtPos == '.') {canMove = true;}         // Player can freely move through empty spaces

            if (charAtPos == 'E') {canMove = true;}         // Player can move onto ending tile which will finish the level.

            return canMove;
        }

        public void MakeMove()
        {
            if (CanMove() == false)
            {
                Console.WriteLine("OUCH! You seem to have collided with something in the darkness...");
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
        * Output text to screen when the player has beat the game
        * Switch state of variables to change gamestate
        */ 
        public void GameOver()
        {
            Console.WriteLine("\n\n");


            Console.WriteLine(" ╔═════════════════╗ ");
            Console.WriteLine(" ║ CONGRATULATIONS ║ ");
            Console.WriteLine(" ║ YOU HAVE BEATEN ║ "); 
            Console.WriteLine(" ║    THE GAME!    ║ ");
            Console.WriteLine(" ╚═════════════════╝ ");

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

            Console.WriteLine(map.Length);

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
            Console.WriteLine("     GOLD: {0}", gold);    // Draw gold
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
