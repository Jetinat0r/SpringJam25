using UnityEngine;
using UnityEngine.Events;

public class AsepriteAnimEventTrigger : MonoBehaviour
{
    public UnityEvent onComplete;

    public void OnComplete()
    {
        onComplete?.Invoke();
    }
}
