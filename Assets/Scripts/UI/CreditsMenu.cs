using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject challengeUnlockPanel;
    [SerializeField]
    public Button returnToMainMenuButton;

    public SoundPlayable selectSound, challengesUnlockedSound, ditSound;
    public SoundPlayer soundPlayer;
    
    public GameObject popupBox;
    public SpriteRenderer fakeScreenWipe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SetSelectedToReturnToMainMenuButton();
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
        //soundPlayer.PlaySound(selectSound);
    }

    public void EndCreditsSequence()
    {
        //Debug statement for testing:
        //ProgramManager.instance.showChallengeUnlock = true;
        if (ProgramManager.instance.showChallengeUnlock)
        {
            ChallengesUnlockedDisplayStartFade();
        }
        else
        {
            ReturnToMainMenu();
        }
    }

    //The fade out of the credist sequence
    public void ChallengesUnlockedDisplayStartFade()
    {
        AudioManager.instance.FadeOutCurrent();
        if (!ScreenWipe.current.WipeIn(() => { DisplayChallengeUnlockBox(); }))
        {
            return;
        }
    }

    public void DisplayChallengeUnlockBox()
    {
        challengeUnlockPanel.SetActive(true);
        //ScreenWipe.current.PostUnwipe += AllowClickingChallengeUnlock;
        StartCoroutine(FadeBackToChallengeBox(1f));
    }

    public IEnumerator FadeBackToChallengeBox(float _delaySeconds)
    {
        yield return new WaitForSeconds(_delaySeconds);
        ScreenWipe.current.WipeOut();
        soundPlayer.PlaySound(challengesUnlockedSound);
        yield return new WaitForSeconds(2.395f);
        AllowClickingChallengeUnlock();
    }

    public void AllowClickingChallengeUnlock()
    {
        //secondaryFocusIndicatorDisplayBlocker.SetActive(true);
        //returnToMainMenuButton.interactable = true;
        returnToMainMenuButton.gameObject.SetActive(true);
        soundPlayer.PlaySound(ditSound);
        SetSelectedToReturnToMainMenuButton();
    }

    public void ClickReturnButton()
    {
        returnToMainMenuButton.interactable = false;

        soundPlayer.PlaySound(selectSound);
        AudioManager.instance.ResetPlayer();
        ReturnToMainMenu();
    }

    public void PlayToggleSound()
    {
        soundPlayer.PlaySound("Game.Lever");
    }
}
