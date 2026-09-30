using UnityEngine;
using System.Collections.Generic;

public class CRViewManager : MonoBehaviour
{
    public static CRViewManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject coilPrefab;
    public GameObject gatePrefab;
    public GameObject jamPinPrefab;
    public GameObject targetPrefab;
    public GameObject coilSegmentPrefab; // Used to draw the unwound path
    
    [Header("Settings")]
    public float cellSize = 1.0f; // Visual size of each cell on screen
    public Vector2 gridOffset;    // To center the grid

    private Dictionary<string, GameObject> activeGateViews = new Dictionary<string, GameObject>();
    private Dictionary<string, CRCoilView> activeCoilViews = new Dictionary<string, CRCoilView>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetupViews(CRCoilSimulator simulator)
    {
        // Clear previous views (for restart/next level)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        activeGateViews.Clear();
        activeCoilViews.Clear();

        // Spawn targets
        foreach (var kvp in simulator.targets)
        {
            SpawnPrefabAtPosition(targetPrefab, kvp.Value);
        }

        // Spawn Jam Pins
        foreach (var pos in simulator.jamPins)
        {
            SpawnPrefabAtPosition(jamPinPrefab, pos);
        }

        // Spawn Coils
        foreach (var kvp in simulator.activeCoils)
        {
            string coilId = kvp.Key;
            CRRuntimeCoil rCoil = kvp.Value;
            
            GameObject coilObj = SpawnPrefabAtPosition(coilPrefab, rCoil.data.position);
            CRCoilView coilView = coilObj.AddComponent<CRCoilView>();
            coilView.Initialize(rCoil, coilSegmentPrefab, cellSize, gridOffset);
            activeCoilViews[coilId] = coilView;
        }

        // Spawn Gates
        foreach (var kvp in simulator.activeGates)
        {
            string gateId = kvp.Key;
            CRRuntimeGate rGate = kvp.Value;
            
            GameObject gateObj = SpawnPrefabAtPosition(gatePrefab, rGate.position);
            gateObj.name = "Gate_" + gateId;
            
            // Add a collider so we can click it
            if (gateObj.GetComponent<Collider2D>() == null)
            {
                var col = gateObj.AddComponent<BoxCollider2D>();
                col.size = new Vector2(cellSize, cellSize);
            }
            
            activeGateViews[gateId] = gateObj;
        }
    }

    public void UpdateViews(CRCoilSimulator simulator)
    {
        // Update Gates (remove if released)
        List<string> gatesToRemove = new List<string>();
        foreach (var kvp in activeGateViews)
        {
            string gateId = kvp.Key;
            if (simulator.activeGates.ContainsKey(gateId) && simulator.activeGates[gateId].isReleased)
            {
                Destroy(kvp.Value);
                gatesToRemove.Add(gateId);
            }
        }
        
        foreach (string id in gatesToRemove)
            activeGateViews.Remove(id);

        // Update Coils (draw segments)
        foreach (var kvp in activeCoilViews)
        {
            string coilId = kvp.Key;
            CRCoilView view = kvp.Value;
            if (simulator.activeCoils.ContainsKey(coilId))
            {
                view.UpdateSegments(simulator.activeCoils[coilId]);
            }
        }
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize + gridOffset.x, gridPos.y * cellSize + gridOffset.y, 0);
    }

    private GameObject SpawnPrefabAtPosition(GameObject prefab, Vector2Int gridPos)
    {
        if (prefab == null)
        {
            GameObject empty = new GameObject("EmptyPrefabHolder");
            empty.transform.position = GridToWorld(gridPos);
            empty.transform.SetParent(transform);
            return empty;
        }
        return Instantiate(prefab, GridToWorld(gridPos), Quaternion.identity, transform);
    }
}
