using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AudioManager;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    public EventSystem eventSystem;

    [SerializeField]
    public GameObject corruptedSaveDataScreen;
    [SerializeField]
    public Button corruptedSaveDataScreenFirstSelected;
    [SerializeField]
    public GameObject startScreen;
    //These 2 are different because of spaghetti code :3
    //  They need to be synced exactly once per play session :P
    [SerializeField]
    public OscillatingLogo mainLogo;
    [SerializeField]
    public OscillatingLogo startScreenLogo;

    [SerializeField]
    //Use this button to detect start input
    public Button startScreenHiddenButton;
    //private bool startedMusic = false;

    [SerializeField]
    public RectTransform panelContainer;
    [SerializeField]
    public RectTransform mainPanel;
    [SerializeField]
    public GameObject mainPanelFirstSelected;
    [SerializeField]
    public LevelSelectMenu levelSelectMenu;
    [SerializeField]
    public RectTransform levelSelectPanel;
    [SerializeField]
    public List<Button> levelSelectPanelFirstSelecteds;
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

    //Used to slide the UI around. Cached because I don't know how things work when they slide
    private Vector2 anchoredMainPanelPos;
    private Vector2 anchoredLevelSelectPanelPos;
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

        mainPanel.gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventSystem = InputOverlord.instance.eventSystem;
        debugInput = InputOverlord.instance.playerInput;

        anchoredMainPanelPos = mainPanel.anchoredPosition;
        anchoredLevelSelectPanelPos = levelSelectPanel.anchoredPosition;
        anchoredSettingsPanelPos = settingsPanel.anchoredPosition;
        anchoredInstructionsPanelPos = instructionsPanel.anchoredPosition;
        anchoredCreditsPanelPos = creditsPanel.anchoredPosition;

        levelSelectMenu.UpdateMenuState();

        resetProgress = debugInput.actions["ResetProgress"];
        resetProgress.Enable();
        resetProgress.started += UnlockAll;

        toggleEP = debugInput.actions["ToggleEctoplasm"];
        toggleEP.Enable();
        toggleEP.started += ToggleEP;
    }

    public void StartupMainMenu()
    {
        ShaderManager.instance.EnableShaders();
        if (!SaveData.ValidateSaveData(ProgramManager.instance.saveData))
        {
            OpenCorruptedSavePopup();
        }
        else
        {
            EnterStartScreen();
        }
    }

    public void OpenCorruptedSavePopup()
    {
        ShaderManager.instance.DisableShaders();
        corruptedSaveDataScreen.SetActive(true);
        MenuPanelWatcher.instance.activePanel = MenuPanel.POPUP;
        eventSystem.SetSelectedGameObject(corruptedSaveDataScreenFirstSelected.gameObject);
    }

    public void EnterStartScreen()
    {
        FindFirstObjectByType<AudioManager>().ChangeBGM(AudioManager.World.CURRENT, true);
        ScreenWipe.current.WipeOut();

        if (ProgramManager.instance.firstOpen && !ProgramManager.instance.saveData.SkipIntro)
        {
            MenuPanelWatcher.instance.activePanel = MenuPanel.START;
            startScreen.SetActive(true);
            eventSystem.SetSelectedGameObject(startScreenHiddenButton.gameObject);
        }
        else
        {
            EnterMainMenu();
        }
    }

    public void SyncLogos()
    {
        mainLogo.timer = startScreenLogo.timer;
    }

    //Sets up the input to allow standard menu interactions
    //  Seperated to allow boot splash, corrupted save popups, and start screen to exist in varying states without exploding logic
    public void EnterMainMenu()
    {
        ProgramManager.instance.firstOpen = false;

        mainPanel.gameObject.SetActive(true);
        startScreen.SetActive(false);

        //Just in case this needs changed last second
        levelSelectMenu.UpdateMenuState();

        MenuPanelWatcher.instance.activePanel = MenuPanel.MAIN;
        eventSystem.SetSelectedGameObject(mainPanelFirstSelected);
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
        levelSelectMenu.UpdateMenuState();
        //ResetProgress();
    }

    //Mostly duplicated ResetProgress so we could play a different sound :P
    public void OverwriteCorruptedSaveData()
    {
        soundPlayer.PlaySound(selectSound);

        ProgramManager.instance.ResetSaveData();
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();

        levelSelectMenu.UpdateMenuState();

        corruptedSaveDataScreen.SetActive(false);
        StartupMainMenu();
    }

    public void ResetProgress()
    {
        soundPlayer.PlaySound(backSound);
        //SettingsManager.completedLevels = 0;
        //SettingsManager.SaveSettings();
        ProgramManager.instance.ResetSaveData();
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();

        levelSelectMenu.UpdateMenuState();
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

    public void MoveToLevelSelectPanel()
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredLevelSelectPanelPos, tweenMoveTime).SetEase(tweenEaseType);
        soundPlayer.PlaySound(selectSound);

        MenuPanelWatcher.instance.activePanel = MenuPanel.LEVELS;
        levelSelectMenu.EnterLevelSelectMenu();
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

        
        //TODO: Fix this button DX
        //levelSelectMenu.UpdateMenuState();
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
        ShaderManager.instance.UpdatePaletteCondenseAmount(0);
        ShaderManager.instance.EnableShaders();
        ScreenWipe.current.WipeIn(true);
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
