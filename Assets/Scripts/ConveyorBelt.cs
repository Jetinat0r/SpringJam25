using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorBelt : MonoBehaviour
{
    public bool clockwise = true;
    public float speed = 1.0f;
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
        if (Time.time - startTime > 5f)
        {
            FlipBelt();
            startTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
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
        LevelManager.instance.conveyorTilemap.SetTile(LevelManager.instance.conveyorTilemap.WorldToCell(transform.position), clockwise ? cwTile : ccwTile);
    }
}
