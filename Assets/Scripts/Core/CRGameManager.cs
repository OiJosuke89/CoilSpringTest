using UnityEngine;
using System.Collections;

public class CRGameManager : MonoBehaviour
{
    public static CRGameManager Instance { get; private set; }

    public CRLevelData currentLevelData;
    public CRCoilSimulator simulator;

    [Header("Lives")]
    public int startingLives = 3;
    public int currentLives { get; private set; }

    [Header("Timings")]
    public float failResetDelay = 1.2f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentLives = startingLives;

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
        
        CRViewManager.Instance?.SetupViews(simulator);
        CRUIManager.Instance?.UpdateLivesDisplay(currentLives);
    }

    public void ReleaseGate(string gateId)
    {
        if (simulator.TryReleaseGate(gateId))
        {
            CRViewManager.Instance?.UpdateViews(simulator);
            
            if (simulator.CurrentState == CRGameState.Win)
            {
                CRUIManager.Instance?.ShowWinScreen();
            }
            else if (simulator.CurrentState == CRGameState.Resetting)
            {
                HandleFail();
            }
        }
    }

    private void HandleFail()
    {
        currentLives--;
        CRUIManager.Instance?.UpdateLivesDisplay(currentLives);

        CRUIManager.Instance?.ShowFailMessage(simulator.LastFailReason);

        if (currentLives <= 0)
        {
            // Out of lives completely -> Wound Down
            CRUIManager.Instance?.ShowWoundDownScreen();
        }
        else
        {
            // Reset level after delay
            StartCoroutine(ResetLevelRoutine());
        }
    }

    private IEnumerator ResetLevelRoutine()
    {
        yield return new WaitForSeconds(failResetDelay);
        CRUIManager.Instance?.HideFailMessage();
        LoadLevel(currentLevelData); // Reload same level
    }

    public void RetryLevel()
    {
        currentLives = startingLives;
        LoadLevel(currentLevelData);
    }
}