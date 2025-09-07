using UnityEngine;
using UnityEngine.Events;

public class VentAnimEventTrigger : MonoBehaviour
{
    public UnityEvent onComplete;

    public void OnComplete()
    {
        Debug.Log("Event happens");
        onComplete?.Invoke();
    }
}
