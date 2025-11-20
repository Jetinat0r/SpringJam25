using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static SaveData;


public class EditorManageSaveData : MonoBehaviour
{
    [MenuItem("PhantomFeline/SaveData/Open Save Data Location")]
    static void OpenSaveDataLocation()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath + "/");
    }

    [MenuItem("PhantomFeline/SaveData/Delete Save Data")]
    static void DeleteSaveData()
    {
        string _saveFilePath = Application.persistentDataPath + "/PF_FLASH.json";
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
            Debug.Log($"Deleted Save File: {_saveFilePath}");
        }
        else
        {
            Debug.Log($"Couldn't Delete Save File: {_saveFilePath}");
        }
    }

    [MenuItem("PhantomFeline/SaveData/Reset Custom Bindings")]
    static void ResetCustomBindings()
    {
        string _saveFilePath = Application.persistentDataPath + "/PF_FLASH.json";
        if (File.Exists(_saveFilePath))
        {
            try
            {
                RootSaveDataObject _saveData = JsonUtility.FromJson<RootSaveDataObject>(File.ReadAllText(_saveFilePath));
                _saveData.CustomBindings = string.Empty;
                string _jsonString = JsonUtility.ToJson(_saveData);
                File.WriteAllText(_saveFilePath, _jsonString);
                Debug.Log($"Reset Bindings");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.Log($"Couldn't Delete Save File: {_saveFilePath}");
        }
    }
}
