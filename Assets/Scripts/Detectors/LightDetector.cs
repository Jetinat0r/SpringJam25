using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class LightDetector : MonoBehaviour
{
    public delegate void LightStateChange();
    
    //Called when the collider enters 1 or more lights
    public LightStateChange onLightEnter;
    //Called when the collider was under 1 or more lights last physics tick, but is now under 0 lights
    public LightStateChange onLightExit;

    public List<Collider2D> activeLightCollisions = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool _broadcastEnterEvent = false;
        if (activeLightCollisions.Count == 0)
        {
            _broadcastEnterEvent = true;
        }

        if (!activeLightCollisions.Contains(collision) && !collision.CompareTag("Mirror"))
        {
            activeLightCollisions.Add(collision);

            if (_broadcastEnterEvent)
            {
                onLightEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activeLightCollisions.Contains(collision))
        {
            activeLightCollisions.Remove(collision);

            if (activeLightCollisions.Count == 0)
            {
                onLightExit?.Invoke();
            }
        }
    }
}
