using UnityEngine;
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DiceRollScript diceScript;
    [SerializeField] private PlayerMovement currentPlayer;
    [SerializeField] private GameInfoUI gameInfoUI;

    [Header("Bot Settings")]
    [SerializeField] private float botRollDelay = 2.5f; 

    private bool waitingForDiceRoll = true;
    private int lastRolledNumber = 0;
    private bool isCurrentPlayerBot = false;

    public bool IsCurrentPlayerBot()
    {
        return isCurrentPlayerBot;
    }

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
        PlayerMovement.ResetFinishPlaceCounter();

        if (gameInfoUI == null)
        {
            gameInfoUI = FindObjectOfType<GameInfoUI>();
        }

        if (currentPlayer != null)
        {
            currentPlayer.SetStartPosition();
        }

        Debug.Log("Game started! Roll the dice to make a move.");
    }

    private void Update()
    {
        if (waitingForDiceRoll && diceScript != null && diceScript.isLanded)
        {
            if (int.TryParse(diceScript.diceFaceNum, out int rolledNumber))
            {
                if (rolledNumber != lastRolledNumber)
                {
                    lastRolledNumber = rolledNumber;
                    OnDiceRolled(rolledNumber);
                }
            }
        }
    }

    public void StartPlayerTurn()
    {
        CancelInvoke(nameof(BotRollDice));

        if (isCurrentPlayerBot && waitingForDiceRoll && diceScript != null)
        {
            Invoke(nameof(BotRollDice), botRollDelay);
        }
    }

    private void BotRollDice()
    {
        if (diceScript != null && waitingForDiceRoll && diceScript.isLanded)
        {
            Debug.Log($"Bot {currentPlayer.name} is rolling the dice...");
            diceScript.RollDice();
        }
    }

    private void OnDiceRolled(int number)
    {
        waitingForDiceRoll = false;

        Debug.Log($"Rolled {number}! Moving player...");

        if (currentPlayer != null && !currentPlayer.IsMoving())
        {
            currentPlayer.AddMove();
            currentPlayer.Move(number);
        }

        Invoke(nameof(ResetForNextRoll), 2f);
    }

    private void ResetForNextRoll()
    {
        if (currentPlayer != null && currentPlayer.IsMoving())
        {
            Debug.Log("Player is still moving, waiting...");
            Invoke(nameof(ResetForNextRoll), 0.5f);
            return;
        }

        waitingForDiceRoll = true;

        if (diceScript != null)
        {
            diceScript.ResetDice();
        }

        Debug.Log("Ready for next roll!");
    }

      public void SetCurrentPlayerWithoutAutoStart(PlayerMovement player, bool isBot = false)
    {
        currentPlayer = player;
        isCurrentPlayerBot = isBot;
        Debug.Log($"Current player set: {player.name} (Bot: {isBot}) - Waiting for first turn");
    }

    public void SetCurrentPlayer(PlayerMovement player, bool isBot = false)
    {
        currentPlayer = player;
        isCurrentPlayerBot = isBot;
        Debug.Log($"Current player set: {player.name} (Bot: {isBot})");

        if (isBot)
        {
            StartPlayerTurn();
        }
    }

    public void SwitchPlayer(PlayerMovement newPlayer, bool isBot = false)
    {
        currentPlayer = newPlayer;
        isCurrentPlayerBot = isBot;
        lastRolledNumber = 0;

        Debug.Log($"Player turn: {newPlayer.name} (Bot: {isBot})");

        if (isBot)
        {
            StartPlayerTurn();
        }
        else
        {
            Debug.Log($"Your turn! Click on the dice to roll.");
        }
    }

    public void RestartGame()
    {
        PlayerMovement.ResetFinishPlaceCounter();

        if (currentPlayer != null)
        {
            currentPlayer.SetStartPosition();
        }

        if (diceScript != null)
        {
            diceScript.ResetDice();
        }

        waitingForDiceRoll = true;
        lastRolledNumber = 0;

        Debug.Log("Game restarted!");
    }
}
