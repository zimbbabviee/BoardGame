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
    [SerializeField] private GameObject entryPrefab;
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

    public void AddEntry(string playerName, float time, int moves)
    {
        List<LeaderboardEntry> entries = LoadLeaderboard();

        entries.Add(new LeaderboardEntry
        {
            playerName = playerName,
            time = time,
            moves = moves
        });

        entries.Sort((a, b) => a.time.CompareTo(b.time));

        if (entries.Count > maxEntries)
        {
            entries.RemoveRange(maxEntries, entries.Count - maxEntries);
        }

        SaveLeaderboard(entries);
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
        if (entryPrefab == null || entriesContainer == null) return;

        GameObject entryObj = Instantiate(entryPrefab, entriesContainer);

        TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 1)
        {
            int minutes = Mathf.FloorToInt(entry.time / 60f);
            int seconds = Mathf.FloorToInt(entry.time % 60f);
            texts[0].text = $"{rank}. {entry.playerName} - {minutes:00}:{seconds:00} - {entry.moves} moves";
        }
    }

    private void CreateEmptyMessage()
    {
        if (entryPrefab == null || entriesContainer == null) return;

        GameObject entryObj = Instantiate(entryPrefab, entriesContainer);
        TextMeshProUGUI text = entryObj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = "No records yet!";
        }
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
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries;
}
