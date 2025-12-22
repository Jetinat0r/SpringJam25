using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetEngine;

public class VerrticalRoomba : MonoBehaviour
{
    public float speed;
    private Rigidbody2D EnemyRB;
    public bool facingUp;
    private Vector3 home;
    public float upDist;
    public float downDist;

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

        //if(!isGrounded && facingRight)
        //{
        //    Flip();
        //}
        //else if(!isGrounded && !facingRight)
        //{
        //    Flip();
        //}


        Vector3 intendedPos;
        if (facingUp)
        {
            intendedPos = transform.position + (speed) * Time.fixedDeltaTime * Vector3.up;
            intendedPos.x = transform.position.x;
            EnemyRB.MovePosition(intendedPos);
            if ((transform.position.y - home.y) > upDist)
            {
                Flip();
                //print("should flip");
            }
        }
        else
        {
            //intendedPos = transform.position + (speed + speed * spdBoost) * (isGrounded ? 1 : 0) * Time.fixedDeltaTime * Vector3.left;
            intendedPos = transform.position + (speed) * Time.fixedDeltaTime * Vector3.down;
            intendedPos.x = transform.position.x;
            EnemyRB.MovePosition(intendedPos);
            if ((home.y - transform.position.y) > downDist)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        facingUp = !facingUp;
        //transform.Rotate(180, 0, 0);
        Vector3 _scale = transform.localScale;
        _scale.Scale(new Vector3(-1, 1, 1));
        transform.localScale = _scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * upDist));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * downDist));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement.instance.Die();
        }
    }
}
