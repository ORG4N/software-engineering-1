﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
        private char[][] map;                               // Persistent map variable
        private char[][] mapCopy;                           // This is a copy of the original map - changes will be applied to this version
        private int height = 0;                             // Vertical length of map
        private int width = 0;                              // Horizontal length of the map

        private bool mapLoaded = false;                     // Has user selected map?
        private bool mapPlaying = false;                    // Has user input 'play'?

        public int gold = 0;                                // Currency - advanced feature

        int[] position = { 0, 0 };

        /**
         * Reads user input from the Console
         * Please use and implement this method to read the user input.
         * Return the input as string to be further processed
         */
        private string ReadUserInput()
        {
            string inputRead = string.Empty;

            // Your code here
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

            if (input == "quit")
            {
                action = PlayerActions.QUIT;
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

            // If the user inputs "play" then the map will be drawn.
            if (mapLoaded && !mapPlaying)
            {
                if (input == "play")
                {
                    Console.WriteLine("Loading map...");
                    GetOriginalMap();
                    mapPlaying = true;
                }
            }

            if (mapPlaying)
            {
                if (input == "w")
                {
                    action = PlayerActions.NORTH;
                }

                if (input == "a")
                {
                    action = PlayerActions.EAST;
                }

                if (input == "s")
                {
                    action = PlayerActions.SOUTH;
                }

                if (input == "d")
                {
                    action = PlayerActions.WEST;
                }
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
            // Your code here

            int x = position[0];
            int y = position[1];
            
            if (GetPlayerAction() == 1)                     // Input = 'W', move North
            {
                position[1] = y + 1;

                Console.WriteLine("({0} , {1})", position[0], position[1]);
            }

            if (GetPlayerAction() == 2)                     // Input = 'A', move West
            {
                position[0] = x - 1;
                Console.WriteLine("({0} , {1})", position[0], position[1]);

            }

            if (GetPlayerAction() == 3)                     // Input = 'S', move South
            {
                position[1] = y - 1;

                Console.WriteLine("({0} , {1})", position[0], position[1]);

            }

            if (GetPlayerAction() == 4)                     // Input = 'D', move East
            {
                position[0] = x + 1;
                Console.WriteLine("({0} , {1})", position[0], position[1]);
            }
        }

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * Create a private object variable for storing the map in Crawler and using it in the game.
        */
        public bool InitializeMap(String mapName)
        {
            bool initSuccess = false;

            // Your code here

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

                catch (IOException)
                {
                    initSuccess = false;
                    Console.WriteLine("Error!");
                }
            }

            return initSuccess;
        }

        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         */
        public char[][] GetOriginalMap()
        {
            // Your code here

            List<char> rowChars = new List<char>();
            height = rows.Count;    // gets the total amount of lines of the map
            this.map = new char[height][];
            int rowCharsCount = 0;

            // Extract each char from each line that was read from the map file.
            for (int i = 0; i < rows.Count; i++)
            {
                rowChars.AddRange(rows[i].ToCharArray());
            }

            if (rowChars.Contains('S'))
            {
                rowChars[rowChars.IndexOf('S')] = '@';
            }

            // Construct a jagged array to store each 'tile'.
            for (int y = 0; y < height; y++)
            {
                this.map[y] = new char[width];
                for (int x = 0; x < width; x++)
                {
                    this.map[y][x] = rowChars[rowCharsCount];
                    rowCharsCount++;
                }
            }

            DrawMap(map);

            return map;
        }

        /*
         * Returns the current map state 
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            // the map should be map[y][x]
            // Your code here

            mapCopy = new char[map.Length][];

            for (int y=0; y< map.Length; y++)
            {
                mapCopy[y] = (char[])map[y].Clone();
            }

            DrawMap(mapCopy);

            return map;
        }

        /*
         * A method that can be called to draw the map.
         */ 
        public char[][] DrawMap(char[][] map)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(this.map[y][x]);
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
                    if (map[y][x] == '@')
                    {
                        position[0] = x;
                        position[1] = y;
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
            int action = (int)this.action;

            return action;
        }


        public bool GameIsRunning()
        {
            bool running = false;

            if (mapPlaying == true) { running = true; }

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
