using UnityEngine;
using System.IO;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    int index;
    private const string textFileName = "PlayersName";

    void Start()
    {
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        StartCoroutine(WaitForBoardAndSpawnPlayers());
    }

    System.Collections.IEnumerator WaitForBoardAndSpawnPlayers()
    {
        while (BoardManager.Instance == null)
        {
            yield return null;
        }

        while (TurnManager.Instance == null)
        {
            yield return null;
        }

        while (GameController.Instance == null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        BoardTile startTile = BoardManager.Instance.GetTile(1);
        Vector3 startPosition = startTile != null ? startTile.GetPlayerPosition() : spawnPoint.transform.position;

        GameObject mainCharacter = Instantiate(playerPrefabs[characterIndex],
            startPosition, Quaternion.identity);
        mainCharacter.GetComponent<NameScript>().SetName(PlayerPrefs.GetString("PlayerName", "Jagit Ler"));

        PlayerMovement playerMovement = mainCharacter.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = mainCharacter.AddComponent<PlayerMovement>();
        }

        playerMovement.SetStartPosition();

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.RegisterPlayer(playerMovement, false);
            Debug.Log($"Главный игрок {playerMovement.name} зарегистрирован первым!");
        }
        else if (GameController.Instance != null)
        {
            GameController.Instance.SetCurrentPlayerWithoutAutoStart(playerMovement, false);
        }

        yield return new WaitForSeconds(0.5f);

        string[] nameArray = ReadLinesFromFile(textFileName);

        int botCount = 3;

        System.Collections.Generic.List<int> availableSkins = new System.Collections.Generic.List<int>();
        for (int i = 0; i < playerPrefabs.Length; i++)
        {
            if (i != characterIndex)
            {
                availableSkins.Add(i);
            }
        }

        for (int botIndex = 0; botIndex < botCount; botIndex++)
        {
            int skinIndex;
            if (availableSkins.Count > 0)
            {
                int randomIndex = Random.Range(0, availableSkins.Count);
                skinIndex = availableSkins[randomIndex];
                availableSkins.RemoveAt(randomIndex);
            }
            else
            {
                skinIndex = (characterIndex + botIndex + 1) % playerPrefabs.Length;
            }

            GameObject botPlayer = Instantiate(playerPrefabs[skinIndex],
                startPosition, Quaternion.identity);

            string botName = nameArray.Length > 0
                ? nameArray[Random.Range(0, nameArray.Length)]
                : $"Bot_{botIndex + 1}";
            botPlayer.GetComponent<NameScript>().SetName(botName);

            PlayerMovement botPlayerMovement = botPlayer.GetComponent<PlayerMovement>();
            if (botPlayerMovement == null)
            {
                botPlayerMovement = botPlayer.AddComponent<PlayerMovement>();
            }
            botPlayerMovement.SetStartPosition();

            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.RegisterPlayer(botPlayerMovement, true); 
                Debug.Log($"Бот {botPlayer.name} зарегистрирован! ({botIndex + 1}/{botCount})");
            }
        }

        Debug.Log($"Всего создано {botCount} ботов");
    }


    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' },
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[0];
        }
    }
}