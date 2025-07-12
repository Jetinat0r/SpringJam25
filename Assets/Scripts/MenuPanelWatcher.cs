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

    // Update is called once per frame
    void Update()
    {
        List<RaycastResult> _results = new List<RaycastResult>();
        PointerEventData _pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(_pointerEventData, _results);
        //_results.Sort((a, b) => (a.sortingOrder > b.sortingOrder) ? -1 : (a.sortingOrder < b.sortingOrder) ? 1 : 0);
        _results.Sort((a, b) => (a.index > b.index) ? -1 : (a.index < b.index) ? 1 : 0);
        //_results.Reverse();
        foreach (var r in _results)
        {
            if (r.gameObject && r.gameObject.TryGetComponent(out SelectOnHover _selectable))
            {
                //Debug.Log(r.gameObject.name);
                _selectable.TrySelect();
                return;
            }
        }
    }
}
