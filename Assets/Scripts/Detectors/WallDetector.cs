using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class WallDetector : MonoBehaviour
{
    public delegate void WallStateChange();
    
    //Called when the collider enters 1 or more lights
    public WallStateChange onWallEnter;
    //Called when the collider was under 1 or more lights last physics tick, but is now under 0 lights
    public WallStateChange onWallExit;

    public List<Collider2D> activeWallCollisions = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool _broadcastEnterEvent = false;
        if (activeWallCollisions.Count == 0)
        {
            _broadcastEnterEvent = true;
        }

        if (!activeWallCollisions.Contains(collision))
        {
            activeWallCollisions.Add(collision);

            if (_broadcastEnterEvent)
            {
                onWallEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (activeWallCollisions.Contains(collision))
        {
            activeWallCollisions.Remove(collision);

            if (activeWallCollisions.Count == 0)
            {
                onWallExit?.Invoke();
            }
        }
    }
}
