# Reminstance
Reminstance or Rememeber Instance is a Game Creator 2 module, expand the Remember component functionality. It allow save and load Runtime Instances just using a component same Remember. This is the most easy wait to save and load prefab instances in Game Creator 2 

 - Save instances in specific slot: Will create a specific json file for each saved slot. The json contains all instantiated scenes.
 - Load instances from specific slot
 - Delete Instance Slot: Delete the slot json file along with data information
 - Instantiation instructions
 - On Instance Triggers: Trigger by instantiation types
 - GameCreator Settings -> Reminstance: Allows the user to manage all prefabs that can be instantiated at runtime for persistence
 - GameCreator Installation - Reminiscence Samples: Allows the user to install or uninstall Game Creator Manager samples
 - RememberInsance component: Like the native Remember component, but this component allows instance to save and load the game.
 - Nested instantiations: allows you to save and load instances of the correct hierarchy

### Download Unity Packages:
You can download Reminstance versions in unity package format, direct from this link:
https://drive.google.com/drive/folders/1UehB9TzQU6tNZRTKc__4tWE1Sk-eWNUH?usp=sharing

### How to Contribute:
Feel free to contribute to this development by fork this repository. 
Join DoubleHitGames at: https://discord.gg/muMDQP6qQB

### How to Install:
- 1: Download package from link
- 2: Extract the package in Unity project and done!

### How to Install Samples?:
- 1: In unity tools bar go to: `Game Creator -> Install -> Reminstance -> Examples -> Install`
*"After installing the sample pack from the Game Creator installer, a known bug causes the installed icon to not appear in the menu. Even though the example is installed."* 

- 2: After install, the example can be found in `Asstes\Plugins\Game Creator\Installs\Reminstance\Reminstace.Examples@0.0...`

# How to use?:
- 1 - Add `RememberInstance` Component in any prefabs that you are using to save instatiation.
*"You can see all prefabs using to instance in Game Creator tool -> Settings -> Reminstance."*

- 2 - For `intenventory item`, just make sure that their items are using `RememberInstance` instead `Remember` Component. 
*"So just drop items from bag or to use inventory instructions to instantiate some item in the world"*

- 3 - For any other instance you can to use the `Reminstance->Instantiate instruction` to instantiate any RememberInstance prefab in the world. 
*"Make sure that your prefabs are using RememberInstance instead Remember component."*

- 4 - Read instantiation, loaded and droped events from `Reminstance Trigger`.
  
  

