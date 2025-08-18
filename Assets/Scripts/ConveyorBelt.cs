using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private bool clockwise = true;
    [SerializeField] private float speed = 1.0f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        // TODO: Player forces
        Vector2 position = rb.position;
        if (clockwise)
        {
            rb.position += Vector2.left * speed * Time.fixedDeltaTime;
            if (transform.localScale.x == -1) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            rb.position += Vector2.right * speed * Time.fixedDeltaTime;
            if (transform.localScale.x == 1) transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

        rb.MovePosition(position);
    }
}
