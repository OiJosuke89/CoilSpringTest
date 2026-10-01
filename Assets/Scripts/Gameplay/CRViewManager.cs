using UnityEngine;
using System.Collections.Generic;

public class CRViewManager : MonoBehaviour
{
    public static CRViewManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject trackPrefab;
    public GameObject coilPrefab;
    public GameObject gatePrefab;
    public GameObject jamPinPrefab;
    public GameObject targetPrefab;
    public GameObject ghostCoilPrefab;

    [Header("Layout Settings")]
    public float stepSize = 1.0f;
    public float trackSpacingY = -2.0f;
    public Vector2 startOffset = new Vector2(-4, 4);

    private Dictionary<string, CRCoilView> coilViews = new Dictionary<string, CRCoilView>();
    private Dictionary<string, CRGateView> gateViews = new Dictionary<string, CRGateView>();

    private GameObject previewContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupViews(CRCoilSimulator simulator)
    {
        ClearViews();

        previewContainer = new GameObject("PreviewContainer");
        previewContainer.transform.SetParent(transform);

        int trackIndex = 0;
        foreach (var coilKvp in simulator.activeCoils)
        {
            var coilId = coilKvp.Key;
            var coilData = coilKvp.Value.data;

            float baseY = startOffset.y + trackIndex * trackSpacingY;

            // Spawn Track Background (Line)
            var trackObj = Instantiate(trackPrefab, transform);
            var lr = trackObj.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.SetPosition(0, new Vector3(startOffset.x, baseY, 0));
                lr.SetPosition(1, new Vector3(startOffset.x + coilData.trackLength * stepSize, baseY, 0));
            }

            // Spawn Target
            Vector3 targetPos = new Vector3(startOffset.x + coilData.targetPosition * stepSize, baseY, 0);
            Instantiate(targetPrefab, targetPos, Quaternion.identity, transform);

            // Spawn Jam Pins
            foreach (var pinPos in coilData.jamPins)
            {
                Vector3 jpPos = new Vector3(startOffset.x + pinPos * stepSize, baseY, 0);
                Instantiate(jamPinPrefab, jpPos, Quaternion.identity, transform);
            }

            // Spawn Coil
            Vector3 startPos = new Vector3(startOffset.x + coilData.startPosition * stepSize, baseY, 0);
            var coilObj = Instantiate(coilPrefab, startPos, Quaternion.identity, transform);
            var coilView = coilObj.GetComponent<CRCoilView>();
            if (coilView == null) coilView = coilObj.AddComponent<CRCoilView>();
            coilView.Initialize(coilId, coilData.color);
            coilViews[coilId] = coilView;

            trackIndex++;
        }

        // Spawn Gates
        // We'll place gates visually near their primary coil's reach range
        int gateIndex = 0;
        foreach (var gateKvp in simulator.activeGates)
        {
            var gateId = gateKvp.Key;
            var gateData = gateKvp.Value.data;

            // Find track index of primary coil to align visually
            int primaryTrackIdx = 0;
            int idx = 0;
            foreach (var c in simulator.activeCoils.Keys)
            {
                if (c == gateData.primaryCoilId) primaryTrackIdx = idx;
                idx++;
            }

            float baseY = startOffset.y + primaryTrackIdx * trackSpacingY;

            // Position gate somewhat below the track, in the middle of its reach window
            float avgReach = (gateData.reachLo + gateData.reachHi) / 2.0f;
            Vector3 gatePos = new Vector3(startOffset.x + avgReach * stepSize, baseY - 0.7f, 0);

            // Adjust to avoid overlap if multiple gates on same primary coil (simple layout for mockup)
            gatePos.x += (gateIndex % 2 == 0 ? 0.2f : -0.2f);
            gatePos.y -= (gateIndex % 3) * 0.3f;

            var gateObj = Instantiate(gatePrefab, gatePos, Quaternion.identity, transform);
            var gateView = gateObj.GetComponent<CRGateView>();
            if (gateView == null) gateView = gateObj.AddComponent<CRGateView>();

            gateView.Initialize(gateId, gateData);
            gateViews[gateId] = gateView;

            gateIndex++;
        }

        UpdateViews(simulator);
    }

    public void UpdateViews(CRCoilSimulator simulator)
    {
        // Update Coil positions
        foreach (var kvp in simulator.activeCoils)
        {
            if (coilViews.TryGetValue(kvp.Key, out var view))
            {
                int trackIdx = GetTrackIndex(simulator, kvp.Key);
                float baseY = startOffset.y + trackIdx * trackSpacingY;
                Vector3 newPos = new Vector3(startOffset.x + kvp.Value.position * stepSize, baseY, 0);

                view.transform.position = newPos; // Or animate
            }
        }

        // Update Gate states (Spent/Chained/Latched)
        foreach (var kvp in simulator.activeGates)
        {
            if (gateViews.TryGetValue(kvp.Key, out var view))
            {
                bool canRelease = simulator.CanReleaseGate(kvp.Key);
                view.UpdateState(kvp.Value.isReleased, canRelease);
            }
        }
    }

    public void ShowPreview(string gateId)
    {
        ClearPreview();

        var sim = CRGameManager.Instance.simulator;
        if (!sim.CanReleaseGate(gateId)) return;

        var previewSim = sim.Clone();
        previewSim.TryReleaseGate(gateId);

        // Create ghost coils
        foreach (var kvp in previewSim.activeCoils)
        {
            int originalPos = sim.activeCoils[kvp.Key].position;
            int newPos = kvp.Value.position;

            if (originalPos != newPos)
            {
                int trackIdx = GetTrackIndex(sim, kvp.Key);
                float baseY = startOffset.y + trackIdx * trackSpacingY;
                Vector3 ghostPos = new Vector3(startOffset.x + newPos * stepSize, baseY, 0);

                var ghostObj = Instantiate(ghostCoilPrefab, ghostPos, Quaternion.identity, previewContainer.transform);
                var sr = ghostObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    var color = sim.activeCoils[kvp.Key].data.color;
                    color.a = 0.5f;
                    sr.color = color;
                }
            }
        }
    }

    public void ClearPreview()
    {
        if (previewContainer != null)
        {
            foreach (Transform child in previewContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void ClearViews()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        coilViews.Clear();
        gateViews.Clear();
    }

    public CRGateView GetGateView(string gateId)
    {
        if (gateViews.TryGetValue(gateId, out var view)) return view;
        return null;
    }

    private int GetTrackIndex(CRCoilSimulator sim, string coilId)
    {
        int idx = 0;
        foreach (var c in sim.activeCoils.Keys)
        {
            if (c == coilId) return idx;
            idx++;
        }
        return 0;
    }
}
