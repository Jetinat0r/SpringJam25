using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject challengeUnlockPanel;
    [SerializeField]
    public Button returnToMainMenuButton;

    public SoundPlayable selectSound, challengesUnlockedSound, ditSound, backSound;
    public SoundPlayer soundPlayer;
    
    public GameObject popupBox;
    public SpriteRenderer fakeScreenWipe;

    public GameObject skipBox;
    //Lock to prevent the skip box from appearing & functioning after the credits have finished and before the scene has exited
    public bool permanentSkipLock = false;
    //Whether or not the skip box can currently be seen and is ready for the skip input
    public bool skipBoxVisible = false;
    //bool on a 1 frame delay to prevent insta skipping
    public bool skipReady = false;
    public float skipBoxDisappearTimeSeconds = 3f;
    public Coroutine skipBoxDisappearRoutine = null;
    IDisposable anyInputListener = null;

    public PlayableDirector timeline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SetSelectedToReturnToMainMenuButton();
        if (!ProgramManager.instance.showChallengeUnlock)
        {
            anyInputListener = InputSystem.onAnyButtonPress.Call((_eventPtr) => { TryShowSkipBox(); });
            InputOverlord.instance.playerInput.actions["ToggleUI"].started += TrySkipCredits;
        }
    }

    private void OnDestroy()
    {
        if (!ProgramManager.instance.showChallengeUnlock)
        {
            anyInputListener.Dispose();
            InputOverlord.instance.playerInput.actions["ToggleUI"].started -= TrySkipCredits;

            if (skipBoxDisappearRoutine != null)
            {
                StopCoroutine(skipBoxDisappearRoutine);
            }
        }
    }

    private void TryShowSkipBox()
    {
        if (!permanentSkipLock)
        {
            skipBox.SetActive(true);
            skipBoxVisible = true;
            if (skipBoxDisappearRoutine != null)
            {
                StopCoroutine(skipBoxDisappearRoutine);
            }
            skipBoxDisappearRoutine = StartCoroutine(SkipBoxDisappearRoutine());
            StartCoroutine(SkipEnable());
        }
    }

    private IEnumerator SkipBoxDisappearRoutine()
    {
        yield return new WaitForSeconds(skipBoxDisappearTimeSeconds);
        skipBox.SetActive(false);
        skipBoxVisible = false;
        skipBoxDisappearRoutine = null;
        skipReady = false;
    }

    private IEnumerator SkipEnable()
    {
        yield return null;
        skipReady = true;
    }

    private void TrySkipCredits(InputAction.CallbackContext _context)
    {
        if (!permanentSkipLock && skipBoxVisible && skipReady)
        {
            /*
            StopCoroutine(skipBoxDisappearRoutine);
            skipBoxDisappearRoutine = null;

            permanentSkipLock = true;
            */

            soundPlayer.PlaySound(backSound);
            timeline.Pause();

            EndCreditsSequence();
        }
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
            skipReady = false;
            if (skipBoxDisappearRoutine != null)
            {
                StopCoroutine(skipBoxDisappearRoutine);
            }
            skipBoxDisappearRoutine = null;

            permanentSkipLock = true;

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
