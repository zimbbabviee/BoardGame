using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveDelay = 0.5f;
    private int currentTileNumber = 1;
    private bool isMoving = false;
    private int totalMoves = 0;
    private bool hasFinished = false;
    private int finishPlace = 0;

    private Vector3 playerOffset = Vector3.zero;
    private static int playerCounter = 0;
    private static int finishPlaceCounter = 0;

    private void Awake()
    {
        int playerIndex = playerCounter++;

        float offset = 0.25f;
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-offset, 0f, -offset), 
            new Vector3(offset, 0f, -offset),  
            new Vector3(-offset, 0f, offset), 
            new Vector3(offset, 0f, offset)   
        };

        playerOffset = positions[playerIndex % positions.Length];
        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(
            Mathf.Abs(scale.x) * 0.7f,
            Mathf.Abs(scale.y) * 0.7f,
            Mathf.Abs(scale.z) * 0.7f
        );
    }

    public void Move(int steps)
    {
        if (isMoving)
        {
            Debug.LogWarning("Player is already moving!");
            return;
        }

        StartCoroutine(MoveCoroutine(steps));
    }

    private IEnumerator MoveCoroutine(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentTileNumber++;

            if (currentTileNumber > BoardManager.Instance.GetTotalTiles())
            {
                currentTileNumber = BoardManager.Instance.GetTotalTiles();
                break;
            }

            BoardTile targetTile = BoardManager.Instance.GetTile(currentTileNumber);
            if (targetTile != null)
            {
                yield return StartCoroutine(MoveToTile(targetTile));
            }

            yield return new WaitForSeconds(moveDelay);
        }

        BoardTile landedTile = BoardManager.Instance.GetTile(currentTileNumber);
        if (landedTile != null)
        {
            int extraMoves = landedTile.ProcessTileEffect();

            if (extraMoves > 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveForward(extraMoves));
            }
            else if (extraMoves < 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine(MoveBackward(Mathf.Abs(extraMoves)));
            }

            if (landedTile.Type == TileType.Finish)
            {
                OnReachFinish();
            }
        }

        isMoving = false;

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnPlayerTurnComplete();
        }
    }

    private IEnumerator MoveForward(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            currentTileNumber++;

            if (currentTileNumber > BoardManager.Instance.GetTotalTiles())
            {
                currentTileNumber = BoardManager.Instance.GetTotalTiles();
                break;
            }

            BoardTile targetTile = BoardManager.Instance.GetTile(currentTileNumber);
            if (targetTile != null)
            {
                yield return StartCoroutine(MoveToTile(targetTile));
            }

            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator MoveBackward(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            currentTileNumber--;

            if (currentTileNumber < 1)
            {
                currentTileNumber = 1;
                break;
            }

            BoardTile targetTile = BoardManager.Instance.GetTile(currentTileNumber);
            if (targetTile != null)
            {
                yield return StartCoroutine(MoveToTile(targetTile));
            }

            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator MoveToTile(BoardTile tile)
    {
        Vector3 targetPosition = tile.GetPlayerPosition() + playerOffset;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / moveSpeed;

        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    public void SetStartPosition()
    {
        currentTileNumber = 1;
        BoardTile startTile = BoardManager.Instance.GetTile(1);
        if (startTile != null)
        {
            transform.position = startTile.GetPlayerPosition() + playerOffset;
        }
    }

    private void OnReachFinish()
    {
        hasFinished = true;
        finishPlaceCounter++;
        finishPlace = finishPlaceCounter;

        Debug.Log($"Player {gameObject.name} reached the finish! Place: {finishPlace}");

        bool isBot = GameController.Instance != null && GameController.Instance.IsCurrentPlayerBot();

        if (!isBot)
        {
            string playerName = gameObject.name;
            NameScript nameScript = GetComponent<NameScript>();
            if (nameScript != null)
            {
                playerName = nameScript.GetName();
            }

            GameInfoUI gameInfoUI = FindObjectOfType<GameInfoUI>();
            float gameTime = gameInfoUI != null ? gameInfoUI.GetGameTime() : 0f;

            WinScreenUI winScreen = WinScreenUI.Instance;
            if (winScreen == null)
            {
                winScreen = FindObjectOfType<WinScreenUI>();
            }

            if (winScreen != null)
            {
                winScreen.ShowWinScreen(playerName, gameTime, totalMoves, finishPlace);
            }
        }
    }

    public static void ResetFinishPlaceCounter()
    {
        finishPlaceCounter = 0;
    }

    public int GetCurrentTileNumber()
    {
        return currentTileNumber;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void AddMove()
    {
        totalMoves++;
    }

    public int GetTotalMoves()
    {
        return totalMoves;
    }

    public bool HasFinished()
    {
        return hasFinished;
    }
}
