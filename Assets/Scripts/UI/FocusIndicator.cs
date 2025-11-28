using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusIndicator : MonoBehaviour
{
    [SerializeField]
    public EventSystem eventSystem;
    [SerializeField]
    public GameObject focusIndicator;
    [SerializeField]
    public GameObject focusTarget;
    private EventTrigger eventTrigger;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;//FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        }

        if (focusTarget == null)
        {
            //This doesn't work? Lmao it'll just disable itself and become unusable
            focusTarget = gameObject;
        }

        focusIndicator.SetActive(focusTarget == eventSystem.currentSelectedGameObject);
        eventTrigger = focusTarget.AddComponent<EventTrigger>();

        EventTrigger.Entry _onSelect = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.Select
        };
        _onSelect.callback.AddListener(OnSelect);
        eventTrigger.triggers.Add(_onSelect);

        EventTrigger.Entry _onDeselect = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.Deselect
        };
        _onDeselect.callback.AddListener(OnDeselect);
        eventTrigger.triggers.Add(_onDeselect);
    }

    private void OnDestroy()
    {
        //This happens for the dropdown menu, for whatever reason
        if (eventTrigger == null || eventTrigger.triggers == null)
        {
            return;
        }

        if (eventTrigger.triggers.Count > 0)
        {
            eventTrigger.triggers.RemoveRange(0, eventTrigger.triggers.Count);
        }
    }

    public void OnSelect(BaseEventData _eventData)
    {
        focusIndicator.SetActive(true);
    }

    public void OnDeselect(BaseEventData _eventData)
    {
        focusIndicator.SetActive(false);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        //TODO: Swap out for adding an EventTrigger to focus target and targeting OnSelect and OnDeselect
        if (eventSystem.currentSelectedGameObject == focusTarget)
        {
            focusIndicator.SetActive(true);
        }
        else
        {
            focusIndicator.SetActive(false);
        }
    }
    */
}
