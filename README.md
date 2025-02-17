# Gaze-Based Menu Navigation in Virtual Reality: A Comparative Study of Layouts and Interaction Techniques

## Note: further documentation will be added soon

## Dependencies

To run this unity project you will need:
- [OpenXR](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.13/manual/index.html)
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html)
- many assets are taken from [Kim et al.](https://github.com/taejun13/LatticeMenu) https://github.com/taejun13/LatticeMenu  

### Folder Structure

UserStudy contains the scripts used to evaluate the user study, see the corresponding readme for more information.

Assets/IML/Gaze/Scripts contains the scripts reponsible for the Gaze Accuracy Grid. (GazeAccuracy, StartGazeAccTest, TrackEyePosScript)
- GazeAccuracy: holds the logic of gaze accuracy grid. it tracks the gaze hit events and evaluates them. also the datastructures for gaze hit events and menu hit events are stored tehre
- StartGazeAccTest: holds the logic of transitioning between the kiosk element of the scene and the gaze accuracy grid
- TrackEyePosScript: holds the logic of tracking menu hit events. this is a general hit event that tracks eye movement on a menu for every menu level.


Assets/Menus contains the logic of all menus, their prefabs and related utility functions. All menus are designed with [prefab variants](https://docs.unity3d.com/Manual/PrefabVariants.html). This enabled fast prototyping while changes to the base prefab affects all variants
- Assets/Menus/Editor contains Editor extensions to visualize the global size of nested gameobjects on their transform component
- Assets/Menus/Lattice contains the Pie menus and the pie menu logic. the pie menu logic is reposible for menu expansion and hover effects
- Assets/Menus/PanelMenu contains the List menus and its logic. same as Pie menu
- Assets/Menus/SUS contains the integrated SUS panel and its logic
- Assets/DwellinteractionScript.cs holds the logic of dwell progessbar
- Assets/JsonLogger.cs holds the datastructures that describe the user study. it also writes them to memory
- Assets/TaskManager.cs Main script. holds datastructures for the menu layouts and handels general procedure of user study. contains ulity function.

Assets/LookAtCameraScript.cs ulity script to rotate List menu to face users.\
Assets/RotateRadialToCameraScript.cs ulity script to rotate Pie menu to face users.
