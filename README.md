
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
![Installation gif]()

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
The maps in CMD DUNGEON are built from tiles, of which there are a variety of.
![Tilemap]

Players must navigate through the maze-like dungeon levels to find the Exit. Among their journey they may notice Monsters and Gold. 

Monsters are hostile enemies that the player can interact with - but they cannot pass through them unless slain. On the otherhand, Gold is a collectible item - within **Additional Features** I show how I have expanded upon this to give Gold a meaningful purpose. 

CMD DUNGEON's framework is very adaptable and can easily be extended upon to incorporate a larger tilemap and greater variety of environmental hazards.

### Combat
![Player VS Enemy combat](https://github.com/Plymouth-Comp/coursework2-ORG4N/blob/master/images/Combat.gif)

## Structure of code

## Additional Features
* Players damage scales with gold
* Monsters health scales with the amount of times the player replays - make the game more adventurous and challenging
* Monsters are assigned damage and health properties randomly
* Two new maps (mayhem.map and arena.map)
* Monsters can randomly move onto any tiles but freeze every 5 turns
* Help menu


## To-Do
- [ ] Record video
- [ ] Record short Gameplay Demo gif
- [ ] Record short Getting Started gif
- [ ] Explain symbols and commands and controls im Getting Started
- [ ] Explain structure of code (methods, game flow, game states)
- [ ] Explain usage of methods
- [ ] Explain which features I have added 
