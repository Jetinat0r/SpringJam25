using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject firstSelectedElement;

    public SoundPlayer soundPlayer, playerSoundPlayer;

    public GameObject soundPlayerPrefab;

    public GameObject canvasPanel;

    public static bool isMenuOpen = false;
    public static bool isMenuClosedThisFrame = false;
    //Player death or victory to avoid race conditions
    public static bool playerOverride = false;
    //Don't allow pausing mid room scroll transition
    public static bool roomScrollTransitionOverride = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundPlayer = Instantiate(soundPlayerPrefab).GetComponent<SoundPlayer>();
    }

    public void ToggleMenu(PlayerMovement player)
    {
        if (playerOverride || roomScrollTransitionOverride) return;
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
        canvasPanel.SetActive(true);
        Time.timeScale = 0;
        AudioManager.instance.PauseCurrent();
        eventSystem.SetSelectedGameObject(firstSelectedElement);
        playerSoundPlayer.PauseSound("all");
        isMenuOpen = true;
    }

    public void CloseMenu()
    {
        isMenuClosedThisFrame = true;

        canvasPanel.SetActive(false);
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
        EventSystem.current.gameObject.SetActive(false);

        soundPlayer.PlaySound("UI.Select");
        CloseMenu();
        Time.timeScale = 1;

        if (PlayerMovement.instance.isDead || PlayerMovement.instance.hasWon) return;
        LevelManager.instance.ResetScene();
    }

    public void ExitToMainMenu()
    {
        EventSystem.current.gameObject.SetActive(false);

        Time.timeScale = 1;
        soundPlayer.PlaySound("UI.Select");
        LevelManager.instance.ReturnToMainMenu();
        AudioManager.instance.UnPauseCurrent();
        isMenuOpen = false;
    }
}
