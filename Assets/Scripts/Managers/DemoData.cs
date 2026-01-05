using System;
using System.IO;
using UnityEngine;

public class DemoData
{
    public const string DEMO_DATA_FILE_NAME = "/PF_ROM.json";

    public static RootDemoData LoadDemoData()
    {
        string _saveDataPath = Application.persistentDataPath + DEMO_DATA_FILE_NAME;
        if (!File.Exists(_saveDataPath))
        {
            Debug.LogWarning("Demo data not found, creating new demo data!");
            RootDemoData _defaultSaveData = new RootDemoData();
            return _defaultSaveData;
        }

        Debug.Log($"Loaded demo data from {_saveDataPath}");
        try
        {
            return JsonUtility.FromJson<RootDemoData>(File.ReadAllText(_saveDataPath));
        }
        catch
        {
            Debug.LogWarning("Demo data corrupted beyond repair, creating new demo data!");
            RootDemoData _defaultSaveData = new RootDemoData();
            return _defaultSaveData;
        }
    }

    [Serializable]
    public class RootDemoData
    {
        [NonSerialized]
        private string[] LEVEL_NAMES =
        {
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "Level6",
            "Level7e",
            "Level8",

            "Level9",
            "Level10",
            "Level11b",
            "Level12",
            "Level13",
            "Level14b",
            "Level16b",
            "Level15",

            "Level17",
            "Level18",
            "Level19b",
            "Level20",
            "Level21b",
            "Level22",
            "Level23",
            "Level24",

            "Level25",
            "Level26",
            "Level27",
            "Level28",
            "Level29",
            "Level30",
            "Level31",
            "Level32",
        };

        public float idleTimeoutSeconds = 180f;
        public int[] demoLevels = { 0 };
        public string idleVideoName = "IdleVideo.webm";

        public bool IsLastPlayableLevel(int _curLevelNumber)
        {
            return demoLevels[^1] == _curLevelNumber;
        }

        public string GetNextLevelName(int _curLevelNumber)
        {
            if (IsLastPlayableLevel(_curLevelNumber))
            {
                return "TitleLevel";
            }

            for (int i = 0; i < demoLevels.Length - 1; i++)
            {
                if (demoLevels[i] == _curLevelNumber)
                {
                    return LEVEL_NAMES[demoLevels[i + 1] - 1];
                }
            }

            Debug.LogError("Failed to find proper next level! Defaulting to Title Level!");
            return "TitleLevel";
        }

        public void SaveDemoData()
        {
            string _saveDataPath = Application.persistentDataPath + DEMO_DATA_FILE_NAME;

            string _jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(_saveDataPath, _jsonString);

            Debug.Log($"Saved Demo Data to {_saveDataPath}");
        }
    }
}
