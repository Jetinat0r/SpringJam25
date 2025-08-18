using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float fallThreshold = -0.01f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Stop moving horizontally if falling
        if (rb.linearVelocityY < fallThreshold)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}
