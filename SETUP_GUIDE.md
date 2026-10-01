# Coil Release - Complete Unity Setup Guide

Welcome! This guide is written for beginners. It will walk you through setting up the Unity project step-by-step from an empty scene to a fully playable 1D-track prototype of "Coil Release" using basic Unity shapes.

Please follow these instructions in order.

---

## Part 1: Project & Folders Setup

1. **Create a New Project:** Open Unity Hub and create a new **2D Core** project.
2. **Import the Code:** Take the unzipped `Assets` folder provided to you and drag it into your Unity Project window, merging it with your existing `Assets` folder.
   * *Your Project window should now have a `Scripts` folder containing `Core`, `Gameplay`, and `Editor` folders.*
3. **Create a Prefabs Folder:** In the Project window, right-click the `Assets` folder -> `Create` -> `Folder`. Name it `Prefabs`.

---

## Part 2: Creating the Visual Prefabs

We need visual representations for the tracks, coils, gates, targets, and jam pins. We will use Unity's basic shapes.

### 1. The Track Prefab
1. Right-click in the **Hierarchy** window -> `Create Empty`. Name it `TrackVisual`.
2. With `TrackVisual` selected, go to the **Inspector** -> `Add Component` -> search for `Line Renderer`.
3. In the Line Renderer component:
   * Expand **Materials**. Set the Element 0 to `Sprites-Default` (click the tiny circle icon to search for it).
   * Set **Width** to `0.1`.
   * Set the **Color** to a dark grey.
4. Drag `TrackVisual` from the Hierarchy into your `Prefabs` folder in the Project window to save it as a prefab.
5. **Delete** `TrackVisual` from the Hierarchy.

### 2. The Coil Prefab
1. Right-click in the Hierarchy -> `2D Object` -> `Sprites` -> `Circle`. Name it `CoilVisual`.
2. In the Inspector, change its **Color** to Blue.
3. Drag `CoilVisual` into the `Prefabs` folder.
4. **Delete** it from the Hierarchy.

### 3. The Ghost Coil Prefab
1. Drag the `CoilVisual` prefab you just made from the Project window back into the Hierarchy. Name it `GhostCoilVisual`.
2. In the Inspector, change the Sprite Renderer's **Color** Alpha (the 'A' value at the bottom of the color picker) to `100` so it is semi-transparent.
3. Drag `GhostCoilVisual` into the `Prefabs` folder.
4. **Delete** it from the Hierarchy.

### 4. The Gate Prefab
1. Right-click in the Hierarchy -> `2D Object` -> `Sprites` -> `Square`. Name it `GateVisual`.
2. **Crucial Step:** In the Inspector, click `Add Component` -> search for `Box Collider 2D`. (Without this, you cannot click the gate!).
3. Change its **Color** to Green.
4. Scale it down slightly: Set Transform **Scale** to X: `0.6`, Y: `0.6`.
5. Drag `GateVisual` into the `Prefabs` folder.
6. **Delete** it from the Hierarchy.

### 5. The Jam Pin Prefab
1. Right-click in the Hierarchy -> `2D Object` -> `Sprites` -> `Triangle` (or Square if Triangle isn't available). Name it `JamPinVisual`.
2. Scale it down: Set Transform **Scale** to X: `0.3`, Y: `0.3`.
3. Change its **Color** to Red.
4. Drag `JamPinVisual` into the `Prefabs` folder.
5. **Delete** it from the Hierarchy.

### 6. The Target Prefab
1. Right-click in the Hierarchy -> `2D Object` -> `Sprites` -> `Square`. Name it `TargetVisual`.
2. Scale it down: Set Transform **Scale** to X: `0.4`, Y: `0.4`.
3. Change its **Color** to Yellow (Gold).
4. Drag `TargetVisual` into the `Prefabs` folder.
5. **Delete** it from the Hierarchy.

---

## Part 3: Setting Up the Managers

### 1. The Game Manager
1. Right-click in the Hierarchy -> `Create Empty`. Name it `GameManager`.
2. With `GameManager` selected, go to the Inspector -> `Add Component` -> search for `CRGameManager`.
3. Click `Add Component` again -> search for `CRViewManager`.

### 2. Linking Prefabs to the View Manager
1. Select `GameManager`. Look at the `CRViewManager` script in the Inspector.
2. You will see empty slots for Prefabs. Drag the corresponding prefabs from your `Prefabs` folder into these slots:
   * Track Prefab -> `TrackVisual`
   * Coil Prefab -> `CoilVisual`
   * Gate Prefab -> `GateVisual`
   * Jam Pin Prefab -> `JamPinVisual`
   * Target Prefab -> `TargetVisual`
   * Ghost Coil Prefab -> `GhostCoilVisual`

### 3. The Input Manager
1. In the Hierarchy, select the `Main Camera`.
2. In the Inspector, click `Add Component` -> search for `CRInputManager`.
3. Ensure the Camera's **Projection** is set to `Orthographic` (this is default in 2D projects).

---

## Part 4: Setting up the UI

We need screens for Winning, Losing (Wound Down), and simple messages (Overshoot, Jammed).

### 1. Create the Canvas
1. Right-click in Hierarchy -> `UI` -> `Canvas`. Name it `UICanvas`.
2. Right-click `UICanvas` -> `UI` -> `Event System` (if it wasn't created automatically).

### 2. UI Text elements (Requires TextMeshPro)
*Unity might ask you to "Import TMP Essentials" when you create your first text. Click "Import TMP Essentials" when the pop-up appears.*

1. Right-click `UICanvas` -> `UI` -> `Text - TextMeshPro`. Name it `LivesText`.
   * Position it at the top-left of the screen.
   * Type "Lives: 3" in the Text Input box.
2. Right-click `UICanvas` -> `UI` -> `Text - TextMeshPro`. Name it `LevelText`.
   * Position it at the top-center.
   * Type "Level 01".

### 3. The Fail Message Overlay
1. Right-click `UICanvas` -> `UI` -> `Panel`. Name it `FailMessageContainer`.
   * In the Inspector, change its color to Red and lower the Alpha so it's transparent.
   * Shrink it down to a small rectangle in the middle of the screen.
2. Right-click `FailMessageContainer` -> `UI` -> `Text - TextMeshPro`. Name it `FailText`.
   * Center the text alignment. Type "MESSAGE".
3. **Turn off** the `FailMessageContainer` by unchecking the small checkbox next to its name at the very top of the Inspector. (It should start hidden).

### 4. The Win Panel
1. Right-click `UICanvas` -> `UI` -> `Panel`. Name it `WinPanel`.
   * Change its color to Green.
2. Right-click `WinPanel` -> `UI` -> `Text - TextMeshPro`. Type "YOU WIN!". Center it.
3. **Turn off** the `WinPanel` in the Inspector.

### 5. The Wound Down Panel (Game Over)
1. Right-click `UICanvas` -> `UI` -> `Panel`. Name it `WoundDownPanel`.
   * Change its color to Black.
2. Right-click `WoundDownPanel` -> `UI` -> `Text - TextMeshPro`. Type "WOUND DOWN\nOut of lives". Center it.
3. Right-click `WoundDownPanel` -> `UI` -> `Button - TextMeshPro`. Name it `RetryButton`.
   * Position it below the text.
   * Change its text to "RETRY".
4. **Turn off** the `WoundDownPanel` in the Inspector.

### 6. Linking the UI to Code
1. Select `UICanvas` in the Hierarchy.
2. Click `Add Component` -> search for `CRUIManager`.
3. Drag the UI elements you just made into the corresponding slots in the Inspector:
   * Win Panel -> `WinPanel`
   * Wound Down Panel -> `WoundDownPanel`
   * Level Text -> `LevelText`
   * Lives Text -> `LivesText`
   * Fail Message Container -> `FailMessageContainer`
   * Fail Reason Text -> `FailText`
4. Now, select the `RetryButton` inside the `WoundDownPanel`.
5. In the Inspector, scroll down to the `Button` component, find the **On Click ()** list.
6. Click the `+` icon.
7. Drag the `UICanvas` from the Hierarchy into the empty object slot.
8. Click the dropdown that says `No Function` -> `CRUIManager` -> `OnRetryClicked()`.

---

## Part 5: Creating Your First Level

Now we create the actual 1D track data.

1. In the Project window, right-click the `Assets` folder -> `Create` -> `CoilRelease` -> `Level Data`.
2. Name the file `Level01_Data`.
3. Select `Level01_Data`. In the Inspector, you will define the tracks and gates.

### Setting up a Coil (The Track)
1. Expand the **Coils (Tracks)** list and click the `+` icon to add an element.
2. Fill it out:
   * **Id**: `coil_1`
   * **Track Length**: `10`
   * **Start Position**: `0`
   * **Target Position**: `8`
3. Expand **Jam Pins**. Click `+`. Type `4`. (This places a red jam pin at position 4).

### Setting up Gates
1. Expand the **Gates** list and click the `+` icon to add a gate.
2. Fill it out:
   * **Id**: `gate_a`
   * **Primary Coil Id**: `coil_1`
   * **Reach Lo**: `0`
   * **Reach Hi**: `2` (You can only click this gate when the coil is between positions 0 and 2).
3. Expand **Effects**. Click `+`.
   * **Coil Id**: `coil_1`
   * **Step**: `5` (Clicking this gate moves the coil forward 5 spaces).

### Validating the Level
1. Look at the very top of your Unity Editor screen. Click the **CR** menu -> `Validate All Levels`.
2. Look at the Unity **Console** window (Window -> General -> Console). It should print a message saying your level is Solvable!

---

## Part 6: Play!

1. Select your `GameManager` in the Hierarchy.
2. Drag `Level01_Data` from your Project window into the **Current Level Data** slot on the `CRGameManager` script.
3. Press the **Play** button at the top of Unity!

**How to interact:**
* Look at the green square (Gate).
* Click and **Hold** the gate. You will see a transparent ghost coil appear at position 5 (because the step is +5).
* Hold for at least 0.35 seconds, then let go of the mouse to commit the move.
* Note: Because you set a Jam Pin at position 4, moving from 0 to 5 means the coil will travel *through* position 4. The game will display "JAMMED!" and you will lose a life!
* Edit the level data to make a winnable puzzle, Validate it, and test your logic!