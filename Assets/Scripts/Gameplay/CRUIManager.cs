using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CRUIManager : MonoBehaviour
{
    public static CRUIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject woundDownPanel; // Replaces simple LosePanel
    public GameObject pausePanel;
    
    [Header("HUD Elements")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI livesText;

    [Header("Fail Messages")]
    public GameObject failMessageContainer;
    public TextMeshProUGUI failReasonText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowWoundDownScreen()
    {
        if (woundDownPanel != null) woundDownPanel.SetActive(true);
    }

    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null) livesText.text = $"Lives: {lives}";
    }

    public void ShowFailMessage(CRFailReason reason)
    {
        if (failMessageContainer != null) failMessageContainer.SetActive(true);
        if (failReasonText != null)
        {
            switch (reason)
            {
                case CRFailReason.Overshoot: failReasonText.text = "OVERSHOOT!"; break;
                case CRFailReason.Jam: failReasonText.text = "JAMMED!"; break;
                case CRFailReason.Stuck: failReasonText.text = "STUCK!\nNo winning moves left."; break;
                default: failReasonText.text = "FAILED!"; break;
            }
        }
    }

    public void HideFailMessage()
    {
        if (failMessageContainer != null) failMessageContainer.SetActive(false);
    }

    // UI Buttons
    public void OnRetryClicked()
    {
        if (woundDownPanel != null) woundDownPanel.SetActive(false);
        CRGameManager.Instance?.RetryLevel();
    }
}