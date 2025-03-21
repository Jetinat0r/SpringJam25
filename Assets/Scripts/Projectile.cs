using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public int damage = 10;

    private Vector3 targetDirection;

    void Start()
    {
        //grab player,
       // targetDirection = (FindObjectOfType<PlayerController>().transform.position - transform.position).normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += targetDirection * speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //restart level
        }
    }
}
