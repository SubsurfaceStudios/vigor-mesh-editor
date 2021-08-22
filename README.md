# Vigor Mesh Editor
An open source runtime mesh editing API built on ProBuilder runtime and extended to allow for high-level API functions.
This is also what the Vigor XR Building System is built upon.
Note that this lacks some functions, but they can be performed manually.

This system is built upon the ProBuilder API made by Unity Technologies.

# Dependencies
-ProBuilder 4.x.x
-Unity Runtime Scene Serialization (URSS)
  This can be installed by going to Window > Package manager, clicking add, then clicking "Install from Git URL". Type in "com.unity.runtime-scene-serialization" and press add.

# Notes
The BuildingUtilityNew script has to placed on an empty GameObject in any scene where you want to use its functions, and all variables on it except "other" need to be set in the editor.

You can disable the editor UI for this script by commenting out or deleting `void OnGUI` and all the code it contains.

