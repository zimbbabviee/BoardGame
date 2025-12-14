using UnityEngine;
using TMPro;

public class GameInfoUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI movesText;

    private float gameTime = 0f;
    private int totalMoves = 0;
    private bool isGameRunning = true;

    private void Update()
    {
        if (isGameRunning && Time.timeScale > 0)
        {
            gameTime += Time.deltaTime;
            UpdateTimeDisplay();
        }

        UpdateCurrentTurnDisplay();
    }

    private void UpdateCurrentTurnDisplay()
    {
        if (currentTurnText == null) return;

        if (TurnManager.Instance != null)
        {
            PlayerMovement currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                string playerName = currentPlayer.gameObject.name;

                NameScript nameScript = currentPlayer.GetComponent<NameScript>();
                if (nameScript != null)
                {
                    playerName = nameScript.GetName();
                }

                bool isBot = GameController.Instance != null && GameController.Instance.IsCurrentPlayerBot();
                string turnType = isBot ? "Bot" : "Player";

                currentTurnText.text = $"Current Turn: {playerName}";
            }
            else
            {
                currentTurnText.text = "Current Turn: ---";
            }
        }
        else
        {
            currentTurnText.text = "Current Turn: ---";
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        timeText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    public void AddMove()
    {
        totalMoves++;
        UpdateMovesDisplay();
    }

    private void UpdateMovesDisplay()
    {
        if (movesText == null) return;
        movesText.text = $"Moves: {totalMoves}";
    }

    public void StopTimer()
    {
        isGameRunning = false;
    }

    public void ResetGameInfo()
    {
        gameTime = 0f;
        totalMoves = 0;
        isGameRunning = true;
        UpdateTimeDisplay();
        UpdateMovesDisplay();
    }

    public int GetTotalMoves()
    {
        return totalMoves;
    }

    public float GetGameTime()
    {
        return gameTime;
    }
}
