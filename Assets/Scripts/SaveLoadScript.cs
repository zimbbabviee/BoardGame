using UnityEngine;
using System.IO;
using System;

public class SaveLoadScript : MonoBehaviour
{
    public string saveFileName = "pickme.json";

    [Serializable]
    public class GameData
    {
        public int character;
        public string characterName;
    }

    private GameData gameData = new GameData();

    public void SaveGame(int character, string characterName)
    {
        gameData.character = character;
        gameData.characterName = characterName;

        string json = JsonUtility.ToJson(gameData);

        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Game path: " + Application.persistentDataPath + "/" + saveFileName);
    }

    public void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found" + filePath);
        }
    }
}
