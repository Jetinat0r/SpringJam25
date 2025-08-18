using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private bool clockwise = true;
    [SerializeField] private float speed = 1.0f;
    private Rigidbody2D rb;
    [SerializeField] private TileBase cwTile, ccwTile;
    private float startTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time;
    }

    private void Update()
    {
        // TODO: Remove, only used for testing
        if (Time.time - startTime > 1f)
        {
            FlipBelt();
            startTime = Time.time;
        }
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

    public void FlipBelt()
    {
        clockwise = !clockwise;
        LevelManager.instance.tilemap.SetTile(LevelManager.instance.tilemap.WorldToCell(transform.position), clockwise ? cwTile : ccwTile);
    }
}
