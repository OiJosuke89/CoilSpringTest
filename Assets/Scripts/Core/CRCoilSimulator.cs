using System.Collections.Generic;
using UnityEngine;

public enum CRGameState
{
    Playing,
    Win,
    Lose
}

public class CRCoilSimulator
{
    private CRLevelData currentLevel;
    
    public HashSet<Vector2Int> occupiedCells;
    public HashSet<Vector2Int> jamPins;
    
    public Dictionary<string, CRRuntimeCoil> activeCoils;
    public Dictionary<string, CRRuntimeGate> activeGates;
    public Dictionary<string, Vector2Int> targets;
    
    public CRGameState CurrentState { get; private set; }
    
    public void Initialize(CRLevelData levelData)
    {
        currentLevel = levelData;
        occupiedCells = new HashSet<Vector2Int>();
        jamPins = new HashSet<Vector2Int>();
        activeCoils = new Dictionary<string, CRRuntimeCoil>();
        activeGates = new Dictionary<string, CRRuntimeGate>();
        targets = new Dictionary<string, Vector2Int>();
        
        CurrentState = CRGameState.Playing;
        
        foreach (var pin in levelData.jamPins)
        {
            jamPins.Add(pin.position);
            occupiedCells.Add(pin.position);
        }
        
        foreach (var coil in levelData.coils)
        {
            activeCoils[coil.id] = new CRRuntimeCoil(coil);
            occupiedCells.Add(coil.position);
        }
        
        foreach (var gate in levelData.gates)
        {
            activeGates[gate.id] = new CRRuntimeGate(gate);
            occupiedCells.Add(gate.position);
        }
        
        foreach (var target in levelData.targets)
        {
            targets[target.targetCoilId] = target.position;
        }
    }
    
    public bool TryReleaseGate(string gateId)
    {
        if (CurrentState != CRGameState.Playing) return false;
        if (!activeGates.ContainsKey(gateId)) return false;
        
        var gate = activeGates[gateId];
        if (gate.isReleased) return false;
        
        // Release gate
        gate.isReleased = true;
        occupiedCells.Remove(gate.position);
        
        // Trigger associated coil
        if (activeCoils.TryGetValue(gate.targetCoilId, out var coil))
        {
            UnwindCoil(coil);
        }
        
        CheckWinLossCondition();
        return true;
    }

    public void UnwindCoil(CRRuntimeCoil coil)
    {
        // Unwinds up to its length, stopping at obstacles (occupied cells) or boundaries
        for (int i = 0; i < coil.data.length; i++)
        {
            Vector2Int nextPos = coil.currentHeadPosition + coil.data.unwindDirection;
            
            // Check grid bounds
            if (nextPos.x < 0 || nextPos.x >= currentLevel.gridWidth ||
                nextPos.y < 0 || nextPos.y >= currentLevel.gridHeight)
            {
                break; // Hit boundary
            }
            
            // Check obstacle
            if (occupiedCells.Contains(nextPos))
            {
                break; // Hit obstacle (other coil, gate, jam pin)
            }
            
            coil.unwoundPath.Add(nextPos);
            coil.currentHeadPosition = nextPos;
            occupiedCells.Add(nextPos);
            
            // Check if it reached its target
            if (targets.TryGetValue(coil.data.id, out var targetPos) && targetPos == nextPos)
            {
                coil.reachedTarget = true;
                break;
            }
        }
    }
    
    private void CheckWinLossCondition()
    {
        bool allCoilsReached = true;
        
        foreach (var coil in activeCoils.Values)
        {
            // If the coil has a target, check if it was reached
            if (targets.ContainsKey(coil.data.id) && !coil.reachedTarget)
            {
                allCoilsReached = false;
            }
        }
        
        if (allCoilsReached)
        {
            CurrentState = CRGameState.Win;
            return;
        }
        
        // If all gates released and not all targets met, it's a loss
        bool allGatesReleased = true;
        foreach (var gate in activeGates.Values)
        {
            if (!gate.isReleased)
            {
                allGatesReleased = false;
                break;
            }
        }
        
        if (allGatesReleased && !allCoilsReached)
        {
            CurrentState = CRGameState.Lose;
        }
    }
}

public class CRRuntimeCoil
{
    public CRCoilData data;
    public List<Vector2Int> unwoundPath;
    public Vector2Int currentHeadPosition;
    public bool reachedTarget;
    
    public CRRuntimeCoil(CRCoilData coilData)
    {
        data = coilData;
        unwoundPath = new List<Vector2Int> { coilData.position };
        currentHeadPosition = coilData.position;
        reachedTarget = false;
    }
}

public class CRRuntimeGate
{
    public CRGateData data;
    public string targetCoilId => data.targetCoilId;
    public Vector2Int position => data.position;
    public bool isReleased;
    
    public CRRuntimeGate(CRGateData gateData)
    {
        data = gateData;
        isReleased = false;
    }
}