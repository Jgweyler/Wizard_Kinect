# Wizard Kinect

This is a project to evaluate the advantages and disadvantages of developing with Kinect SDK 2.0.

To test this, I design a game prototype (still in alpha version) with Unity where the user controls a little wizard who has to destroy all the enemies in the scene.

## Before start:

Make sure you have installed:

* [Unity](https://unity3d.com/es)
* [Blender](https://www.blender.org/)
* [Kinect SDK 2.0](https://developer.microsoft.com/es-es/windows/kinect)

## After clone this repository:

You'll need  to add a depency to the project that is located in Unity installation folder:

```
C:\Program Files (x86)\Unity\Editor\Data\MonoBleedingEdge\lib\mono\2.0\ICSharpCode.SharpZipLib.dll
```
Copy that file and paste it in:
```
<project folder>/Assets/KinectScripts/SharpZipLib
```

If the Wizard model didn't load properly in the project, try to reimport it.

## The objetive of the game

This game is a 'Beat them all' type where the player/Wizard has to change between different spells to destroy all enemies (cubes).
These enemies are using a specific type of spell (FIRE, WATER, EARTH, ICE, THUNDER, WIND). The player must select the vulnerability spell
of the enemy type to destroy them. See table below:

|Element  |Vulnerability    |
| --------|-----------------|
| FIRE    |     WATER       |
| WATER   |     THUNDER     |
| THUNDER |     EARTH       |
| EARTH   |     WIND        |
| WIND    |     ICE         |
| ICE     |     FIRE        |

**Important:** This porject is set with only one enemy for testing. If you want more enemies change the "numEnemies" property from the GameManager.cs script.

## Interaction with Kinect

To select a spell, I used speech recognition using simple words (in spanish):
"Fuego", "Agua", "Tierra", "Hielo", "Rayo", "Viento".

You can create your own grammar and install your language package. See [documentation.](https://msdn.microsoft.com/en-us/library/jj131034.aspx)

### Gestures

- **Move forward:** Make one step forward
- **Turn Left/Right:** Rise your (left/right) arm to the (right/left).
- **Move backwards / Remove target:** Rise your arms. Your elbows must be lined up with your shoulders.
- **Focus target:** Get close to an enemy, then move your (left/right) arm to the height of your shoulder, pointing with the index finger to the front.
- **Charge spell:** This gesture tries to imitate to someone who is prepearing a strong punch.
- **Throw spell:** Complete the movement of a punch.
- **Confirm spell selection:** Put your hands together at the height of your chest. (Imitate someone who prays).

You can see the full documentation [here](https://drive.google.com/file/d/1OQ6ypAus1_X7Y8tlD0iuVZtK4uvrSXA2/view?usp=sharing) in Spanish.

Check also the [video](https://www.youtube.com/watch?v=FF7i1bP04uw&t=1s) demostration.

## Author

[Jgweyler](https://github.com/Jgweyler/)



