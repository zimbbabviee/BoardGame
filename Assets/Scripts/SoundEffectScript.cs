using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] soundEffect;
    public AudioSource audioSource;

    private void Start()
    {
        if (AudioManager.Instance != null && audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.GetSFXVolume();
        }
    }

    public void Hover()
    {
        PlaySound(soundEffect[0]);
    }

    public void Click()
    {
        PlaySound(soundEffect[1]);
    }

    public void OnDice()
    {
        if (audioSource != null && soundEffect.Length > 2 && soundEffect[2] != null)
        {
            audioSource.loop = true;
            audioSource.clip = soundEffect[2];

            if (AudioManager.Instance != null)
            {
                audioSource.volume = AudioManager.Instance.GetSFXVolume();
            }

            audioSource.Play();
        }
    }

    public void CancelButton()
    {
        PlaySound(soundEffect[3]);
    }

    public void PlayButton()
    {
        PlaySound(soundEffect[4]);
    }

    public void NameField()
    {
        PlaySound(soundEffect[5]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(clip);
            }
            else
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}