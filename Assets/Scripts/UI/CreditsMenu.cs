using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    public Button returnToMainMenuButton;

    public SoundPlayable selectSound, challengesUnlockedSound;
    public SoundPlayer soundPlayer;
    
    public GameObject popupBox;
    public SpriteRenderer fakeScreenWipe;

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

    public void ChallengesUnlockedSequence()
    {
        // TODO: Add persistent save data for if this message has already played, and check against it
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => AudioManager.instance.FadeOutCurrent());
        sequence.Append(fakeScreenWipe.DOFade(1, 1));
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() => AudioManager.instance.ResetPlayer());
        sequence.AppendCallback(() => soundPlayer.PlaySound(challengesUnlockedSound));
        sequence.AppendCallback(() => popupBox.SetActive(true));
        sequence.Play();
    }
}
