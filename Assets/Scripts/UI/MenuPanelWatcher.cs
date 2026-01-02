using UnityEngine;

public enum MenuPanel
{
    MAIN, //Used for the main page of menu options
    LEVELS, //Used for the level select page and all world tabs
    SETTINGS, //Used for the settings page and all settings tabs
    INSTRUCTIONS, //Unused
    CREDITS, //Used for the credits page
    POPUP, //Used for any popup screen
    BOOT, //Used for the boot animation
    START, //Used for the "PRESS [Z]/(A) Screen
    NONE //Used for level buttons you can't access
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
