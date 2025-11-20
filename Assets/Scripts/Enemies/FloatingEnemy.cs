//using UnityEngine;

//public class FloatingEnemy : MonoBehaviour
//{
//    public float moveSpeed = 0.5f; // Speed of movement
//    public float moveRadius = 0.3f; // Maximum range from the starting position

//    private Vector3 startPosition;
//    private Vector3 targetPosition;
//    private float changeTime = 2.0f;
//    private float timer;

//    void Start()
//    {
//        startPosition = transform.position;
//        SetNewTargetPosition();
//    }

//    void Update()
//    {
//        timer += Time.deltaTime;
//        if (timer >= changeTime)
//        {
//            SetNewTargetPosition();
//            timer = 0;
//        }

//        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
//    }

//    void SetNewTargetPosition()
//    {
//        targetPosition = startPosition + new Vector3(
//            Random.Range(-moveRadius, moveRadius),
//            Random.Range(-moveRadius, moveRadius),
//            Random.Range(-moveRadius, moveRadius)
//        );
//    }

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.tag == "Player")
//        {
//            //restart level
//        }
//    }
//}
using UnityEngine;

public class FloatingEnemy : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Speed of movement
    public float moveRadius = 0.3f; // Maximum range from the start position
    public float smoothTime = 1.5f; // Controls smooth movement
    public float rotationSpeed = 15f; // Speed of rotation

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        startPosition = transform.position;
        SetNewTargetPosition();
    }

    void Update()
    {
        // Smoothly move towards target
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, moveSpeed);

        // Small rotation for a floating effect
        //transform.Rotate(0, 0, Mathf.Sin(Time.time * rotationSpeed) * Time.deltaTime * 5f);
        //transform.position = new Vector3(transform.position.x, transform.position.y + (Mathf.Sin(Time.time * rotationSpeed) * 0.001f), transform.position.z);//.Rotate(0, 0, Mathf.Sin(Time.time * rotationSpeed) * Time.deltaTime * 5f);

        // Check if close to target, then pick a new one
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            SetNewTargetPosition();
        }
    }

    void SetNewTargetPosition()
    {
        targetPosition = startPosition + new Vector3(
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius),
            0 // Ensuring movement is only in 2D space
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Restart level logic
        }
    }
}