using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject firstSelectedElement;

    public SoundPlayer soundPlayer, playerSoundPlayer;

    public GameObject soundPlayerPrefab;

    public static bool isMenuOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundPlayer = Instantiate(soundPlayerPrefab).GetComponent<SoundPlayer>();
    }

    public void ToggleMenu(PlayerMovement player)
    {
        playerSoundPlayer = player.soundPlayer;
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
        playerSoundPlayer.PauseSound("all");
        isMenuOpen = true;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        AudioManager.instance.UnPauseCurrent();
        eventSystem.SetSelectedGameObject(null);
        playerSoundPlayer.UnPauseSound("all");
        isMenuOpen = false;
    }

    public void PlayBackSound()
    {
        soundPlayer.PlaySound("UI.Back");
    }

    public void RestartLevel()
    {
        soundPlayer.PlaySound("UI.Select");
        // TODO this should use a delay + screen transition
        LevelManager.instance.ResetScene();
        Time.timeScale = 1;
        CloseMenu();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1;
        soundPlayer.PlaySound("UI.Confirm");
        // TODO this should use a delay + screen transition
        LevelManager.instance.ReturnToMainMenu();
    }
}
