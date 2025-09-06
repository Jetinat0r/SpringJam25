using UnityEngine;
using System.Collections;

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

    Animator myAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnemyRB = GetComponent<Rigidbody2D>();
        home = transform.position;

        myAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelMenuManager.isMenuOpen) return;

        Vector3 intendedPos;
        if (facingRight)
        {
            intendedPos = transform.position + speed * Time.deltaTime * Vector3.right;
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
            intendedPos = transform.position + speed * Time.deltaTime * Vector3.left;
            intendedPos.y = transform.position.y;
            EnemyRB.MovePosition(intendedPos);
            if ((home.x - transform.position.x) > leftDist)
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
