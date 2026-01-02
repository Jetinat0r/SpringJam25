using UnityEngine;

public enum LightDirection : int
{
    DOWN = 0,
    LEFT = 1,
    UP = 2,
    RIGHT = 3
}

public class MirrorableLight : MonoBehaviour
{
    public LightDirection lightDir = LightDirection.DOWN;
    public bool calculateLightDirOnStart = true;
    public Mirror parentMirror = null;

    private void Start()
    {
        //Allow manual overrides if desired
        if (!calculateLightDirOnStart)
        {
            return;
        }

        // TODO: Only works with long lights, not circle lights.
        float zRotation = transform.eulerAngles.z % 360;

        if (zRotation >= 315 || zRotation <= 45)
        {
            lightDir = LightDirection.DOWN;
        }
        else if (zRotation >= 45 && zRotation <= 135)
        {
            lightDir = LightDirection.RIGHT;
        }
        else if (zRotation >= 135 && zRotation <= 225)
        {
            lightDir = LightDirection.UP;
        }
        else if (zRotation >= 225 && zRotation <= 315)
        {
            lightDir = LightDirection.LEFT;
        }
        else
        {
            Debug.LogWarning("Warning: Direction of held light has not been set.\nZ-rotation of object: " + zRotation);
        }
    }
}
