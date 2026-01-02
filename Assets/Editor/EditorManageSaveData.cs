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
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            Debug.Log($"Deleted Save");
        }
        else
        {
            Debug.Log($"Couldn't Delete Save");
        }
    }

    [MenuItem("PhantomFeline/SaveData/Reset Custom Bindings")]
    static void ResetCustomBindings()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            try
            {
                RootSaveDataObject _saveData = JsonUtility.FromJson<RootSaveDataObject>(PlayerPrefs.GetString(SAVE_KEY));
                _saveData.CustomBindings = string.Empty;
                string _jsonString = JsonUtility.ToJson(_saveData);
                PlayerPrefs.SetString(SAVE_KEY, _jsonString);
                Debug.Log($"Reset Bindings");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else
        {
            Debug.Log($"Couldn't Delete Save");
        }
    }
}
