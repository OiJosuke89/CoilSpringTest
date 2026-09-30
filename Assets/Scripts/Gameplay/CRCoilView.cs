using UnityEngine;
using System.Collections.Generic;

public class CRCoilView : MonoBehaviour
{
    private CRRuntimeCoil runtimeCoil;
    private GameObject segmentPrefab;
    private float cellSize;
    private Vector2 gridOffset;

    private List<GameObject> activeSegments = new List<GameObject>();

    public void Initialize(CRRuntimeCoil coil, GameObject segmentPrefab, float cellSize, Vector2 gridOffset)
    {
        this.runtimeCoil = coil;
        this.segmentPrefab = segmentPrefab;
        this.cellSize = cellSize;
        this.gridOffset = gridOffset;

        // Set initial color based on data if SpriteRenderer exists
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = coil.data.color;
        }

        UpdateSegments(coil);
    }

    public void UpdateSegments(CRRuntimeCoil coil)
    {
        this.runtimeCoil = coil;

        // Clear existing segments if any logic changed (or just append)
        // For simplicity, we just rebuild if the count doesn't match
        // Note: unwoundPath includes the start pos (index 0). We draw segments for index 1 onwards.
        if (activeSegments.Count < runtimeCoil.unwoundPath.Count - 1)
        {
            for (int i = activeSegments.Count + 1; i < runtimeCoil.unwoundPath.Count; i++)
            {
                Vector2Int pos = runtimeCoil.unwoundPath[i];
                GameObject seg = SpawnSegment(pos);
                activeSegments.Add(seg);
            }
        }
    }

    private GameObject SpawnSegment(Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(gridPos.x * cellSize + gridOffset.x, gridPos.y * cellSize + gridOffset.y, 0);
        
        GameObject seg;
        if (segmentPrefab != null)
        {
            seg = Instantiate(segmentPrefab, worldPos, Quaternion.identity, transform);
        }
        else
        {
            seg = new GameObject("Segment");
            seg.transform.position = worldPos;
            seg.transform.SetParent(transform);
            
            // Default visual if no prefab
            var sr = seg.AddComponent<SpriteRenderer>();
            // Since we can't load standard assets without errors in text-only, we just leave it invisible 
            // or assume the user assigns a sprite to the prefab.
            sr.color = runtimeCoil.data.color; 
        }

        return seg;
    }
}
