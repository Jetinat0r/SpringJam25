using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollOnSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    public FixedScrollArea scrollArea;

    [SerializeField]
    public int minScrollPos;
    [SerializeField]
    public int maxScrollPos;

    public void OnSelect(BaseEventData eventData)
    {
        bool _updatedScrollPos = false;
        if (scrollArea.scrollPos  < minScrollPos)
        {
            scrollArea.scrollPos = minScrollPos;
            _updatedScrollPos = true;
        }
        if (scrollArea.scrollPos > maxScrollPos)
        {
            scrollArea.scrollPos = maxScrollPos;
            _updatedScrollPos = true;
        }

        if (_updatedScrollPos)
        {
            scrollArea.UpdateScroll();
        }
    }
}
