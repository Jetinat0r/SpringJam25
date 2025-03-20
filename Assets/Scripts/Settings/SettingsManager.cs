using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine.InputSystem;
using System;

public class SettingsManager : MonoBehaviour
{
    public static Settings currentSettings = null;

    public const string fileName = "Settings.txt";

    void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName) && currentSettings == null)
            LoadSettings();
        else if (currentSettings == null)
        {
            currentSettings = new Settings();
            SaveSettings();
        }
        
    }

    public static void LoadSettings()
    {
        Settings newSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + fileName));
        currentSettings = newSettings;
    }

    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + "/" + fileName;
        string settingsJSON = JsonUtility.ToJson(currentSettings, true);

        File.WriteAllText(path, settingsJSON);
        Debug.Log("Saved settings to: " + path);
    }
}