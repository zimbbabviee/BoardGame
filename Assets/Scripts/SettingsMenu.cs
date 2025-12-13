using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Управление меню настроек игры
/// Функции: разрешение экрана, громкость музыки/звуковых эффектов, качество графики, VSync
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buttonsObject; // Объект с кнопками меню (Buttons)
    [SerializeField] private GameObject headerObject; // Объект с Header (Ribbon и Medal)
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    [Header("Volume Labels (Optional)")]
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    private Resolution[] resolutions;
    private int currentResolutionIndex;

    private void Start()
    {
        SetupResolutions();
        SetupQualityLevels();
        LoadSettings();

        // Подписка на события изменения слайдеров
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
        if (vsyncToggle != null)
        {
            vsyncToggle.onValueChanged.AddListener(OnVSyncToggled);
        }
    }

    /// <summary>
    /// Настройка доступных разрешений экрана
    /// </summary>
    private void SetupResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions
            .Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
            .Distinct()
            .ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // Найти текущее разрешение
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Настройка уровней качества графики
    /// </summary>
    private void SetupQualityLevels()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();

        List<string> qualityNames = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityNames);
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Загрузить сохраненные настройки
    /// </summary>
    private void LoadSettings()
    {
        // Громкость музыки
        if (musicVolumeSlider != null && AudioManager.Instance != null)
        {
            musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
            UpdateMusicVolumeText(musicVolumeSlider.value);
        }

        // Громкость звуковых эффектов
        if (sfxVolumeSlider != null && AudioManager.Instance != null)
        {
            sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
            UpdateSFXVolumeText(sfxVolumeSlider.value);
        }

        // Полноэкранный режим
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
        }

        // VSync
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
        }
    }

    /// <summary>
    /// Обработчик изменения громкости музыки
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
        else
        {
            // Сохранить напрямую, если AudioManager не доступен
            PlayerPrefs.SetFloat("MusicVolume", value);
            AudioListener.volume = value; // Временное решение - меняет общую громкость
        }
        UpdateMusicVolumeText(value);
    }

    /// <summary>
    /// Обработчик изменения громкости звуковых эффектов
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        else
        {
            // Сохранить напрямую, если AudioManager не доступен
            PlayerPrefs.SetFloat("SFXVolume", value);
        }
        UpdateSFXVolumeText(value);
    }

    /// <summary>
    /// Обновить текст громкости музыки
    /// </summary>
    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    /// <summary>
    /// Обновить текст громкости звуковых эффектов
    /// </summary>
    private void UpdateSFXVolumeText(float value)
    {
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }

    /// <summary>
    /// Обработчик изменения разрешения экрана
    /// </summary>
    private void OnResolutionChanged(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Обработчик изменения качества графики
    /// </summary>
    private void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Обработчик переключения полноэкранного режима
    /// </summary>
    private void OnFullscreenToggled(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Обработчик переключения вертикальной синхронизации
    /// </summary>
    private void OnVSyncToggled(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Открыть меню настроек
    /// </summary>
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        // Скрыть кнопки меню
        if (buttonsObject != null)
        {
            buttonsObject.SetActive(false);
        }

        // Скрыть Header (Ribbon и Medal)
        if (headerObject != null)
        {
            headerObject.SetActive(false);
        }
    }

    /// <summary>
    /// Закрыть меню настроек
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // Показать кнопки меню обратно
        if (buttonsObject != null)
        {
            buttonsObject.SetActive(true);
        }

        // Показать Header обратно
        if (headerObject != null)
        {
            headerObject.SetActive(true);
        }
    }

    /// <summary>
    /// Сбросить настройки к значениям по умолчанию
    /// </summary>
    public void ResetToDefaults()
    {
        // Громкость
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 0.7f;
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1f;
        }

        // Разрешение (самое высокое)
        if (resolutionDropdown != null && resolutions != null && resolutions.Length > 0)
        {
            resolutionDropdown.value = resolutions.Length - 1;
            OnResolutionChanged(resolutions.Length - 1);
        }

        // Качество (среднее)
        if (qualityDropdown != null)
        {
            int defaultQuality = QualitySettings.names.Length / 2;
            qualityDropdown.value = defaultQuality;
            OnQualityChanged(defaultQuality);
        }

        // Полный экран
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = true;
        }

        // VSync включен
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = true;
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        // Отписка от событий
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        }
        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.RemoveListener(OnQualityChanged);
        }
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenToggled);
        }
        if (vsyncToggle != null)
        {
            vsyncToggle.onValueChanged.RemoveListener(OnVSyncToggled);
        }
    }
}
