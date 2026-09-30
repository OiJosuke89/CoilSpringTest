using UnityEngine;
using System.Collections.Generic;

public class CRGameManager : MonoBehaviour
{
    public static CRGameManager Instance { get; private set; }

    public CRLevelData currentLevelData;
    public CRCoilSimulator simulator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (currentLevelData != null)
        {
            LoadLevel(currentLevelData);
        }
    }

    public void LoadLevel(CRLevelData levelData)
    {
        currentLevelData = levelData;
        simulator = new CRCoilSimulator();
        simulator.Initialize(currentLevelData);
        
        // Notify views to setup
        CRViewManager.Instance?.SetupViews(simulator);
    }

    public void ReleaseGate(string gateId)
    {
        if (simulator.TryReleaseGate(gateId))
        {
            // Notify views of change
            CRViewManager.Instance?.UpdateViews(simulator);
            
            if (simulator.CurrentState == CRGameState.Win)
            {
                CRUIManager.Instance?.ShowWinScreen();
            }
            else if (simulator.CurrentState == CRGameState.Lose)
            {
                CRUIManager.Instance?.ShowLoseScreen();
            }
        }
    }
}
