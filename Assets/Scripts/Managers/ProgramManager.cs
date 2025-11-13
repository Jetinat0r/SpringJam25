using DG.Tweening;
using JetEngine;
using System;
using UnityEngine;
using static SaveData;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager instance;
    public SaveData.RootSaveDataObject saveData = null;

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

        //Match Gameboy Framerate
        Application.targetFrameRate = 60;
        LoadSettings();
        //SaveSettings();
    }

    private void Start()
    {
        //LoadSettings();
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
                Debug.LogError("Couldn't Load Existing or Default Save Data! Very Bad!");
                return;
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
    }

    public void SaveSettings()
    {
        saveData.SaveSaveData();
    }

    public void ResetSaveData()
    {
        RootSaveDataObject _defaultSaveData = SaveData.LoadDefaultSaveData();

        //Keep Settings
        TransferSettings(_defaultSaveData, saveData);

        saveData = _defaultSaveData;

        SaveSettings();
    }

    //This is for cheating! Ensure nothing can call this for full release!
    public void LoadFullCompleteSaveData()
    {
        RootSaveDataObject _fullCompleteSaveData = JsonUtility.FromJson<RootSaveDataObject>(Resources.Load<TextAsset>("FullCompleteSaveData").text);

        //Keep Settings
        TransferSettings(_fullCompleteSaveData, saveData);

        saveData = _fullCompleteSaveData;

        SaveSettings();
    }
}
