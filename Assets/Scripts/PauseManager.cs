using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("GameObjects to Hide")]
    [SerializeField] private GameObject gameUIElements;

    private CanvasGroup pausePanelCanvasGroup;
    private CanvasGroup settingsPanelCanvasGroup;
    private bool isPaused = false;

    private void Awake()
    {
        if (pausePanel != null)
        {
            pausePanelCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
            if (pausePanelCanvasGroup == null)
            {
                pausePanelCanvasGroup = pausePanel.AddComponent<CanvasGroup>();
            }
            HidePanel(pausePanelCanvasGroup);
        }

        if (settingsPanel != null)
        {
            settingsPanelCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
            if (settingsPanelCanvasGroup == null)
            {
                settingsPanelCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
            }
            HidePanel(settingsPanelCanvasGroup);
        }
    }

    private void ShowPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void HidePanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame вызвана");
        isPaused = true;
        Time.timeScale = 0f;

        if (gameUIElements != null)
        {
            gameUIElements.SetActive(false);
            Debug.Log("gameUIElements скрыт");
        }

        ShowPanel(pausePanelCanvasGroup);
        Debug.Log($"Панель паузы показана. Alpha: {pausePanelCanvasGroup?.alpha}");

        if (pausePanel != null)
        {
            pausePanel.transform.SetAsLastSibling();
            Debug.Log($"PausePanel активен: {pausePanel.activeSelf}");
        }

        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        HidePanel(pausePanelCanvasGroup);
        HidePanel(settingsPanelCanvasGroup);

        if (gameUIElements != null)
        {
            gameUIElements.SetActive(true);
        }

        AudioListener.pause = false;
    }

    public void OpenSettingsFromPause()
    {
        HidePanel(pausePanelCanvasGroup);

        ShowPanel(settingsPanelCanvasGroup);
    }

    public void CloseSettingsToPause()
    {
        HidePanel(settingsPanelCanvasGroup);

        ShowPanel(pausePanelCanvasGroup);
    }

    public void OpenLeaderboard()
    {
        Debug.Log("Leaderboard opened from pause menu");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; 
        AudioListener.pause = false;

        SceneManager.LoadScene("SampleScene");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
