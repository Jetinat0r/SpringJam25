using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static Settings currentSettings = null;

    public const string fileName = "Settings.txt";

    public TMP_Dropdown resolutionDropdown;

    void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/" + fileName) && currentSettings == null)
            LoadSettings();
        else if (currentSettings == null)
        {
            currentSettings = new Settings();
            SaveSettings();
        }

        resolutionDropdown.value = currentSettings.resolution;
    }

    public static void SetResolution(Int32 res)
    {
        int x = 320 * (int)Math.Pow(2, res);
        int y = 288 * (int)Math.Pow(2, res);
        Screen.SetResolution(x, y, false);
        currentSettings.resolution = res;
    }

    public static void LoadSettings()
    {
        Settings newSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + fileName));
        currentSettings = newSettings;

        SetResolution(currentSettings.resolution);
    }

    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + "/" + fileName;
        string settingsJSON = JsonUtility.ToJson(currentSettings, true);

        File.WriteAllText(path, settingsJSON);
        Debug.Log("Saved settings to: " + path);
    }

    public void UpdateResolution()
    {
        SetResolution(resolutionDropdown.value);
    }

}