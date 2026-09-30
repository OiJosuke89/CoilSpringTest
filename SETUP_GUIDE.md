# Coil Release - Unity Editor Setup Guide

Welcome to the "COIL RELEASE" Setup Guide. Since the project code has been generated, this guide will instruct you step-by-step on how to attach the generated scripts (`Assets/Scripts/...`) to your Unity scene to match the GDD and UI mockups provided.

## 1. Project Setup
1. Unzip the provided package directly into the `Assets` folder of a new Unity 2D project.
2. The folder structure should look like this:
   - `Assets/Scripts/Core`
   - `Assets/Scripts/Gameplay`
   - `Assets/Scripts/Editor`

## 2. Setting Up the UI ("Family B" Standards)
Based on the `image.png` mockups and GDD, you will set up standard overlays.
1. Create a new **Canvas** (GameObject -> UI -> Canvas). Name it `UICanvas`.
2. Add the `CRUIManager.cs` script to `UICanvas`.
3. Create the required UI panels as children of `UICanvas`:
   - **WinPanel**: A panel shown on victory.
   - **LosePanel**: A panel shown when gates run out and targets aren't hit.
   - **PausePanel**: A panel with resume/restart logic.
   - **TopBar**: Contains the Level Text and the Pause Button.
4. Drag these UI GameObjects into the corresponding slots in the `CRUIManager` component on `UICanvas`.

## 3. Creating Gameplay Prefabs
The mockup shows metallic casing (Gate), coiled wire (Coil), and pins (Jam Pins/Targets).
1. **Gate Prefab**: Create a Sprite (e.g., a lock/metallic casing). Make sure it has a `BoxCollider2D`. Save it as a prefab.
2. **Coil Prefab**: Create a Sprite (a wound gear/wire). Save it as a prefab.
3. **Coil Segment Prefab**: Create a Sprite (straight wire/metal). Save it as a prefab.
4. **Jam Pin Prefab**: Create a Sprite (the pin obstacle). Save it as a prefab.
5. **Target Prefab**: Create a Sprite (the end goal socket). Save it as a prefab.

## 4. Setting up the Game Manager
1. Create an Empty GameObject in your scene named `GameManager`.
2. Attach the `CRGameManager.cs` script.
3. Attach the `CRViewManager.cs` script.
4. On `CRViewManager`, assign the prefabs you created in step 3 to their corresponding slots (`Coil Prefab`, `Gate Prefab`, `Jam Pin Prefab`, `Target Prefab`, `Coil Segment Prefab`).
5. Adjust `Cell Size` (default 1.0f) and `Grid Offset` (default (0,0)) on `CRViewManager` to align the grid to your camera based on the mockups.

## 5. Setting up Input
1. Attach the `CRInputManager.cs` script to the Main Camera (or the `GameManager`).
2. Ensure your Main Camera is orthographic.
3. The InputManager will automatically cast 2D raycasts looking for objects named "Gate_*" when tapped/clicked.

## 6. Creating and Validating Level Data
1. In your Project window, Right-Click -> `Create` -> `CoilRelease` -> `Level Data`.
2. Select the new asset (`NewCRLevelData.asset`).
3. In the Inspector, specify your grid size (e.g., 10x15).
4. Add elements (Coils, Gates, Jam Pins, Targets). Make sure to assign IDs (e.g., `coil_1`, `gate_1`) and target references correctly.
5. Click **"Validate Level Data"** at the bottom of the inspector (this uses the `CRLevelValidator.cs` we wrote in the Editor folder) to ensure there are no overlapping objects or invalid setups.
6. Drag the valid `CRLevelData` asset into the `Current Level Data` slot of the `CRGameManager` in your scene.

## 7. Play!
Hit Play in the editor.
- The `CRGameManager` will load the `CRLevelData` into the deterministic `CRCoilSimulator`.
- The `CRViewManager` will spawn the prefabs at their grid positions.
- Tapping/Clicking a Gate will tell `CRCoilSimulator` to release it, unwinding the wire.
- If the coil hits the target, it wins.
- Standard Family B UI panels will show up on win/loss.
