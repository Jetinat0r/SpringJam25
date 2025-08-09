using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSnapToPlayerZone : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2Int zoneSize = new Vector2Int(10, 9);
    [SerializeField] private float scrollSpeed = 0.1f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Error: Camera needs a target to follow");
        }
    }

    private void Update()
    {
        // Only move screen in distinct zones, snap to current zone (2D Zelda)
        Vector3 targetPos = new Vector3(Mathf.RoundToInt(target.position.x / zoneSize.x) * zoneSize.x, Mathf.RoundToInt(target.position.y / zoneSize.y) * zoneSize.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, scrollSpeed);

    }
}
