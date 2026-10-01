using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCRLevelData", menuName = "CoilRelease/Level Data")]
public class CRLevelData : ScriptableObject
{
    [Header("Level Settings")]
    public int levelIndex = 1;
    public int minGates = 0; // Calculated by validator

    [Header("Coils (Tracks)")]
    public List<CRCoilData> coils = new List<CRCoilData>();

    [Header("Gates")]
    public List<CRGateData> gates = new List<CRGateData>();
}

[System.Serializable]
public class CRCoilData
{
    public string id;
    public int trackLength; // H
    public int startPosition = 0;
    public int targetPosition;
    public List<int> jamPins = new List<int>();
    public Color color = Color.white;
}

[System.Serializable]
public class CRGateEffect
{
    public string coilId;
    public int step;
}

[System.Serializable]
public class CRGateData
{
    public string id;
    public string primaryCoilId;
    public int reachLo;
    public int reachHi;
    public List<CRGateEffect> effects = new List<CRGateEffect>();
    public Color color = Color.gray;
}
