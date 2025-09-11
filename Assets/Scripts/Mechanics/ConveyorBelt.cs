using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConveyorBelt : MonoBehaviour, IToggleable
{
    public bool clockwise = true;
    public int speed = 1;
    private Rigidbody2D rb;
    private EdgeCollider2D cldr;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [Header("Indicator Controllers")]
    [SerializeField] private RuntimeAnimatorController controllerCW;
    [SerializeField] private RuntimeAnimatorController controllerCCW;
    [SerializeField] private float indicatorTime = 1.0f;
    private Coroutine cr = null;

    [Header("Tiles")]
    [SerializeField] private TileBase[] cwTile, ccwTile;
    private float startTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<EdgeCollider2D>();
        startTime = Time.time;
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.enabled = false;
        UpdateTiles();
        LevelManager.instance.conveyorTilemap.RefreshAllTiles();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        List<ContactPoint2D> _contactPoints = new();
        rb.GetContacts(_contactPoints);

        foreach (ContactPoint2D _contact in _contactPoints)
        {
            if (Mathf.Abs((_contact.normal - Vector2.down).magnitude) <= 0.001f)
            {
                Rigidbody2D _collidingRb = _contact.collider.attachedRigidbody;
                if (_collidingRb != null && _collidingRb.bodyType != RigidbodyType2D.Static)
                {
                    if (_collidingRb.TryGetComponent(out PlayerMovement player))
                    {
                        if (!player.isShadow)
                            _collidingRb.MovePosition(_collidingRb.transform.position + (speed * Time.fixedDeltaTime * (clockwise ? Vector3.right : Vector3.left)) + Time.fixedDeltaTime * (Vector3)_collidingRb.linearVelocity);
                    }
                    else
                    {
                        _collidingRb.MovePosition(_collidingRb.transform.position + (speed * Time.fixedDeltaTime * (clockwise ? Vector3.right : Vector3.left)) + Time.fixedDeltaTime * (Vector3)_collidingRb.linearVelocity);
                        _collidingRb.linearVelocity = Vector2.zero;
                    }
                }
            }
        }

        /* TODO: This is the old method, in case we ever want to go back to it.
         * It worked in a much jankier way with friction, which ended up carrying momentum improperly
         */
        // Vector2 position = rb.position;
        // if (clockwise)
        // {
        //     rb.position += speed * Time.fixedDeltaTime * Vector2.left;
        // }
        // else
        // {
        //     rb.position += speed * Time.fixedDeltaTime * Vector2.right;
        // }
        // rb.MovePosition(position);
    }

    public void FlipBelt()
    {
        clockwise = !clockwise;
        UpdateTiles();

        // Swap indicator and display
        if (clockwise)
        {
            animator.runtimeAnimatorController = controllerCW;
        }
        else
        {
            animator.runtimeAnimatorController = controllerCCW;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            if (cr != null)
            {
                StopCoroutine(cr);
            }
            cr = StartCoroutine(HideIndicator(indicatorTime));
        }

    }

    private void UpdateTiles()
    {
        float pos = cldr.bounds.min.x + 0.25f;
        while (pos < cldr.bounds.max.x)
        {
            Vector2 worldPos = new(pos, transform.position.y);
            Vector3Int tilePos = LevelManager.instance.conveyorTilemap.WorldToCell(worldPos);

            if (clockwise)
                LevelManager.instance.conveyorTilemap.SetTile(tilePos, cwTile[speed-1]);
            else
                LevelManager.instance.conveyorTilemap.SetTile(tilePos, ccwTile[speed-1]);

            pos += 0.5f;
        }
    }

    private IEnumerator HideIndicator(float numSeconds)
    {
        yield return new WaitForSeconds(numSeconds);
        spriteRenderer.enabled = false;
    }

    public void OnToggle()
    {
        FlipBelt();
    }
}
