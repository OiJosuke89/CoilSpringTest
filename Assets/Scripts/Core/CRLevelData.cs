using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCRLevelData", menuName = "CoilRelease/Level Data")]
public class CRLevelData : ScriptableObject
{
    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 15;

    [Header("Level Elements")]
    public List<CRCoilData> coils = new List<CRCoilData>();
    public List<CRGateData> gates = new List<CRGateData>();
    public List<CRJamPinData> jamPins = new List<CRJamPinData>();
    public List<CRTargetData> targets = new List<CRTargetData>();
}

[System.Serializable]
public class CRCoilData
{
    public string id;
    public Vector2Int position;
    public Vector2Int unwindDirection; // e.g. (1,0) for right
    public int length;                 // Maximum length it can unwind
    public Color color = Color.white;
}

[System.Serializable]
public class CRGateData
{
    public string id;
    public string targetCoilId;
    public Vector2Int position;
}

[System.Serializable]
public class CRJamPinData
{
    public Vector2Int position;
}

[System.Serializable]
public class CRTargetData
{
    public string targetCoilId;
    public Vector2Int position;
}