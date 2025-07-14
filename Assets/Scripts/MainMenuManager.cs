using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    public RectTransform panelContainer;
    [SerializeField]
    public RectTransform mainPanel;
    [SerializeField]
    public GameObject mainPanelFirstSelected;
    [SerializeField]
    public RectTransform levelSelectPanel;
    [SerializeField]
    public GameObject levelSelectPanelFirstSelected;
    [SerializeField]
    public RectTransform settingsPanel;
    [SerializeField]
    public GameObject settingsPanelFirstSelected;
    [SerializeField]
    public RectTransform instructionsPanel;
    [SerializeField]
    public GameObject instructionsPanelFirstSelected;
    [SerializeField]
    public RectTransform creditsPanel;
    [SerializeField]
    public GameObject creditsPanelFirstSelected;


    [SerializeField]
    [Tooltip("Move time in seconds")]
    public float tweenMoveTime = 1f;
    [SerializeField]
    public Ease tweenEaseType = Ease.OutQuint;
    
    [SerializeField]
    public LevelButton[] levelButtons = new LevelButton[12];

    //Used to slide the UI around. Cached because I don't know how things work when they slide
    private Vector2 anchoredMainPanelPos;
    private Vector2 anchoredLevelSelectPanelPos;
    private Vector2 anchoredSettingsPanelPos;
    private Vector2 anchoredInstructionsPanelPos;
    private Vector2 anchoredCreditsPanelPos;

    private Tween curTween;

    public SoundClip selectSound, backSound;
    public SoundPlayer soundPlayer;

    public LabelledSliderLinker musicSetting, sfxSetting;

    private bool inSettings = false;

    [SerializeField]
    private PlayerInput debugInput;
    private InputAction resetProgress;

    [SerializeField]
    [Tooltip("Allows Right Shift + R to Reset all progress. Save for Debug and Showcase builds!")]
    public bool allowProgressDeletion = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchoredMainPanelPos = mainPanel.anchoredPosition;
        anchoredLevelSelectPanelPos = levelSelectPanel.anchoredPosition;
        anchoredSettingsPanelPos = settingsPanel.anchoredPosition;
        anchoredInstructionsPanelPos = instructionsPanel.anchoredPosition;
        anchoredCreditsPanelPos = creditsPanel.anchoredPosition;

        musicSetting.SetValue(Mathf.Pow(10, SettingsManager.musicVolume / 20) * 100 - 0.00001f);
        sfxSetting.SetValue(Mathf.Pow(10, SettingsManager.sfxVolume / 20) * 100 - 0.00001f);

        eventSystem.SetSelectedGameObject(mainPanelFirstSelected);

        for(int i = 0; i < levelButtons.Length; i++)
        {
            if(SettingsManager.completedLevels >= i)
            {
                levelButtons[i].UnlockLevel();
            }
            else
            {
                levelButtons[i].LockLevel();
            }
        }

        resetProgress = debugInput.actions["ResetProgress"];
        resetProgress.Enable();
        resetProgress.started += ResetProgress;
    }

    private void OnDestroy()
    {
        resetProgress.started -= ResetProgress;
    }

    public void ResetProgress(InputAction.CallbackContext _context)
    {
        if (!allowProgressDeletion)
        {
            return;
        }

        ResetProgress();
    }

    public void ResetProgress()
    {
        SettingsManager.completedLevels = 0;
        SettingsManager.SaveSettings();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (SettingsManager.completedLevels >= i)
            {
                levelButtons[i].UnlockLevel();
            }
            else
            {
                levelButtons[i].LockLevel();
            }
        }
    }

    public void MoveToMainMenu(bool _isXMove)
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredMainPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(backSound);
        if (inSettings)
        {
            inSettings = false;
            SettingsManager.SaveSettings();
        }

        MenuPanelWatcher.instance.activePanel = MenuPanel.MAIN;
        eventSystem.SetSelectedGameObject(mainPanelFirstSelected);
    }

    public void MoveToLevelSelectPanel()
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredLevelSelectPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);

        MenuPanelWatcher.instance.activePanel = MenuPanel.LEVELS;
        eventSystem.SetSelectedGameObject(levelSelectPanelFirstSelected);
    }

    public void MoveToSettingsPanel()
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredSettingsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);
        inSettings = true;

        MenuPanelWatcher.instance.activePanel = MenuPanel.SETTINGS;
        eventSystem.SetSelectedGameObject(settingsPanelFirstSelected);
    }

    public void MoveToInstructionsPanel()
    {
        instructionsPanel.gameObject.SetActive(true);
        creditsPanel.gameObject.SetActive(false);

        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredInstructionsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);

        MenuPanelWatcher.instance.activePanel = MenuPanel.INSTRUCTIONS;
        eventSystem.SetSelectedGameObject(instructionsPanelFirstSelected);
    }

    public void MoveToCreditsPanel()
    {
        instructionsPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(true);

        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredCreditsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);

        MenuPanelWatcher.instance.activePanel = MenuPanel.CREDITS;
        eventSystem.SetSelectedGameObject(creditsPanelFirstSelected);
    }

    public void ContinuePlaying()
    {
        soundPlayer.PlaySound(selectSound);
        if (SettingsManager.completedLevels >= levelButtons.Length)
        {
            EnterLevel(levelButtons[^1].levelName);
        }
        else
        {
            EnterLevel(levelButtons[SettingsManager.completedLevels].levelName);
        }
    }

    public void EnterLevel(string _levelName)
    {
        curTween?.Kill();

        EventSystem.current.gameObject.SetActive(false);

        soundPlayer.PlaySound(selectSound);
        ScreenWipe.current.WipeIn();
        ScreenWipe.current.PostWipe += () =>
        {
            //Debug.Log($"Entering level {_levelName}");
            SceneManager.LoadScene(_levelName);
        };
    }

    public void EnterLevel(LevelButton _levelButton)
    {
        EnterLevel(_levelButton.levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
