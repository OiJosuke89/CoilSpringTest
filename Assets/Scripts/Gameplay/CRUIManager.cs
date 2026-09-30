using UnityEngine;
using UnityEngine.UI;

public class CRUIManager : MonoBehaviour
{
    public static CRUIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    
    [Header("Top Bar")]
    public Text levelText;
    public Button pauseButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }

        HideAllPanels();
    }

    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }

    public void TogglePause()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }
    }

    private void HideAllPanels()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
