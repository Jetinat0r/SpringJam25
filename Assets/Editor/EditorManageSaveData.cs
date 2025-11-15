using System.IO;
using UnityEditor;
using UnityEngine;


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
}
