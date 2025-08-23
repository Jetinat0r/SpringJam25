using UnityEngine;

public class CustomLight : MonoBehaviour
{
    public Vector2 lightDirection = Vector2.down;

    private void Start()
    {
        // TODO: Only works with long lights, not circle lights.
        float zRotation = transform.eulerAngles.z % 360;

        if (zRotation >= 315 || zRotation <= 45)
        {
            lightDirection = Vector2.down;
        }
        else if (zRotation >= 45 && zRotation <= 135)
        {
            lightDirection = Vector2.right;
        }
        else if (zRotation >= 135 && zRotation <= 225)
        {
            lightDirection = Vector2.up;
        }
        else if (zRotation >= 225 && zRotation <= 315)
        {
            lightDirection = Vector2.left;
        }
        else
        {
            Debug.LogWarning("Warning: Direction of held light has not been set.\nZ-rotation of object: " + zRotation);
        }
    }
}
