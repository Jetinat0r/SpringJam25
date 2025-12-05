using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject firstSelectedElement;

    public SoundPlayer soundPlayer, playerSoundPlayer;

    public GameObject soundPlayerPrefab;

    public GameObject canvasPanel;

    public GameObject activeChallengeParent;
    public GameObject ectoplasmChallengeIcon;
    public GameObject lightsOutChallengeIcon;
    public GameObject spectralShuffleChallengeIcon;

    [Header("Spectral Shuffle Challenge Box")]
    public RectTransform spectralShuffleChallengeBox;
    public TMP_Text spectralShuffleChallengeName;
    Sequence challengeBoxSequence;
    public float tweenDist = 110f;
    public float tweenMoveTime = 1.5f;
    public float tweenHoldTime = 1f;
    public Ease tweenEasing = Ease.InOutQuint;

    public static bool isMenuOpen = false;
    public static bool isMenuClosedThisFrame = false;
    //Player death or victory to avoid race conditions
    public static bool playerOverride = false;
    //Don't allow pausing mid room scroll transition
    public static bool roomScrollTransitionOverride = false;
    //True if restarting or leaving level
    public static bool isTransitioningFromMenu = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isTransitioningFromMenu = false;
        isMenuOpen = false;
        eventSystem = InputOverlord.instance.eventSystem;

        soundPlayer = Instantiate(soundPlayerPrefab, transform).GetComponent<SoundPlayer>();
        MainMenuManager.menuSoundPlayer = soundPlayer;

        if (ChallengeManager.instance.ectoplasmEnabled || ChallengeManager.instance.lightsOutEnabled || ChallengeManager.instance.spectralShuffleEnabled)
        {
            activeChallengeParent.SetActive(true);
            ectoplasmChallengeIcon.SetActive(ChallengeManager.instance.ectoplasmEnabled);
            lightsOutChallengeIcon.SetActive(ChallengeManager.instance.lightsOutEnabled);
            spectralShuffleChallengeIcon.SetActive(ChallengeManager.instance.spectralShuffleEnabled);
        }
        else
        {
            activeChallengeParent.SetActive(false);
        }

        //DisplaySpectralShuffleChallenge("CUSTOM");
    }

    private void OnDestroy()
    {
        challengeBoxSequence?.Kill();
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
        soundPlayer.PlaySound("UI.Pause");
        if (SpeedrunManager.instance)
            SpeedrunManager.instance.PauseTimer();
        isMenuOpen = true;

        challengeBoxSequence?.Pause();
        spectralShuffleChallengeBox.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        isMenuClosedThisFrame = true;

        canvasPanel.SetActive(false);
        Time.timeScale = 1;
        AudioManager.instance.UnPauseCurrent();
        eventSystem.SetSelectedGameObject(null);
        playerSoundPlayer.UnPauseSound("all");
        soundPlayer.PlaySound("UI.Pause");
        if (SpeedrunManager.instance)
            SpeedrunManager.instance.ContinueTimer();
        isMenuOpen = false;

        spectralShuffleChallengeBox.gameObject.SetActive(true);
        challengeBoxSequence?.Play();
    }

    public void PlayBackSound()
    {
        soundPlayer.PlaySound("UI.Back");
    }

    public void RestartLevel()
    {
        isTransitioningFromMenu = true;

        EventSystem.current.gameObject.SetActive(false);

        soundPlayer.PlaySound("UI.Select");
        CloseMenu();
        Time.timeScale = 1;

        if (PlayerMovement.instance.isDead || PlayerMovement.instance.hasWon) return;
        LevelManager.instance.ResetScene();
    }

    public void ExitToMainMenu()
    {
        if (!LevelManager.instance.ReturnToMainMenu()) return;
        isTransitioningFromMenu = true;
        EventSystem.current.gameObject.SetActive(false);

        Time.timeScale = 1;
        soundPlayer.PlaySound("UI.Select");
        AudioManager.instance.UnPauseCurrent();
        if (SpeedrunManager.instance)
            Destroy(SpeedrunManager.instance);
        isMenuOpen = false;
    }

    public void DisplaySpectralShuffleChallenge(string _challengeName)
    {
        spectralShuffleChallengeName.SetText(_challengeName);

        Vector2 _initialPos = spectralShuffleChallengeBox.anchoredPosition;

        challengeBoxSequence = DOTween.Sequence();
        challengeBoxSequence.SetUpdate(true);
        challengeBoxSequence.Append(spectralShuffleChallengeBox.DOAnchorPosY(_initialPos.y - tweenDist, tweenMoveTime).SetEase(tweenEasing))
            .AppendInterval(tweenHoldTime)
            .Append(spectralShuffleChallengeBox.DOAnchorPosY(_initialPos.y, tweenMoveTime).SetEase(tweenEasing));
        challengeBoxSequence.Play();
    }
}
