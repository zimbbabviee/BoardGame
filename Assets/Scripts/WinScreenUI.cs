using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    public static WinScreenUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI placeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button leaderboardButton;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string leaderboardSceneName = "Leaderboard";

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

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        if (leaderboardButton != null)
        {
            leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        }
    }

    public void ShowWinScreen(string playerName, float gameTime, int totalMoves, int place)
    {
        if (winPanel == null) return;

        GameInfoUI gameInfoUI = FindObjectOfType<GameInfoUI>();
        if (gameInfoUI != null)
        {
            gameInfoUI.StopTimer();
        }

        Time.timeScale = 0f;

        winPanel.SetActive(true);

        int placePoints = LeaderboardUI.CalculatePlacePoints(place);
        int movesPoints = LeaderboardUI.CalculateMovesPoints(totalMoves);
        int timePoints = LeaderboardUI.CalculateTimePoints(gameTime);
        int totalScore = placePoints + movesPoints + timePoints;

        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00} (+{timePoints})";
        }

        if (movesText != null)
        {
            movesText.text = $"Moves: {totalMoves} (+{movesPoints})";
        }

        if (placeText != null)
        {
            string placeStr = GetPlaceString(place);
            placeText.text = $"Place: {placeStr} (+{placePoints})";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Total Score: {totalScore}";
        }

        SaveResult(playerName, gameTime, totalMoves, place);
    }

    private string GetPlaceString(int place)
    {
        switch (place)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{place}th";
        }
    }

    private void SaveResult(string playerName, float gameTime, int totalMoves, int place)
    {
        PlayerPrefs.SetString("LastWinner", playerName);
        PlayerPrefs.SetFloat("LastTime", gameTime);
        PlayerPrefs.SetInt("LastMoves", totalMoves);
        PlayerPrefs.SetInt("LastPlace", place);
        PlayerPrefs.Save();

        LeaderboardUI leaderboard = LeaderboardUI.Instance;
        if (leaderboard == null)
        {
            leaderboard = FindObjectOfType<LeaderboardUI>();
        }

        if (leaderboard != null)
        {
            leaderboard.AddEntry(playerName, gameTime, totalMoves, place);
        }
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnLeaderboardClicked()
    {
        LeaderboardUI leaderboard = LeaderboardUI.Instance;

        if (leaderboard == null)
        {
            leaderboard = FindObjectOfType<LeaderboardUI>();
        }

        if (leaderboard != null)
        {
            leaderboard.Show();
        }
        else
        {
            Debug.LogWarning("LeaderboardUI not found on scene!");
        }
    }

    public void HideWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }
}
