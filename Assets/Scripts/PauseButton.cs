using UnityEngine;
public class PauseButton : MonoBehaviour
{
    [SerializeField] private PauseManager pauseManager;

    public void OnPauseButtonClicked()
    {
        if (pauseManager != null)
        {
            pauseManager.PauseGame();
        }
        else
        {
            Debug.LogWarning("PauseManager не назначен в PauseButton!");
        }
    }
}
