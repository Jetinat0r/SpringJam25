using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine.InputSystem;
using System;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    //public static Settings currentSettings = null;

    public static int completedLevels = 0;
    public static float musicVolume = -10f;
    public static float sfxVolume = -6f;
    public static int resolution = 2;

    public TMP_Dropdown resolutionDropdown;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        LoadSettings();

        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
        resolutionDropdown.value = resolution;
    }

    public static void SetResolution(Int32 res)
    {
        resolution = res;
        if (FullScreenDetector.fullscreen) return;
        int x = 320 * (int)Math.Pow(2, res);
        int y = 288 * (int)Math.Pow(2, res);
#if !PLATFORM_WEBGL
        Screen.SetResolution(x, y, false);
#endif
    }

    public static void LoadSettings()
    {
        completedLevels = PlayerPrefs.GetInt("completedLevels", completedLevels);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        resolution = PlayerPrefs.GetInt("resolution", resolution);
        
        /*
        Settings newSettings = JsonUtility.FromJson<Settings>(File.ReadAllText(Application.persistentDataPath + "/" + fileName));
        currentSettings = newSettings;
        */

        //SetResolution(resolution);

        //Debug.Log($"LOADED: {completedLevels} {musicVolume} {sfxVolume} {resolution}");
    }

    public static void SaveSettings()
    {
        //Debug.Log($"SAVING: {completedLevels} {musicVolume} {sfxVolume} {resolution}");

        PlayerPrefs.SetInt("completedLevels", completedLevels);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetInt("resolution", resolution);

        PlayerPrefs.Save();
        /*
        string path = Application.persistentDataPath + "/" + fileName;
        string settingsJSON = JsonUtility.ToJson(currentSettings, true);

        File.WriteAllText(path, settingsJSON);
        Debug.Log("Saved settings to: " + path);
        */
    }

    public void UpdateResolution()
    {
        SetResolution(resolutionDropdown.value);
    }

}