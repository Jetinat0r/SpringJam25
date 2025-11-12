using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    public List<RectTransform> levelSelectPanels;
    [SerializeField]
    public List<GameObject> levelSelectPanelFirstSelecteds;
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
    public LevelButton[] levelButtons = new LevelButton[32];

    //Used to slide the UI around. Cached because I don't know how things work when they slide
    private Vector2 anchoredMainPanelPos;
    private readonly List<Vector2> anchoredLevelSelectPanelPoses = new();
    private Vector2 anchoredSettingsPanelPos;
    private Vector2 anchoredInstructionsPanelPos;
    private Vector2 anchoredCreditsPanelPos;

    private Tween curTween;

    public SoundPlayable selectSound, backSound;
    public SoundPlayer soundPlayer;
    public static SoundPlayer menuSoundPlayer;

    private bool inSettings = false;

    [SerializeField]
    private PlayerInput debugInput;
    private InputAction resetProgress;
    private InputAction toggleEP;   // DEBUG PURPOSES ONLY. REPLACE WITH CHALLENGE MENU.

    [SerializeField]
    [Tooltip("Allows Right Shift + R to Reset all progress. Save for Debug and Showcase builds!")]
    public bool allowDebugFullComplete = false;

    public static bool inMenu = false;
    public static bool muteFirstButtonSound = true;

    void Awake()
    {
        inMenu = true;
        menuSoundPlayer = soundPlayer;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchoredMainPanelPos = mainPanel.anchoredPosition;
        foreach (RectTransform panel in levelSelectPanels)
        {
            anchoredLevelSelectPanelPoses.Add(panel.anchoredPosition);
        }
        anchoredSettingsPanelPos = settingsPanel.anchoredPosition;
        anchoredInstructionsPanelPos = instructionsPanel.anchoredPosition;
        anchoredCreditsPanelPos = creditsPanel.anchoredPosition;

        eventSystem.SetSelectedGameObject(mainPanelFirstSelected);

        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            //if (SettingsManager.completedLevels >= i)
            if (_completedLevels >= i)
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
        resetProgress.started += UnlockAll;

        toggleEP = debugInput.actions["ToggleEctoplasm"];
        toggleEP.Enable();
        toggleEP.started += ToggleEP;
    }

    private void OnDestroy()
    {
        resetProgress.started -= UnlockAll;
        muteFirstButtonSound = true;
    }

    //Has been morphed into UnlockAll
    public void UnlockAll(InputAction.CallbackContext _context)
    {
        if (!allowDebugFullComplete)
        {
            return;
        }

        soundPlayer.PlaySound(selectSound);
        //SettingsManager.completedLevels = 17;
        //SettingsManager.SaveSettings();
        ProgramManager.instance.LoadFullCompleteSaveData();
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            //if (SettingsManager.completedLevels >= i)
            if (_completedLevels >= i)
            {
                levelButtons[i].UnlockLevel();
            }
            else
            {
                levelButtons[i].LockLevel();
            }
        }
        //ResetProgress();
    }

    public void ResetProgress()
    {
        soundPlayer.PlaySound(backSound);
        //SettingsManager.completedLevels = 0;
        //SettingsManager.SaveSettings();
        ProgramManager.instance.ResetSaveData();
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            //if (SettingsManager.completedLevels >= i)
            if (_completedLevels >= i)
            {
                levelButtons[i].UnlockLevel();
            }
            else
            {
                levelButtons[i].LockLevel();
            }
        }
    }

    public void ToggleEP(InputAction.CallbackContext _context)
    {
        if (ChallengeManager.currentMode == ChallengeManager.ChallengeMode.None)
        {
            ChallengeManager.currentMode = ChallengeManager.ChallengeMode.Ectoplasm;
            soundPlayer.PlaySound(selectSound);
            Debug.Log("Ectoplasm Enabled!");

        }
        else if (ChallengeManager.currentMode == ChallengeManager.ChallengeMode.Ectoplasm)
        {
            ChallengeManager.currentMode = ChallengeManager.ChallengeMode.None;
            soundPlayer.PlaySound(backSound);
            Debug.Log("Ectoplasm Disabled!");
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
            //SettingsManager.SaveSettings();
            ProgramManager.instance.saveData.SaveSaveData();
        }

        MenuPanelWatcher.instance.activePanel = MenuPanel.MAIN;
        eventSystem.SetSelectedGameObject(mainPanelFirstSelected);
    }

    public void MoveToLevelSelectPanel(int num = 0)
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredLevelSelectPanelPoses[num], tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);

        MenuPanelWatcher.instance.activePanel = MenuPanel.LEVELS;
        eventSystem.SetSelectedGameObject(levelSelectPanelFirstSelecteds[num]);
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
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();
        //if (SettingsManager.completedLevels >= levelButtons.Length)
        if (_completedLevels >= levelButtons.Length)
        {
            EnterLevel(levelButtons[^1].levelName);
        }
        else
        {
            //EnterLevel(levelButtons[SettingsManager.completedLevels].levelName);
            EnterLevel(levelButtons[_completedLevels].levelName);
        }
    }

    public void EnterLevel(string _levelName)
    {
        if (!ScreenWipe.current.WipeIn()) return;
        ScreenWipe.current.PostWipe += () =>
        {
            //Debug.Log($"Entering level {_levelName}");
            SceneManager.LoadScene(_levelName);
        };

        curTween?.Kill();

        EventSystem.current.gameObject.SetActive(false);

        AudioManager.instance.CheckChangeWorlds(_levelName);

        soundPlayer.PlaySound(selectSound);
        inMenu = false;
    }

    public void EnterLevel(LevelButton _levelButton)
    {
        EnterLevel(_levelButton.levelName);
    }

    public void QuitGame()
    {
        if (!ScreenWipe.current.WipeIn()) return;
        ScreenWipe.current.PostWipe += () =>
        {
            //Debug.Log($"Entering level {_levelName}");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };
        soundPlayer.PlaySound(backSound);
        AudioManager.instance.FadeOutCurrent();
    }
}
