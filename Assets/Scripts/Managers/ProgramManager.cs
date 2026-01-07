using DG.Tweening;
using JetEngine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SaveData;
using static DemoData;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System.Collections;
using System.IO;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager instance;
    public SaveData.RootSaveDataObject saveData = null;
    public DemoData.RootDemoData demoData = null;
    //Determines whether or not to load the boot splash and start screen when Main Menu is entered
    //  Should only ever be true when the game opens, then false forever more
    public bool firstOpen = true;
    //Determines whether or not to display the challenge unlock at the end of the credits sequence
    public bool showChallengeUnlock = false;

    public Coroutine idleCoroutine = null;

    private StreamWriter curLogFile = null;
    private int writtenLines = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void InitDOTween()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
        //Debug.Log("DOTween Initialized");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AddGlobalManagers()
    {
        //Try to load Steam
        //  Returns false if Steam fails to load or if "-noSteam" is defined as a command line arg
        SteamUtils.TryInitSteam();

        GameObject _managerGameObject = new GameObject();

        _managerGameObject.name = "GLOBAL_MANAGERS";
        DontDestroyOnLoad(_managerGameObject);

        _managerGameObject.AddComponent<ProgramManager>();
        _managerGameObject.AddComponent<ShaderManager>();
        _managerGameObject.AddComponent<InputOverlord>();
        _managerGameObject.AddComponent<ChallengeManager>();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Load Demo Settings
        demoData = LoadDemoData();
        demoData.SaveDemoData();

        //Match Gameboy Framerate
        Application.targetFrameRate = 60;
        //Screen.SetResolution(1920, 1080, false);
        LoadSettings();
        //SaveSettings();
        CreateNewLogFile();
        WriteLineToLogFile("BOOT");
        SceneManager.sceneLoaded += OnSceneLoad;
        
    }

    private void OnSceneLoad(Scene _scene, LoadSceneMode _sceneMode)
    {
        WriteLineToLogFile($"SCENE,{_scene.name}");
    }

    private void OnApplicationQuit()
    {
        WriteLineToLogFile($"TERMINATED");
        curLogFile.Close();
        curLogFile = null;
    }


    private void Start()
    {
        //LoadSettings();
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("Game didn't start in Main Menu; disabling First Open logic!");
            firstOpen = false;
        }

        InputSystem.onAnyButtonPress.Call((_eventPtr) => ResetIdleRoutine());
        idleCoroutine = StartCoroutine(IdleCoroutine());
    }

    void ResetIdleRoutine()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
        }

        idleCoroutine = StartCoroutine(IdleCoroutine());
    }

    public IEnumerator IdleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(demoData.idleTimeoutSeconds);
            //Send to Idle Scene
            if (SceneManager.GetActiveScene().name != "IdleScene")
            {
                Time.timeScale = 1f;

                WriteLineToLogFile("IDLE");

                AudioManager.instance.CheckChangeWorlds("IdleScene");
                PlayerMovement.instance.soundPlayer.PlaySound("Game.Stairs");
                MainMenuManager.inMenu = true;
                ScreenWipe.current.WipeIn(() => SceneManager.LoadScene("IdleScene"));
            }
        }
    }

    public static FullScreenMode IndexToFullScreenMode(int _index)
    {
        if (_index < 0 || _index >= 3)
        {
            Debug.LogError($"Bad Fullscreen Mode Index: {_index}. Defaulting to Borderless.");
            _index = 0;
        }

        switch (_index)
        {
            case 0:
                return FullScreenMode.FullScreenWindow;
            case 1:
                return FullScreenMode.ExclusiveFullScreen;
            case 2:
                return FullScreenMode.Windowed;
            default:
                Debug.LogError($"SOMETHING HAS GONE HORRIBLY WRONG: {_index}");
                return FullScreenMode.FullScreenWindow;
        }
    }

    private void LoadSettings()
    {
        try
        {
            saveData = LoadSaveData();
            if (saveData == null)
            {
                Debug.LogWarning("Save data corrupted beyond repair, creating new save data!");
                saveData = LoadDefaultSaveData();
                saveData.PortPlayerPrefProgress();
            }

            RepairSettingsIfNecessary(saveData);

            if (!ValidateSaveData(saveData))
            {
                //TODO: Display a warning popup
                Debug.LogError("Save data failed to validate!");
            }
            else
            {
                //Save data is valid, apply display settings (audio settings get applied elsewhere
                if (saveData.DisplaySettings.fullScreenMode < 0 || saveData.DisplaySettings.fullScreenMode >= 3)
                {
                    saveData.DisplaySettings.fullScreenMode = 0;
                }

                Screen.fullScreenMode = IndexToFullScreenMode(saveData.DisplaySettings.fullScreenMode);
                QualitySettings.vSyncCount = saveData.DisplaySettings.vsync ? 1 : 0;
            }


        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void RepairSettingsIfNecessary(RootSaveDataObject _saveData)
    {
        _saveData.AudioSettings ??= new SaveData.AudioSettings();
        _saveData.DisplaySettings ??= new SaveData.DisplaySettings();

        //Unlock challenges if they should be unlocked but weren't for some reason
        if (_saveData.WorldSaveData != null)
        {
            for (int i = 0; i < _saveData.WorldSaveData.Length; i++)
            {
                if (_saveData.WorldSaveData[i] != null && _saveData.WorldSaveData[i].levels.Length >= 8 && _saveData.WorldSaveData[i].levels[7] != null && _saveData.WorldSaveData[i].levels[7].completed)
                {
                    _saveData.WorldSaveData[i].unlockedChallenges = true;
                }
            }
        }
    }

    //Transfer ALL transferrable settings between same version save datas
    private void TransferSettings(RootSaveDataObject _oldSaveData, RootSaveDataObject _newSaveData)
    {
        //Keep Game Settings
        _newSaveData.SkipIntro = _oldSaveData.SkipIntro;
        _newSaveData.UseCustomPalette = _oldSaveData.UseCustomPalette;

        //Keep Custom Bindings
        _newSaveData.CustomBindings = _oldSaveData.CustomBindings;

        //Keep Audio Volumes
        _newSaveData.AudioSettings.musicVolume = _oldSaveData.AudioSettings.musicVolume;
        _newSaveData.AudioSettings.sfxVolume = _oldSaveData.AudioSettings.sfxVolume;

        //Keep Resolution Settings
        _newSaveData.DisplaySettings.fullScreenMode = _oldSaveData.DisplaySettings.fullScreenMode;
        _newSaveData.DisplaySettings.vsync = _oldSaveData.DisplaySettings.vsync;

        //Keep Log Index
        _newSaveData.NextLogFileIndex = _oldSaveData.NextLogFileIndex;
    }

    public void SaveSettings()
    {
        saveData.SaveSaveData();
    }

    public void ResetSaveData()
    {
        RootSaveDataObject _defaultSaveData = SaveData.LoadDefaultSaveData();

        //Keep Settings
        TransferSettings(saveData, _defaultSaveData);

        saveData = _defaultSaveData;

        SaveSettings();
    }

    //This is for cheating! Ensure nothing can call this for full release!
    public void LoadFullCompleteSaveData()
    {
        RootSaveDataObject _fullCompleteSaveData = JsonUtility.FromJson<RootSaveDataObject>(Resources.Load<TextAsset>("FullCompleteSaveData").text);

        //Keep Settings
        TransferSettings(saveData, _fullCompleteSaveData);

        saveData = _fullCompleteSaveData;

        SaveSettings();
    }

    public string GetNewLogFileName()
    {
        return $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss")}_{saveData.NextLogFileIndex:00000}.csv";
    }

    public void CreateNewLogFile()
    {
        //Close current file if open
        curLogFile?.Close();
        writtenLines = 0;

        string _logFileDir = Application.persistentDataPath + "/MagfestLogs";
        if (!Directory.Exists(_logFileDir))
        {
            Directory.CreateDirectory(_logFileDir);
        }

        
        curLogFile = new StreamWriter(_logFileDir + "/" + GetNewLogFileName(), true, System.Text.Encoding.ASCII); ;
        saveData.NextLogFileIndex++;
        saveData.SaveSaveData();
    }

    public void WriteLineToLogFile(string _line)
    {
        //Just in case!
        if (curLogFile == null) { Debug.LogError("No Log File Open!");  return; }

        curLogFile.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss") + "," + _line);
        writtenLines++;

        if (writtenLines >= demoData.linesPerLogFile)
        {
            CreateNewLogFile();
        }
    }
}
