using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField] private float floatSpd;   // This will act as the frequency modulation for the sin wave
    [SerializeField] private float incrementFactor; // timer will increase by this amount every frame.
    [SerializeField] private float amplitude;    // This is the amplitude of the sin wave.
    private float timer = 0.0f;  // This will reset to 0 after it has made one full sinusoidal cycle
    private SpriteRenderer sprite;
    private CapsuleCollider2D pickupCollider;

    private void Start()
    {
        // Get components
        sprite = GetComponentInChildren<SpriteRenderer>();
        pickupCollider = GetComponentInChildren<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (LevelMenuManager.isMenuOpen) return;

        // Make sprite float
        sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, amplitude * Mathf.Sin(floatSpd * timer) * Time.fixedDeltaTime, sprite.transform.localPosition.z);

        // Update timer to make collectible move along the sin wave and reset to 0 if it has made a full wave to prevent overflow
        timer += incrementFactor;

        if (timer >= 2 * Mathf.PI)
        {
            timer = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You are being touched.");
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("You are about to be collected");
            OnCollected(collision);
            Destroy(gameObject);
        }
    }

    protected abstract void OnCollected(Collider2D collision);
}
