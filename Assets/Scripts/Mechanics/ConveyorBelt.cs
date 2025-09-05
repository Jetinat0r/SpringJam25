using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorBelt : MonoBehaviour, IToggleable
{
    public bool clockwise = true;
    public float speed = 1.0f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] private TileBase cwTile, ccwTile;
    private float startTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        startTime = Time.time;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector2 position = rb.position;
        if (clockwise)
        {
            rb.position += speed * Time.fixedDeltaTime * Vector2.left;
        }
        else
        {
            rb.position += speed * Time.fixedDeltaTime * Vector2.right;
        }
        rb.MovePosition(position);
    }

    public void FlipBelt()
    {
        clockwise = !clockwise;
        float pos = boxCollider.bounds.min.x + 0.25f;
        while (pos < boxCollider.bounds.max.x)
        {
            Vector2 worldPos = new(pos, transform.position.y);
            Vector3Int tilePos = LevelManager.instance.conveyorTilemap.WorldToCell(worldPos);

            if (clockwise)
                LevelManager.instance.conveyorTilemap.SetTile(tilePos, cwTile);
            else
                LevelManager.instance.conveyorTilemap.SetTile(tilePos, ccwTile);

            pos += 0.5f;
        }
    }

    public void OnToggle()
    {
        FlipBelt();
    }
}
