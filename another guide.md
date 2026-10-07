# Coil Release Asset Integration Guide

This guide explains how to transition from the basic shapes and primitive figures mockup to integrating the final 2D game assets based on `assets.png`, `gameplay assets.png`, and `Sample.png`.

## Overview of Visual Assets

The visual target is a 2D isometric-style or stylized top-down look representing tracks, coils (sliders), and gates (pins). Based on the provided sprite sheets:

1.  **Background / Track Base:**
    -   The main track acts as a slot where the coils run.
    -   Use the large textured metallic or colored bars from `assets.png` for the background track. Slice these horizontally.
2.  **Coils (Sliders):**
    -   The main movable elements. There are variations in `assets.png` (blue, orange, etc.) with arrows indicating movement direction or connectivity.
    -   The sprites have an inherent 3D/depth look. Maintain their aspect ratio.
3.  **Gates (Pins):**
    -   Gates are represented by circular/metallic nodes or pins in the track.
    -   Use the circular sprites from the sprite sheet. When a gate is "Released", it could be visually greyed out, removed, or pushed into the background.
4.  **Target Areas / Jam Pins:**
    -   The target zones can be indicated using the highlighted or colored segmented blocks.
    -   Jam pins can be represented using red or warning-colored markers.

## Slicing the Sprites

1.  Import `assets.png` and `gameplay assets.png` into `Assets/Art/Sprites/`.
2.  In the Unity Inspector, set the **Texture Type** to `Sprite (2D and UI)`.
3.  Set **Sprite Mode** to `Multiple`.
4.  Click **Sprite Editor**.
5.  Use **Automatic Slicing** or manually draw rectangles around:
    -   Each individual slider (coil).
    -   Each pin (gate).
    -   The track backgrounds.
    -   The UI elements (lives, buttons, fail state overlays).
6.  Click **Apply**.

## Updating the Views

The mockup used simple `SpriteRenderer` with basic generated shapes. Now, we'll swap those out for the newly sliced assets.

### 1. Update CRCoilView.cs Prefab
- In your Coil Prefab, replace the placeholder square sprite with one of the colored slider sprites from the sprite sheet.
- **Sorting Order:** Ensure the Coil has a higher sorting order than the track.
- **Visual Offsets:** Because the new sprites might have shadows or a specific perspective, you may need to add a child `GameObject` for the actual visual graphic to offset it correctly without messing up the logical transform position.

### 2. Update CRGateView.cs Prefab
- Replace the gate graphic (e.g., the circle) with the metallic pin sprites.
- Add an extra visual state for the "Released" (spent) mode.
```csharp
// Example addition to CRGateView.cs
public Sprite activeSprite;
public Sprite releasedSprite;

public void UpdateView(bool isReleased, bool canRelease)
{
    // existing logic...
    spriteRenderer.sprite = isReleased ? releasedSprite : activeSprite;
    // ...
}
```

### 3. Track and Background Visualization
Currently, the track is implicit in the 1D logic. To make it visually match `Sample.png`:
- Create a new script `CRTrackView.cs` or handle it in `CRViewManager`.
- Instantiate track background sprites that stretch along the length of each coil's track (from `0` to `coil.data.trackLength`).
- Place these at a low sorting layer (e.g., `Background`).

### 4. UI Elements Integration
- Replace the basic TextMeshPro UI for "Lives" with the heart or battery icons found in the assets.
- Use the stylized panels from `gameplay assets.png` as the background for the Game Over / Level Complete screens.

## Adjusting Grid and Scale
The basic shapes used 1 Unity unit per position. The new assets might be 100 pixels per unit.
- You can adjust the `PositionOffset` in `CRViewManager.cs` to spread the tracks and positions further apart to match the visual size of the new slider sprites.

```csharp
// In CRViewManager.cs
// Tweak these values to match your new sprite dimensions
public float visualSpacingX = 1.5f;
public float visualSpacingY = 2.0f;
```

This ensures the logic remains purely 1-dimensional and data-driven, while the visuals accurately reflect the provided art style!