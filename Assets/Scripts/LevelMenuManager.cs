using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject firstSelectedElement;

    public static bool isMenuOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ToggleMenu()
    {
        if (!isMenuOpen)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        AudioManager.instance.PauseCurrent();
        eventSystem.SetSelectedGameObject(firstSelectedElement);
        foreach (SoundPlayer sound in FindObjectsByType<SoundPlayer>(FindObjectsSortMode.None))
        {
            sound.PauseSound("all");
        }
        isMenuOpen = true;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        AudioManager.instance.UnPauseCurrent();
        eventSystem.SetSelectedGameObject(null);
        foreach (SoundPlayer sound in FindObjectsByType<SoundPlayer>(FindObjectsSortMode.None))
        {
            sound.UnPauseSound("all");
        }
        isMenuOpen = false;
    }

    public void RestartLevel()
    {
        LevelManager.instance.ResetScene();
        Time.timeScale = 1;
        CloseMenu();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        LevelManager.instance.ReturnToMainMenu();
    }
}
