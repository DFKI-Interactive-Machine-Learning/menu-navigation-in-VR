# Gaze-Based Menu Navigation in Virtual Reality: A Comparative Study of Layouts and Interaction Techniques
This repository accompanies the paper "Gaze-Based Menu Navigation in Virtual Reality: A Comparative Study of Layouts and Interaction Techniques", presented at 20th IFIP TC13 International Conference on Human-Computer Interaction (INTERACT 2025).

## Overview

This Unity project investigates gaze-based menu navigation in virtual reality, comparing different menu layouts (e.g., Pie and List menus) and interaction techniques (e.g., dwelling, border-crossing, controller, and multimodal input). The project builds on previous research and utilizes both original and adapted assets.

## Dependencies

The project requires these Unity packages:
- [OpenXR Plugin](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.13/manual/index.html)
- [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html)

Several assets are adapted from [Kim et al. 2022](https://github.com/taejun20/LatticeMenu).

## Folder Structure

- **UserStudy/**: Contains evaluation scripts and a dedicated README with further details.
- **Assets/IML/Gaze/Scripts/**: Scripts for the Gaze Accuracy Grid:
  - `GazeAccuracy.cs`: Implements gaze grid logic, gaze and menu hit event tracking, and related data structures.
  - `StartGazeAccTest.cs`: Handles transitions between scene kiosks and the gaze accuracy grid.
  - `TrackEyePosScript.cs`: Tracks gaze activity and menu hit events across different menu levels.
- **Assets/Menus/**: Menu implementations, prefabs, and utility scripts:
  - `Editor/`: Editor extensions for transform visualization of nested GameObjects.
  - `Lattice/`: Pie menus and logic for menu expansion and hover effects.
  - `PanelMenu/`: List menu logic, analogous in structure to Pie menus.
  - `SUS/`: System Usability Scale (SUS) integrated panel and logic.
- **Assets/DwellinteractionScript.cs**: Dwell-based progress bar logic.
- **Assets/JsonLogger.cs**: Data structures and serialization routines for user study logging.
- **Assets/TaskManager.cs**: Core study manager, handles menu layout data and orchestrates study flow, including utility functions.
- **Assets/LookAtCameraScript.cs**: Rotates list menus to face the user.
- **Assets/RotateRadialToCameraScript.cs**: Rotates pie menus towards the user.

## How to Run

1. Clone the repository and open it in Unity.
2. Ensure the required dependencies are installed (see above).
3. Refer to the README in `UserStudy/` for instructions on evaluating the user studies.

## Citation

If you use this codebase in your research, please cite the following:
```
@inproceedings{10.1145/3677386.3688887,
	author = {Kop\'{a}csi, L\'{a}szl\'{o} and Klimenko, Albert and Barz, Michael and Sonntag, Daniel},
	title = {Exploring Gaze-Based Menu Navigation in Virtual Environments},
	year = {2024},
	isbn = {9798400710889},
	publisher = {Association for Computing Machinery},
	address = {New York, NY, USA},
	url = {https://doi.org/10.1145/3677386.3688887},
	doi = {10.1145/3677386.3688887},
	abstract = {With the integration of eye tracking technologies in Augmented Reality (AR) and Virtual Reality (VR) headsets, gaze-based interactions have opened up new possibilities for user interface design, including menu navigation. Prior research in gaze-based menu navigation in VR has predominantly focused on pie menus, yet recent studies indicate a user preference for list layouts. However, the comparison of gaze-based interactions on list menus is lacking in the literature. This work aims to fill this gap by exploring the viability of list menus for multi-level gaze-based menu navigation in VR and evaluating the efficiency of various gaze-based interactions, such as dwelling and border-crossing, against traditional controller navigation and multi-modal interaction using gaze and button press.},
	booktitle = {Proceedings of the 2024 ACM Symposium on Spatial User Interaction},
	articleno = {40},
	numpages = {2},
	keywords = {Extended Reality (XR), Eye Tracking, Gaze-based Interaction, Menu Navigation},
	location = {Trier, Germany},
	series = {SUI '24}
}
```

## Follow-up Work

Reusable assets from this work will be available at: https://github.com/DFKI-Interactive-Machine-Learning/com.dfki-iml.xr-gaze-interaction-toolkit
