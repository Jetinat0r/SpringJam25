using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MenuPanel
{
    MAIN,
    LEVELS,
    SETTINGS,
    INSTRUCTIONS,
    CREDITS
}

public class MenuPanelWatcher : MonoBehaviour
{
    public static MenuPanelWatcher instance;
    public MenuPanel activePanel = MenuPanel.MAIN;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
