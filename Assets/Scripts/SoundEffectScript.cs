using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] soundEffect;
    public AudioSource audioSource;
    public void Hover()
    {
        audioSource.PlayOneShot(soundEffect[0]);
    }

    public void Click()
    {
        audioSource.PlayOneShot(soundEffect[1]);
    }

    public void OnDice()
    {
        audioSource.loop = true;
        audioSource.clip = soundEffect[2];
        audioSource.Play();
    }

    public void CancelButton()
    {
        audioSource.PlayOneShot(soundEffect[3]);
    }

    public void PlayButton()
    {
        audioSource.PlayOneShot(soundEffect[4]);
    }

    public void NameField()
    {
        audioSource.PlayOneShot(soundEffect[5]);
    }
}