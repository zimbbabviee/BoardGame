using UnityEngine;
using System;


[Serializable]
public class SettingsData
{
    public float musicVolume = 0.7f;
    public float sfxVolume = 1f;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public bool isFullscreen = true;
    public int qualityLevel = 2;
    public bool vSyncEnabled = true;

    public void Save()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("ResolutionWidth", resolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", resolutionHeight);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
        PlayerPrefs.SetInt("VSync", vSyncEnabled ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("Настройки сохранены");
    }

    public void Load()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
        vSyncEnabled = PlayerPrefs.GetInt("VSync", 1) == 1;

        Debug.Log("Настройки загружены");
    }

    public void Apply()
    {
        Screen.SetResolution(resolutionWidth, resolutionHeight, isFullscreen);

        QualitySettings.SetQualityLevel(qualityLevel);

        QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(musicVolume);
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }

        Debug.Log($"Настройки применены: {resolutionWidth}x{resolutionHeight}, Качество: {qualityLevel}");
    }

    public void ResetToDefaults()
    {
        musicVolume = 0.7f;
        sfxVolume = 1f;
        resolutionWidth = 1920;
        resolutionHeight = 1080;
        isFullscreen = true;
        qualityLevel = 2;
        vSyncEnabled = true;

        Save();
        Apply();

        Debug.Log("Настройки сброшены к значениям по умолчанию");
    }

    public void SaveToJSON()
    {
        string json = JsonUtility.ToJson(this, true);
        string path = Application.persistentDataPath + "/settings.json";
        System.IO.File.WriteAllText(path, json);
        Debug.Log($"Настройки сохранены в JSON: {path}");
    }

    public void LoadFromJSON()
    {
        string path = Application.persistentDataPath + "/settings.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Настройки загружены из JSON: {path}");
        }
        else
        {
            Debug.LogWarning($"Файл настроек не найден: {path}");
        }
    }
}
