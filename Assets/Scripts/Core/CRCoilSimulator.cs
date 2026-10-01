using System.Collections.Generic;
using UnityEngine;

public enum CRGameState
{
    Playing,
    Win,
    Lose, // Out of lives
    Resetting // Temporary state while showing fail animation before resetting
}

public enum CRFailReason
{
    None,
    Overshoot,
    Jam,
    Stuck
}

public class CRCoilSimulator
{
    public CRLevelData currentLevel;
    
    public Dictionary<string, CRRuntimeCoil> activeCoils;
    public Dictionary<string, CRRuntimeGate> activeGates;
    
    public CRGameState CurrentState { get; private set; }
    public CRFailReason LastFailReason { get; private set; }
    
    public void Initialize(CRLevelData levelData)
    {
        currentLevel = levelData;
        activeCoils = new Dictionary<string, CRRuntimeCoil>();
        activeGates = new Dictionary<string, CRRuntimeGate>();
        
        CurrentState = CRGameState.Playing;
        LastFailReason = CRFailReason.None;
        
        foreach (var coilData in levelData.coils)
        {
            activeCoils[coilData.id] = new CRRuntimeCoil(coilData);
        }
        
        foreach (var gateData in levelData.gates)
        {
            activeGates[gateData.id] = new CRRuntimeGate(gateData);
        }
    }

    // Creates a deep copy of the simulator state, useful for validator and preview
    public CRCoilSimulator Clone()
    {
        var clone = new CRCoilSimulator();
        clone.currentLevel = currentLevel;
        clone.CurrentState = CurrentState;
        clone.LastFailReason = LastFailReason;
        
        clone.activeCoils = new Dictionary<string, CRRuntimeCoil>();
        foreach (var kvp in activeCoils)
        {
            clone.activeCoils[kvp.Key] = new CRRuntimeCoil(kvp.Value.data)
            {
                position = kvp.Value.position,
                reachedTarget = kvp.Value.reachedTarget
            };
        }
        
        clone.activeGates = new Dictionary<string, CRRuntimeGate>();
        foreach (var kvp in activeGates)
        {
            clone.activeGates[kvp.Key] = new CRRuntimeGate(kvp.Value.data)
            {
                isReleased = kvp.Value.isReleased
            };
        }

        return clone;
    }
    
    public bool CanReleaseGate(string gateId)
    {
        if (CurrentState != CRGameState.Playing) return false;
        if (!activeGates.ContainsKey(gateId)) return false;
        
        var gate = activeGates[gateId];
        if (gate.isReleased) return false;
        
        if (!activeCoils.ContainsKey(gate.data.primaryCoilId)) return false;
        var primaryCoil = activeCoils[gate.data.primaryCoilId];

        return primaryCoil.position >= gate.data.reachLo && primaryCoil.position <= gate.data.reachHi;
    }

    public bool TryReleaseGate(string gateId)
    {
        if (!CanReleaseGate(gateId)) return false;

        var gate = activeGates[gateId];

        // 1. Resolve effects
        Dictionary<string, int> newPositions = new Dictionary<string, int>();
        bool hasOvershoot = false;
        bool hasJam = false;

        foreach (var effect in gate.data.effects)
        {
            if (!activeCoils.ContainsKey(effect.coilId)) continue;
            var coil = activeCoils[effect.coilId];
            
            int newPos = coil.position + effect.step;
            newPositions[effect.coilId] = newPos;
            
            // Check Overshoot
            if (newPos < 0 || newPos > coil.data.trackLength)
            {
                hasOvershoot = true;
            }
            
            // Check Jam (traveled positions)
            if (!hasOvershoot) // Optimization: if already overshoot, jam doesn't matter as much, or they happen together
            {
                int start = coil.position;
                int end = newPos;
                int stepDir = System.Math.Sign(effect.step);

                if (stepDir != 0)
                {
                    for (int p = start + stepDir; p != end + stepDir; p += stepDir)
                    {
                        if (coil.data.jamPins.Contains(p))
                        {
                            hasJam = true;
                            break;
                        }
                    }
                }
            }
        }

        // 2. Commit or Fail
        if (hasOvershoot || hasJam)
        {
            // Gate becomes spent even on fail? GDD says "If any effect fails, the level plays the failing move... and then the failure event fires... No positions change on a failing move."
            // Wait, GDD: "Each Released gate stays spent...". Let's spend it.
            // Actually: "If any effect fails... no positions change". But is the gate spent? "If every effect passes, all positions are updated and the gate becomes Released (spent)."
            // So if it fails, it does NOT become Released. But the level resets anyway! So it doesn't matter for the current attempt.

            CurrentState = CRGameState.Resetting;
            LastFailReason = hasJam ? CRFailReason.Jam : CRFailReason.Overshoot;
            return true;
        }

        // Commit
        gate.isReleased = true;
        foreach (var kvp in newPositions)
        {
            activeCoils[kvp.Key].position = kvp.Value;
        }

        CheckWinLossCondition();
        return true;
    }

    private void CheckWinLossCondition()
    {
        bool allCoilsReached = true;
        
        foreach (var coil in activeCoils.Values)
        {
            coil.reachedTarget = (coil.position == coil.data.targetPosition);
            if (!coil.reachedTarget)
            {
                allCoilsReached = false;
            }
        }
        
        if (allCoilsReached)
        {
            CurrentState = CRGameState.Win;
            return;
        }
        
        // Check Dead-end (Stuck)
        // Optimization: For the actual game we need a deep search.
        // For now, if no sequence can win, it's a dead end.
        if (IsDeadEnd())
        {
            CurrentState = CRGameState.Resetting;
            LastFailReason = CRFailReason.Stuck;
        }
    }

    // A simple recursive check to see if ANY valid sequence of remaining gates leads to a win
    private bool IsDeadEnd()
    {
        // Simple heuristic for now: if no unreleased gates exist, we are stuck (since we aren't winning)
        bool hasUnreleased = false;
        foreach(var g in activeGates.Values) {
            if(!g.isReleased) { hasUnreleased = true; break; }
        }
        if(!hasUnreleased) return true;

        // Full DFS check is required by GDD for Dead-end check.
        return !CanWinFromCurrentState(this, new HashSet<string>());
    }

    private static bool CanWinFromCurrentState(CRCoilSimulator sim, HashSet<string> visitedStates)
    {
        // State representation for memoization
        string stateKey = sim.GetStateKey();
        if (visitedStates.Contains(stateKey)) return false;
        visitedStates.Add(stateKey);

        bool allWon = true;
        foreach (var coil in sim.activeCoils.Values)
        {
            if (coil.position != coil.data.targetPosition) { allWon = false; break; }
        }
        if (allWon) return true;

        foreach (var gate in sim.activeGates.Values)
        {
            if (!gate.isReleased && sim.CanReleaseGate(gate.data.id))
            {
                var nextSim = sim.Clone();
                nextSim.TryReleaseGate(gate.data.id);

                if (nextSim.CurrentState == CRGameState.Win) return true;
                if (nextSim.CurrentState == CRGameState.Playing) // Not failed
                {
                    if (CanWinFromCurrentState(nextSim, visitedStates)) return true;
                }
            }
        }

        return false;
    }

    private string GetStateKey()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var c in activeCoils) sb.Append($"{c.Key}:{c.Value.position},");
        foreach (var g in activeGates) sb.Append(g.Value.isReleased ? "1" : "0");
        return sb.ToString();
    }
}

public class CRRuntimeCoil
{
    public CRCoilData data;
    public int position;
    public bool reachedTarget;
    
    public CRRuntimeCoil(CRCoilData coilData)
    {
        data = coilData;
        position = coilData.startPosition;
        reachedTarget = (position == coilData.targetPosition);
    }
}

public class CRRuntimeGate
{
    public CRGateData data;
    public bool isReleased;
    
    public CRRuntimeGate(CRGateData gateData)
    {
        data = gateData;
        isReleased = false;
    }
}