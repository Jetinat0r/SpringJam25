using System;
using Unity.VisualScripting;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private float floatSpd;   // This will act as the frequency modulation for the sin wave
    [SerializeField] private float incrementFactor; // keyTimer will increase by this amount every frame.
    [SerializeField] private float amplitude;    // This is the amplitude of the sin wave.
    private float keyTimer = 0.0f;  // This will reset to 0 after it has made one full sinusoidal cycle
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
        sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, amplitude * Mathf.Sin(floatSpd * keyTimer) * Time.fixedDeltaTime, sprite.transform.localPosition.z);

        // Update key timer to make key move along the sin wave and reset to 0 if it has made a full wave to prevent overflow
        keyTimer += incrementFactor;

        if (keyTimer >= 2 * Mathf.PI)
        {
            keyTimer = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.hasKey = true;
            player.soundPlayer.PlaySound("Game.Key");
            Destroy(gameObject);
        }
    }
}
