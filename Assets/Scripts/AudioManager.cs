using UnityEngine;

/// <summary>
/// Singleton класс для управления громкостью музыки и звуковых эффектов
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Запускаем фоновую музыку
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Установить громкость музыки (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Установить громкость звуковых эффектов (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Получить текущую громкость музыки
    /// </summary>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// Получить текущую громкость звуковых эффектов
    /// </summary>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Воспроизвести звуковой эффект
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    /// <summary>
    /// Воспроизвести звуковой эффект с пользовательской громкостью
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }

    /// <summary>
    /// Загрузить сохраненные настройки
    /// </summary>
    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f); // По умолчанию 70%
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); // По умолчанию 100%

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    /// <summary>
    /// Включить/выключить музыку
    /// </summary>
    public void ToggleMusic(bool enabled)
    {
        if (musicSource != null)
        {
            if (enabled && !musicSource.isPlaying)
            {
                musicSource.Play();
            }
            else if (!enabled && musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }
    }

    /// <summary>
    /// Сменить фоновую музыку
    /// </summary>
    public void ChangeBackgroundMusic(AudioClip newMusic)
    {
        if (musicSource != null && newMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = newMusic;
            musicSource.Play();
        }
    }
}
