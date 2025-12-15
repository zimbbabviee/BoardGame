using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CreateAudioSources();
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void CreateAudioSources()
    {
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        GameObject sfxObj = new GameObject("SFXSource");
        sfxObj.transform.SetParent(transform);
        sfxSource = sfxObj.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    private void Start()
    {
        if (musicSource != null && backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

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

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }

    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f); 
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); 

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

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
