using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetEngine;

[RequireComponent(typeof(Collider2D))]
public class Roomba : MonoBehaviour
{
    public float speed;
    public float radius;
    private Rigidbody2D EnemyRB;
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public bool facingRight;
    public bool isGrounded;
    private Vector3 home;
    public float rightDist;
    public float leftDist;
    public float spdBoost;
    public PhysicsMaterial2D friction, slippery;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        home = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelMenuManager.isMenuOpen) return;

        // If the player is resetting, pretend like it flipped when it "touches"
        if (PlayerMovement.instance != null && (LevelManager.isResetting || PlayerMovement.instance.hasWon))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), PlayerMovement.instance.GetComponent<Collider2D>());
            if (Vector2.Distance(PlayerMovement.instance.transform.position, transform.position) < 0.5)
            {
                Flip();
            }
        }

        //if(!isGrounded && facingRight)
        //{
        //    Flip();
        //}
        //else if(!isGrounded && !facingRight)
        //{
        //    Flip();
        //}

        // Ground/box push check
        isGrounded = false;
        spdBoost = 0;
        List<ContactPoint2D> contacts = new();
        EnemyRB.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            // TODO: Amend this check for other ground collidable objects
            if (contact.collider.gameObject.layer == 6 || contact.collider.gameObject.layer == 10 || contact.collider.gameObject.layer == 11)
            {
                if (Mathf.Abs((contact.normal - (transform.up.GetXY() * transform.localScale.y).normalized).magnitude) <= 0.001f)
                {
                    isGrounded = true;
                    if (contact.collider.gameObject.TryGetComponent(out ConveyorBelt belt))
                    {
                        if ((belt.clockwise && facingRight) || (!belt.clockwise && !facingRight))
                        {
                            spdBoost = belt.speed;
                        }
                        else if ((belt.clockwise && !facingRight) || (!belt.clockwise && facingRight))
                        {
                            spdBoost = -belt.speed / 2.0f;
                        }
                        break;
                    }
                }
            }
        }

        // Set physics material for proper friction behavior
        if (!isGrounded)
        {
            EnemyRB.sharedMaterial = slippery;
        }
        else
        {
            EnemyRB.sharedMaterial = friction;
        }

        Vector3 intendedPos;
        if (isGrounded)
        {
            if (facingRight)
            {
                intendedPos = transform.position + (speed + speed * spdBoost) * Time.fixedDeltaTime * Vector3.right;
                intendedPos.y = transform.position.y;
                EnemyRB.MovePosition(intendedPos);
                if ((transform.position.x - home.x) > rightDist)
                {
                    Flip();
                    //print("should flip");
                }
            }
            else
            {
                //intendedPos = transform.position + (speed + speed * spdBoost) * (isGrounded ? 1 : 0) * Time.fixedDeltaTime * Vector3.left;
                intendedPos = transform.position + (speed + speed * spdBoost) * Time.fixedDeltaTime * Vector3.left;
                intendedPos.y = transform.position.y;
                EnemyRB.MovePosition(intendedPos);
                if ((home.x - transform.position.x) > leftDist)
                {
                    Flip();
                }
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(new Vector3(0, 180, 0));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * rightDist));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.left * leftDist));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement.instance.Die();
        }
    }
}
