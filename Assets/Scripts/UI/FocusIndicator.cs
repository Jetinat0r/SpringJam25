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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;//FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        }

        if (focusTarget == null)
        {
            focusTarget = gameObject;
        }

        focusIndicator.SetActive(focusTarget == eventSystem.currentSelectedGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == focusTarget)
        {
            focusIndicator.SetActive(true);
        }
        else
        {
            focusIndicator.SetActive(false);
        }
    }
}
