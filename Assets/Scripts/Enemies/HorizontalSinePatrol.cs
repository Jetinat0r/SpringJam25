using UnityEngine;

public class HorizontalSinePatrol : MonoBehaviour
{
    private Vector3 initialPosition;
    [SerializeField]
    public float patrolDistanceFromCenter = 2.5f;
    [SerializeField]
    public float patrolPeriod = 2f;
    private float timeOnPatrol = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timeOnPatrol += Time.deltaTime;
        timeOnPatrol %= patrolPeriod;

        transform.position = initialPosition + (Vector3.right * (Mathf.Sin((timeOnPatrol / patrolPeriod) * (2f * Mathf.PI)) * patrolDistanceFromCenter));
    }
}
