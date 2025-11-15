using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuPanel
{
    MAIN, //Used for the main page of menu options
    LEVELS, //Used for the level select page and all world tabs
    SETTINGS, //Used for the settings page and all settings tabs
    INSTRUCTIONS, //Unused
    CREDITS, //Used for the credits page
    POPUP, //Used for any popup screen
    BOOT, //Used for the boot animation
    START //Used for the "PRESS [Z]/(A) Screen
}

public class MenuPanelWatcher : MonoBehaviour
{
    public static MenuPanelWatcher instance;
    public MenuPanel activePanel = MenuPanel.BOOT;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
