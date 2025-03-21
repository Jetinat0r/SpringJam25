using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{
    public float moveSpeed = 0.5f; // Speed of movement
    public float moveRadius = 0.3f; // Maximum range from the starting position

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float changeTime = 2.0f;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        SetNewTargetPosition();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeTime)
        {
            SetNewTargetPosition();
            timer = 0;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    void SetNewTargetPosition()
    {
        targetPosition = startPosition + new Vector3(
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius)
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //restart level
        }
    }
}
