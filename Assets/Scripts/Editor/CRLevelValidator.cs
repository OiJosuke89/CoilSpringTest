using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class CRLevelValidator
{
    public static bool ValidateLevel(CRLevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("Validation Failed: LevelData is null.");
            return false;
        }

        bool isValid = true;
        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
        HashSet<string> coilIds = new HashSet<string>();

        // 1. Check bounds and overlaps
        
        // Coils
        foreach (var coil in levelData.coils)
        {
            if (string.IsNullOrEmpty(coil.id))
            {
                Debug.LogError("Validation Failed: A coil is missing an ID.");
                isValid = false;
            }
            if (!coilIds.Add(coil.id))
            {
                Debug.LogError($"Validation Failed: Duplicate Coil ID found: {coil.id}");
                isValid = false;
            }
            if (coil.unwindDirection == Vector2Int.zero)
            {
                Debug.LogError($"Validation Failed: Coil {coil.id} has no unwind direction.");
                isValid = false;
            }
            if (coil.length <= 0)
            {
                Debug.LogError($"Validation Failed: Coil {coil.id} length must be > 0.");
                isValid = false;
            }
            if (!CheckBounds(levelData, coil.position))
            {
                Debug.LogError($"Validation Failed: Coil {coil.id} is out of grid bounds.");
                isValid = false;
            }
            if (!occupiedCells.Add(coil.position))
            {
                Debug.LogError($"Validation Failed: Position {coil.position} is already occupied by another element.");
                isValid = false;
            }
        }

        // Gates
        HashSet<string> gateIds = new HashSet<string>();
        foreach (var gate in levelData.gates)
        {
            if (string.IsNullOrEmpty(gate.id))
            {
                Debug.LogError("Validation Failed: A gate is missing an ID.");
                isValid = false;
            }
            if (!gateIds.Add(gate.id))
            {
                Debug.LogError($"Validation Failed: Duplicate Gate ID found: {gate.id}");
                isValid = false;
            }
            if (string.IsNullOrEmpty(gate.targetCoilId) || !coilIds.Contains(gate.targetCoilId))
            {
                Debug.LogError($"Validation Failed: Gate {gate.id} references invalid or missing Coil ID: {gate.targetCoilId}");
                isValid = false;
            }
            if (!CheckBounds(levelData, gate.position))
            {
                Debug.LogError($"Validation Failed: Gate {gate.id} is out of grid bounds.");
                isValid = false;
            }
            if (!occupiedCells.Add(gate.position))
            {
                Debug.LogError($"Validation Failed: Gate {gate.id} position {gate.position} is already occupied.");
                isValid = false;
            }
        }

        // Jam Pins
        foreach (var pin in levelData.jamPins)
        {
            if (!CheckBounds(levelData, pin.position))
            {
                Debug.LogError($"Validation Failed: Jam Pin is out of grid bounds.");
                isValid = false;
            }
            if (!occupiedCells.Add(pin.position))
            {
                Debug.LogError($"Validation Failed: Jam Pin position {pin.position} is already occupied.");
                isValid = false;
            }
        }

        // Targets (Not strictly obstacles, but check bounds and ID)
        foreach (var target in levelData.targets)
        {
            if (string.IsNullOrEmpty(target.targetCoilId) || !coilIds.Contains(target.targetCoilId))
            {
                Debug.LogError($"Validation Failed: Target references invalid or missing Coil ID: {target.targetCoilId}");
                isValid = false;
            }
            if (!CheckBounds(levelData, target.position))
            {
                Debug.LogError($"Validation Failed: Target for coil {target.targetCoilId} is out of grid bounds.");
                isValid = false;
            }
            // A target can theoretically overlap with an empty space that a coil unwinds into.
            // But usually shouldn't overlap with a hard obstacle at start.
            if (occupiedCells.Contains(target.position))
            {
                Debug.LogWarning($"Validation Warning: Target for coil {target.targetCoilId} is on an occupied starting cell.");
            }
        }

        if (isValid)
        {
            Debug.Log($"Validation Passed for LevelData! Grid: {levelData.gridWidth}x{levelData.gridHeight}. Coils: {levelData.coils.Count}. Gates: {levelData.gates.Count}.");
        }

        return isValid;
    }

    private static bool CheckBounds(CRLevelData level, Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < level.gridWidth && pos.y >= 0 && pos.y < level.gridHeight;
    }
}
