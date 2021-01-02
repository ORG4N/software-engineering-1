
![title image](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Title.png)
> As part of my Software Engineering 1 module I have been tasked to create a Dungeon Crawler console game using a given codebase. 

CMD DUNGEON is an exciting but compact game that has been designed for Command Line Interfaces. From slaying monsters to collecting gold, CMD DUNGEON takes the player on a high-risk adventure within the Command Line.

## Gameplay Features
These are the most prominent features that define what type of game CMD DUNGEON truly is:
* PVE Combat
* Hardmode
* Monster and player scaling
* Interactable items (gold and health potions)
* A variety of different maps to explore

## Getting Started
### Installation
1. Download files from repo to local machine
   1. Clone from GitHub, OR
   2. Download as ZIP and extract 
2. Run the .exe to open the game (coursework2-ORG4N > Crawler > CMD-Crawler.exe)
   1. Double click on the CMD-Crawler.exe within file explorer, or
   2. Execute the .exe through the Command Line
   
![Installation gif](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Installation.gif)


* Further instructions for executing .exe through Command Line:
   * Navigate to the following path using cd: coursework2-ORG4N > Crawler
   * Input the following command: Start-Process CMD-Crawler.exe.lnk
   
(Refer to GIF to see visual instructions on how to open from within the Command Line)
 

### Navigating the menu
![using the load and play commands](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Load%20and%20Play.gif)
Upon opening CMD DUNGEON the player will be expected to choose a specific map and submit a request to play their chosen map. The user can do this using the following two commands:
* load [mapname]
* play

Inputs, which are case insensitive can be submitetd within the Command Line by pressing Enter - upon which feedback will be written in response to the player's action.

Players may also find some more assistance within the game via the 'help'. At this stage of the game the user also has the chance to input another three additional commands ('hard', 'replay', 'all'). These commands will be further explained within **Additional Features**.

## Gameplay Demos and Walkthrough
Within the dungeon the player can create their own story - how they want to play the game is largely up to them. The player might find enjoyment in being a completionist and collecting all the coins; or they might prefer hacking and slashing their way through hordes of monsters. 

### Adventuring
> *A demonstration of the **'simple.map'** level.*
![Simple gameplay functionality](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Simple%20Gameplay.gif)

#### Tilemap:
Player | Wall | Empty Space | Gold | Monster | Exit | 
-------|------|-------------|------|---------|-------
@ | # | Period/Fullstop | G | M | E 

#### Controls:
Up | Left | Down | Right | Interact | Attack | Quit
---|-----|-----|------|---------|-------|-----
W | A | S | D | E | Spacebar | Q |

Players must navigate through the maze-like dungeon levels to find the Exit. Among their journey they may notice Monsters and Gold.

Monsters are hostile enemies that the player can interact with - but they cannot pass through them unless slain. On the otherhand, Gold is a collectible item - within **Additional Features** I show how I have expanded upon this to give Gold a meaningful purpose. 

CMD DUNGEON's framework is very adaptable and can easily be extended upon to incorporate a larger tilemap and greater variety of environmental hazards.

### Combat
> *A demonstration of basic combat.*
![Player VS Enemy combat](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Combat.gif)

Taking inspiration from turn-based PVE style games, CMD DUNGEON has a very simple implementation of combat. When a player tries to move onto a monster's tile they have the option to enter combat using Spacebar. However, there is a 50-50 chance of whoever gains initiative (first attack). Attacks will repeatedly go back and forth until either the monster or the player has died, of which the latter concludes an Adventure. 

However, this is simply a foundation, and the more advanced aspects will be explored within **Additional Features**

## Additional Features
Combat | Gameplay | Other
-------|----------|------
Player damage scaling | Random monster movement | Help menu
Monster health scaling | Health potions | Two new maps
Random monster stats | Heads-Up Display | Multiple monsters
Monsters can attack players| Replay maps |

### Replayability and Monster Movement
Replayabiltiy can be enabled before loading a map by inputting either **'replay'**. This means that once a player has reached the Exit they can select and load another map (keeping their gold).

Random monster movement is always enabled, yet by default they are not allowed to attack players first (a player can be the only one to start combat, even if they are not the first to deal damage). However, by inputting **'hard'**, monsters will be enabled to attack players without player confirmation - therefore this can be classified as Hard Mode.

For simplicity, both of the above commands can also be toggled by inputting 'all'. A third command - **'help'**, provides this information from within the game and may be useful to beginning players.

### Monster and Player Scalability
Each time a player chooses to replay the game, the monsters will scale in Health. Becoming harder to kill, the game becomes more challenging, and player scaling becomes a necessity. Therefore, by collecting and hoarding Gold, by slaying Monsters and picking it up from the ground, the player can increase their Attack Damage. In simpler terms, depending on the amount of Gold collected, the player recieves an additional Critical Multiplier to their damage. Gold, Health and Damage are shown through the HUD.

### Other Monster Advancements
Upon entering combat with a monster, its Health and Damage stats are randomly assigned (based on a three tier system). There are three possible hierarchies for Health, and another three for Damage. However, unlike Damage, the three possible tiers of Health can scale. I believe this feature makes the game challenging and fun as it is always a gamble when initiating combat. 

CMD-DUNGEON's framework also supports multiple independent monsters, each with their own stats and movements. The two additional maps, **'arena'** and **'mayhem'**, showcase this.

### Health Potions
The game I have created is challenging and based on luck. Therefore, Health Potion tiles can be recognised on maps as a plus (**+**) symbol. Like Gold, these potions are interactable with the 'E' input. However, be aware that your Health resets to the base value upon reaching the exit.

## Structure of code
In this section I will explore the structure of CMD DUNGEON and present each method's purpose.

Method|Description 
------|-------------
Main()| This method contains the main game loop that will run whilst the user does not command the game to close, or the game state has not changed to inactive. This method can be thought of as a container for the game - everything will be run inside the while loop.
ReadUserInput()| User input can be read a key at a time (when the game is in the playing state), or it can be read until the user submits the input with the Enter key. These inputs can then be fed into ProcessUserInput().
ProcessUserInput()| Depending on certain inputs, the game has to react a certain way. The current gamestate must also be considered when processing inputs. The method is generally used to change gamestate but also set the current action that should be further carried out within the GameLoop() method.
GameLoop()| This method responds to the processed input and is therefore used to: make character movements; interact with Gold, Monsters, and Health Potions; and, drawing the map/HUD to the screen. To make my game organised I have split significant pieces of code into their own methods and call them within GameLoop().
InitializeMap()| Within the game files are 2D maps that can be loaded as levels for the player to play. This method is called upon loading a map, and it is tailored to work with any map witthin the game files. Using a jagged array also gives this method the advantage of being compatable with irregularly shaped maps.
GetOriginalMap()| This method simply returns the initial state of the map that is created via InitializeMap().
GetCurrentMapState()| A copy of the original map is made (mapCopy). The purpose of this copy is so that changes can be applied, and if the initial map needs to be drawn again then it does not need to be re-initialised.
DrawMap()| Having an independent method to draw the mapCopy to the screen means that the code can be easily re-used. This method also includes code to write a HUD to the screen, and even provide textual feedback to the player if they are on an interactable tile - such as Gold or Health Potions.
GetPlayerPosition()| The mapCopy is iterated through until the x and y positions of the player are found. This method is used within GameLoop() to get the original starting position of the player.
GetMonsterPositions()| Similar to the previous method, this method gets the positions of the 'M' symbols (note the plurality). Therefore, to store various x and y posittions a list is used, with every 2 elements being one single coordinate.
GetPlayerAction()| The enum action stores a playerstate, such as the direction of movement or an interaction. The value of this enum is changed and set within ProcessUserInput() to be used within GameLoop(), wherein GetPlayerAction() is called to return the int representation of the enum.
GameIsRunning()| The gamestate running can be determined via logic. The purpose of this method is for testing purposes and overall has no effect on the game.
CanMove()| By creating a copy of the current position and then applying a theoretical movement to this copy, the method can determine if whether the movement the player is attempting to make is legal or invalid. This method has been tailored for use by both Monsters and Players, and therefore has an Entity parameter to differentiate between whatever entity is performing the action.
MakeMove()| After assessing the validity of the move, the movement is either applied to the original player position, or feedback about the invalidity of the movement is drawn to the screen and no movement is made. Likewise, if the movement is into the Exit tile the gamestate is set to game over.
MoveMonsters()| An algorithm similar to the above method is used when moving the monsters, however it has been adapted to move multiple monsters and each of them in a random direction. This method also uses CanMove() to ascertain validity of this randomly generated movement. If hardmode is enabled and a monster moves onto a player tile then the player is instantly and automatically attacked, without requirng confirmation.
Replay()| This method is invoked when the player reaches the exit. If the user has enabled replayability then they are given an option to load a new map, otherwise the game ends. In the case that the player chooses to replay then all of the class variables are reset to their default values, and lists are cleared - except from the gold counter, which carries over into the next level.
Combat()| Combat has been previously described within this readme. It is called within the GameLoop() method whenever the player has input Spacebar and there is a monster on the opposing tile. Players may die in Combat() which will then invoke Replay().
MonsterAttack()| Monster damage is deducted from player health and the damage recieved is drawn to the screen. Called within Combat().
PlayerAttack()| Player damage is deducted from monster health and the damage dealt is drawn to the screen. Called within Combat().
SetMonsterDamage()| Called at the start of combat, to determine a monster's damage stats. There are three tiers - hard, medium, easy. Uses the random class to determine the monster's strength.
SetMonsterHealth()|  Scales with the amount of map replays. The game will get harder the longer it is played for. Invoked alongside SetMonsterDamage() at the start of Combat().
PlayerDamage()| Scales expontentially with Gold. The algorithm applies a critical effect to damage and therefore makes collecting gold a necessity later on in the game. Called whenever Gold is interacted with.


## To-Do
- [ ] Record video
- [ ] Record short Gameplay Demo gif
- [ ] Record short Getting Started gif
- [ ] Explain symbols and commands and controls im Getting Started
- [ ] Explain structure of code (methods, game flow, game states)
- [ ] Explain usage of methods
- [ ] Explain which features I have added 
