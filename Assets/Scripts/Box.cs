using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool grounded = false;
    [SerializeField] private PhysicsMaterial2D slippery, friction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Stop moving horizontally if not on ground
        grounded = false;
        List<ContactPoint2D> contacts = new();
        rb.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            // TODO: Amend this check for other ground collidable objects
            if (contact.collider.gameObject.layer == 6 || contact.collider.gameObject.layer == 10 || contact.collider.gameObject.layer == 11)
            {
                if (Mathf.Abs((contact.normal - Vector2.up).magnitude) <= 0.001f)
                {
                    grounded = true;
                }
            }
        }
        if (!grounded && rb.constraints != (RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation))
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            rb.sharedMaterial = slippery;
        }
        else if (grounded && rb.constraints != RigidbodyConstraints2D.FreezeRotation)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.sharedMaterial = friction;
        }
    }
}
