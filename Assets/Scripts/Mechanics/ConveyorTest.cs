using UnityEngine;

public class ConveyorTest : MonoBehaviour
{
    private Vector3 pos;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(1f, 0);
        transform.position = pos;
    }
}
