using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    public Button returnToMainMenuButton;

    public SoundPlayable selectSound;
    public SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetSelectedToReturnToMainMenuButton();
    }

    //Called by end of credits sequence
    public void SetSelectedToReturnToMainMenuButton()
    {
        EventSystem.current.SetSelectedGameObject(returnToMainMenuButton.gameObject);
    }

    public void ReturnToMainMenu()
    {
        if (!ScreenWipe.current.WipeIn(() => { SceneManager.LoadScene("MainMenu"); }))
        {
            return;
        }

        AudioManager.instance.CheckChangeWorlds("MainMenu");

        //TODO: Sound Player
        soundPlayer.PlaySound(selectSound);
    }
}
