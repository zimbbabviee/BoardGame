using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Header("Players")]
    private List<PlayerMovement> players = new List<PlayerMovement>();
    private List<bool> playerIsBot = new List<bool>(); 
    private int currentPlayerIndex = 0;

    [Header("Turn Settings")]
    [SerializeField] private bool autoSwitchTurns = true; 
    [SerializeField] private float turnSwitchDelay = 2f; 
    private bool waitingForTurnSwitch = false;

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

    public void RegisterPlayer(PlayerMovement player, bool isBot = false)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            playerIsBot.Add(isBot);
            Debug.Log($"Player {player.name} registered (Bot: {isBot}). Total players: {players.Count}");

            if (players.Count == 1 && GameController.Instance != null)
            {
                GameController.Instance.SetCurrentPlayerWithoutAutoStart(player, isBot);
            }
        }
    }

    public PlayerMovement GetCurrentPlayer()
    {
        if (players.Count == 0) return null;
        return players[currentPlayerIndex];
    }

    public int GetPlayerCount()
    {
        return players.Count;
    }
    public void NextTurn()
    {
        if (players.Count <= 1)
        {
            Debug.Log("Only one player - no turn switch required");
            return;
        }

        if (waitingForTurnSwitch)
        {
            Debug.LogWarning("Turn switch already in progress!");
            return;
        }

        waitingForTurnSwitch = true;
        Invoke(nameof(SwitchToNextPlayer), turnSwitchDelay);
    }

    private void SwitchToNextPlayer()
    {
        int startIndex = currentPlayerIndex;
        int attempts = 0;

        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            attempts++;

            if (attempts > players.Count)
            {
                Debug.Log("All players have finished!");
                waitingForTurnSwitch = false;
                return;
            }
        }
        while (players[currentPlayerIndex].HasFinished());

        PlayerMovement nextPlayer = players[currentPlayerIndex];
        bool isBot = playerIsBot[currentPlayerIndex];

        if (GameController.Instance != null)
        {
            GameController.Instance.SwitchPlayer(nextPlayer, isBot);
        }

        Debug.Log($"Turn passed to: {nextPlayer.name} (player {currentPlayerIndex + 1}/{players.Count}, Bot: {isBot})");
        waitingForTurnSwitch = false;
    }
    
    public void OnPlayerTurnComplete()
    {
        if (autoSwitchTurns)
        {
            NextTurn();
        }
        else
        {
            Debug.Log("Click the dice for the next turn");
        }
    }
}
