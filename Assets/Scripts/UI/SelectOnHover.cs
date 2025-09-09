using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectOnHover : MonoBehaviour, IPointerMoveHandler
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
            if (EventSystem.current.currentSelectedGameObject != selectable.gameObject)
                MainMenuManager.menuSoundPlayer.PlaySound("UI.Move");
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            
            //selectable.Select();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        List<RaycastResult> _results = new List<RaycastResult>();
        PointerEventData _pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        if (EventSystem.current == null || _pointerEventData == null) return;
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
