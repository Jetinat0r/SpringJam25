using System.Linq;
using System.IO;
using UnityEngine;
using System;
using NUnit.Framework;

public class SaveData
{
    public const string SAVE_DATA_FILE_NAME = "/PF_FLASH.json";

    public static RootSaveDataObject LoadSaveData()
    {
        string _saveDataPath = Application.persistentDataPath + SAVE_DATA_FILE_NAME;
        if (!File.Exists(_saveDataPath))
        {
            Debug.LogWarning("Save data not found, creating new save data!");
            RootSaveDataObject _defaultSaveData = LoadDefaultSaveData();
            _defaultSaveData.PortPlayerPrefProgress();
            return _defaultSaveData;
        }

        Debug.Log($"Loaded save data from {_saveDataPath}");
        try
        {
            return JsonUtility.FromJson<RootSaveDataObject>(File.ReadAllText(_saveDataPath));
        }
        catch
        {
            Debug.LogWarning("Save data corrupted beyond repair, creating new save data!");
            RootSaveDataObject _defaultSaveData = LoadDefaultSaveData();
            _defaultSaveData.PortPlayerPrefProgress();
            return _defaultSaveData;
        }
    }

    public static RootSaveDataObject LoadDefaultSaveData()
    {
        //Debug.Log(Resources.Load<TextAsset>("DefaultSaveData").text);
        return JsonUtility.FromJson<RootSaveDataObject>(Resources.Load<TextAsset>("DefaultSaveData").text);
    }

    //Ensure save data object is valid and has not been irreparably tampered with
    public static bool ValidateSaveData(RootSaveDataObject _saveData)
    {
        //Ensure the save data exists in the first place
        if (_saveData == null)
        {
            return false;
        }

        //Ensure audio settings exist
        if (_saveData.AudioSettings == null)
        {
            return false;
        }

        //Ensure Resolution settings exist
        if (_saveData.DisplaySettings == null)
        {
            return false;
        }

        //Ensure all world save data exists
        if (_saveData.WorldSaveData == null || _saveData.WorldSaveData.Length < 4)
        {
            return false;
        }

        int j = 0;
        for (int i = 0; i < 4; i++)
        {
            //Ensure individual world save data exists
            WorldSaveData w = _saveData.WorldSaveData[i];
            if (w == null || w.worldNumber != i + 1)
            {
                return false;
            }

            //Ensure level save data exists
            if (w.levels == null || w.levels.Length < 8)
            {
                return false;
            }

            //Ensure individual level save data exists
            for (int k = 0; k < 8; k++)
            {
                if (w.levels[k] == null || w.levels[k].levelNumber != j + 1)
                {
                    return false;
                }

                //Ensure challenge save data exists
                if (w.levels[k].challenges == null)
                {
                    return false;
                }

                j++;
            }
        }

        return true;
    }

    [Serializable]
    public class RootSaveDataObject
    {
        //Track Save data revision for automatic replacement / upgrading
        public int SaveDataVersion = -1;
        public AudioSettings AudioSettings;
        public DisplaySettings DisplaySettings;
        public WorldSaveData[] WorldSaveData;
        public string CustomBindings = "";
        public int LastPlayedLevel = 1;
        //Defaults to true, but is treated as false if no custom palette is found
        public bool UseCustomPalette = true;
        public bool SkipIntro = false;

        //Expects world number 1 indexed
        public WorldSaveData GetWorldSaveData(int _worldNumber)
        {
            if (_worldNumber <= 0 || _worldNumber > 4)
            {
                return null;
            }

            return WorldSaveData[_worldNumber - 1];
        }

        public LevelSaveData GetLevelSaveData(int _levelNumber)
        {
            if (_levelNumber <= 0 || _levelNumber > 32)
            {
                return null;
            }

            _levelNumber -= 1;
            return WorldSaveData[_levelNumber / 8].levels[_levelNumber % 8];
        }

        public int GetNumCompletedLevels()
        {
            for (int i = 0; i < WorldSaveData.Length; i++)
            {
                for (int j = 0; j <  WorldSaveData[i].levels.Length; j++)
                {
                    if (WorldSaveData[i].levels[j].completed == false)
                    {
                        return WorldSaveData[i].levels[j].levelNumber - 1;
                    }
                }
            }

            return 32;
        }

        public void SaveSaveData()
        {
            string _saveDataPath = Application.persistentDataPath + SAVE_DATA_FILE_NAME;

            string _jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(_saveDataPath, _jsonString);

            Debug.Log($"Saved Data to {_saveDataPath}");
        }

        //Attempts to upgrade save data from PlayerPrefs to the new Json format
        public void PortPlayerPrefProgress()
        {
            /*
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                AudioSettings.musicVolume = PlayerPrefs.GetFloat("musicVolume");
            }

            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                AudioSettings.sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            }

            if (PlayerPrefs.HasKey("completedLevels"))
            {
                int _numCompletedLevels = PlayerPrefs.GetInt("completedLevels");
                
            }
            */
        }
    }

    [Serializable]
    public class AudioSettings
    {
        public float musicVolume = 32;
        public float sfxVolume = 50;
    }

    [Serializable]
    public class DisplaySettings
    {
        public int fullScreenMode = 0;
        public bool vsync = true;
    }

    [Serializable]
    public class ChallengeSaveData
    {
        public bool beatEctoplasm;
        public bool beatLightsOut;
        public bool beatSpectralShuffle;
        public bool beatPoltergeist;
    }

    [Serializable]
    public class WorldSaveData
    {
        public int worldNumber;
        public bool unlockedChallenges = false;
        public LevelSaveData[] levels;
        public float fastestTime;

        public LevelSaveData GetLevelSaveData(int _levelNumber)
        {
            return levels.First(y => y.levelNumber == _levelNumber);
        }
    }

    [Serializable]
    public class LevelSaveData
    {
        public int levelNumber;
        public bool completed;
        public float fastestTime;
        public ChallengeSaveData challenges;
    }
}
