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
        if (currentPlayer != null)
        {
            currentPlayer.SetStartPosition();
        }

        Debug.Log("Игра началась! Бросьте кубик чтобы сделать ход.");
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
            Debug.Log($"Бот {currentPlayer.name} бросает кубик...");
            diceScript.RollDice();
        }
    }

    private void OnDiceRolled(int number)
    {
        waitingForDiceRoll = false;

        Debug.Log($"Выпало {number}! Перемещаем игрока...");

        if (currentPlayer != null && !currentPlayer.IsMoving())
        {
            currentPlayer.Move(number);

            if (gameInfoUI != null)
            {
                gameInfoUI.AddMove();
            }
        }

        Invoke(nameof(ResetForNextRoll), 2f);
    }

    private void ResetForNextRoll()
    {
        if (currentPlayer != null && currentPlayer.IsMoving())
        {
            Debug.Log("Игрок еще движется, ждем завершения...");
            Invoke(nameof(ResetForNextRoll), 0.5f);
            return;
        }

        waitingForDiceRoll = true;

        if (diceScript != null)
        {
            diceScript.ResetDice();
        }

        Debug.Log("Готов к следующему броску!");
    }

      public void SetCurrentPlayerWithoutAutoStart(PlayerMovement player, bool isBot = false)
    {
        currentPlayer = player;
        isCurrentPlayerBot = isBot;
        Debug.Log($"Current player set: {player.name} (Bot: {isBot}) - Ожидание первого хода");
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

        Debug.Log($"Ход игрока: {newPlayer.name} (Bot: {isBot})");

        if (isBot)
        {
            StartPlayerTurn();
        }
        else
        {
            Debug.Log($"Ваш ход! Кликните на кубик чтобы бросить.");
        }
    }

    public void RestartGame()
    {
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

        Debug.Log("Игра перезапущена!");
    }
}
