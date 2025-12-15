using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class LeaderboardUI : MonoBehaviour
{
    public static LeaderboardUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private Button closeButton;

    [Header("Main Menu Elements (for SampleScene)")]
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject mainMenuHeader;

    [Header("Settings")]
    [SerializeField] private int maxEntries = 10;

    private const string LEADERBOARD_KEY = "Leaderboard";
    private Action onHideCallback;

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

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }

        SetupEntriesContainer();
    }

    private void SetupEntriesContainer()
    {
        if (entriesContainer == null) return;

        VerticalLayoutGroup layoutGroup = entriesContainer.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = entriesContainer.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.spacing = 5f;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter sizeFitter = entriesContainer.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null)
        {
            sizeFitter = entriesContainer.gameObject.AddComponent<ContentSizeFitter>();
        }
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform rectTransform = entriesContainer.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
        }
    }

    public void Show()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            leaderboardPanel.transform.SetAsLastSibling();
            RefreshLeaderboard();
        }
    }

    public void Hide()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }

        if (mainMenuButtons != null)
        {
            mainMenuButtons.SetActive(true);
        }

        if (mainMenuHeader != null)
        {
            mainMenuHeader.SetActive(true);
        }

        onHideCallback?.Invoke();
        onHideCallback = null;
    }

    public void SetOnHideCallback(Action callback)
    {
        onHideCallback = callback;
    }

    public void AddEntry(string playerName, float time, int moves, int place)
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();

        int placePoints = CalculatePlacePoints(place);
        int movesPoints = CalculateMovesPoints(moves);
        int timePoints = CalculateTimePoints(time);
        int totalScore = placePoints + movesPoints + timePoints;

        entries.Add(new LeaderboardEntry
        {
            playerName = playerName,
            time = time,
            moves = moves,
            place = place,
            placePoints = placePoints,
            movesPoints = movesPoints,
            timePoints = timePoints,
            totalScore = totalScore
        });

        entries.Sort((a, b) => b.totalScore.CompareTo(a.totalScore));

        if (entries.Count > maxEntries)
        {
            entries.RemoveRange(maxEntries, entries.Count - maxEntries);
        }

        SaveLeaderboard(entries);
    }

    public static int CalculateMovesPoints(int moves)
    {
        if (moves <= 3) return 10;
        int points = 10 - (moves - 3);
        return Mathf.Max(0, points);
    }

    public static int CalculatePlacePoints(int place)
    {
        switch (place)
        {
            case 1: return 3;
            case 2: return 2;
            case 3: return 1;
            default: return 0;
        }
    }

    public static int CalculateTimePoints(float time)
    {
        if (time <= 120f) return 3; 
        float extraTime = time - 120f;
        int penalty = Mathf.FloorToInt(extraTime / 30f);
        int points = 3 - penalty;
        return Mathf.Max(0, points);
    }

    public static int CalculateTotalScore(int moves, int place, float time)
    {
        return CalculateMovesPoints(moves) + CalculatePlacePoints(place) + CalculateTimePoints(time);
    }

    private void RefreshLeaderboard()
    {
        foreach (Transform child in entriesContainer)
        {
            Destroy(child.gameObject);
        }

        List<LeaderboardEntry> entries = LoadLeaderboard();

        for (int i = 0; i < entries.Count; i++)
        {
            CreateEntryUI(i + 1, entries[i]);
        }

        if (entries.Count == 0)
        {
            CreateEmptyMessage();
        }
    }

    private void CreateEntryUI(int rank, LeaderboardEntry entry)
    {
        if (entriesContainer == null) return;

        GameObject entryObj = new GameObject($"Entry_{rank}");
        entryObj.transform.SetParent(entriesContainer, false);

        RectTransform rectTransform = entryObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 35f);

        LayoutElement layoutElement = entryObj.AddComponent<LayoutElement>();
        layoutElement.minHeight = 35f;
        layoutElement.preferredHeight = 35f;
        layoutElement.flexibleWidth = 1f;

        TextMeshProUGUI text = entryObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18f;
        text.alignment = TextAlignmentOptions.Left;
        text.color = Color.white;

        int minutes = Mathf.FloorToInt(entry.time / 60f);
        int seconds = Mathf.FloorToInt(entry.time % 60f);
        string placeStr = GetPlaceString(entry.place);
        text.text = $"{rank}. {entry.playerName} - {minutes:00}:{seconds:00} - {entry.moves} moves - {placeStr} - Score: {entry.totalScore}";
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

    private void CreateEmptyMessage()
    {
        if (entriesContainer == null) return;

        GameObject entryObj = new GameObject("EmptyMessage");
        entryObj.transform.SetParent(entriesContainer, false);

        RectTransform rectTransform = entryObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 35f);

        LayoutElement layoutElement = entryObj.AddComponent<LayoutElement>();
        layoutElement.minHeight = 35f;
        layoutElement.preferredHeight = 35f;
        layoutElement.flexibleWidth = 1f;

        TextMeshProUGUI text = entryObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18f;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.text = "No records yet!";
    }

    private List<LeaderboardEntry> LoadLeaderboard()
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "");

        if (!string.IsNullOrEmpty(json))
        {
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            if (data != null && data.entries != null)
            {
                entries = data.entries;
            }
        }

        return entries;
    }

    private void SaveLeaderboard(List<LeaderboardEntry> entries)
    {
        LeaderboardData data = new LeaderboardData { entries = entries };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        PlayerPrefs.Save();
        RefreshLeaderboard();
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
    public int moves;
    public int place;
    public int placePoints;
    public int movesPoints;
    public int timePoints;
    public int totalScore;
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
}
