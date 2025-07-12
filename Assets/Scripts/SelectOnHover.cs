using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectOnHover : MonoBehaviour
{
    [SerializeField]
    public MenuPanel parentPanel = MenuPanel.MAIN;
    [SerializeField]
    public Selectable selectable;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!selectable)
        {
            if(!TryGetComponent(out selectable))
            {
                selectable = GetComponentInParent<Selectable>();
            }
        }
    }

    public void TrySelect()
    {
        if (selectable && MenuPanelWatcher.instance.activePanel == parentPanel)
        {
            selectable.Select();
        }
    }
}
