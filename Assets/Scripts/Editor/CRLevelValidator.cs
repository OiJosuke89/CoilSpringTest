using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CRLevelValidator
{
    [MenuItem("CR / Validate All Levels")]
    public static void ValidateAllLevels()
    {
        string[] guids = AssetDatabase.FindAssets("t:CRLevelData");

        int winnableLevels = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CRLevelData levelData = AssetDatabase.LoadAssetAtPath<CRLevelData>(path);

            if (levelData != null)
            {
                if (ValidateLevel(levelData)) winnableLevels++;
            }
        }

        Debug.Log($"Validation Complete. {winnableLevels}/{guids.Length} levels are winnable.");
    }

    public static bool ValidateLevel(CRLevelData levelData)
    {
        CRCoilSimulator sim = new CRCoilSimulator();
        sim.Initialize(levelData);

        int minGates = int.MaxValue;
        bool isSolvable = BruteForceSimulate(sim, 0, ref minGates);

        if (isSolvable)
        {
            levelData.minGates = minGates;
            EditorUtility.SetDirty(levelData);
            Debug.Log($"Level {levelData.levelIndex} is Solvable. Min Gates: {minGates}");
        }
        else
        {
            Debug.LogError($"Level {levelData.levelIndex} is NOT Solvable!");
        }

        AssetDatabase.SaveAssets();

        return isSolvable;
    }

    private static bool BruteForceSimulate(CRCoilSimulator sim, int gatesUsed, ref int minGates)
    {
        if (sim.CurrentState == CRGameState.Win)
        {
            if (gatesUsed < minGates) minGates = gatesUsed;
            return true;
        }

        if (sim.CurrentState != CRGameState.Playing) return false;

        bool foundWin = false;

        foreach (var gate in sim.activeGates.Values)
        {
            if (!gate.isReleased && sim.CanReleaseGate(gate.data.id))
            {
                var nextSim = sim.Clone();
                if (nextSim.TryReleaseGate(gate.data.id))
                {
                    if (BruteForceSimulate(nextSim, gatesUsed + 1, ref minGates))
                    {
                        foundWin = true;
                    }
                }
            }
        }

        return foundWin;
    }
}