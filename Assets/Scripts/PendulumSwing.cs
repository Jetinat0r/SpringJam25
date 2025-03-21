using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    public float swingAngle = 45f; // Maximum angle (degrees)
    public float swingSpeed = 2f;  // Speed of swinging motion

    private float startRotation;
    private float startTime;

    void Start()
    {
        startRotation = transform.rotation.z; // Store initial rotation
        startTime = Time.time; //Store initial time
    }

    void Update()
    {
        float angle = swingAngle * Mathf.Sin((Time.time - startTime) * swingSpeed); // Swing back and forth
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
