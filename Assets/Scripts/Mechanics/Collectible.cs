using System.Collections;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    [SerializeField] private float floatSpd;   // This will act as the frequency modulation for the sin wave
    [SerializeField] private float incrementFactor; // timer will increase by this amount every frame.
    [SerializeField] protected float amplitude;    // This is the amplitude of the sin wave.
    private float timer = 0.0f;  // This will reset to 0 after it has made one full sinusoidal cycle
    protected SpriteRenderer sprite;
    private Vector3 initialSpritePosition;
    private CapsuleCollider2D pickupCollider;
    //[SerializeField] private VFX pickupVFX;
    //[SerializeField] private VFX ambientVFX;
    [SerializeField] private string pickupSFXPath = "Game.Key";
    [SerializeField] protected bool enableAmbientVFX = true;
    //private Coroutine cr1 = null;
    //private Coroutine cr2 = null;

    private void Start()
    {
        // Get components
        sprite = GetComponentInChildren<SpriteRenderer>();
        initialSpritePosition = sprite.transform.localPosition;
        pickupCollider = GetComponentInChildren<CapsuleCollider2D>();

        /*
        if (enableAmbientVFX)
        {
            BeginHandleAmbientVFX();
        }
        */
    }

    /*
    private void OnDestroy()
    {
        if (cr1 != null)
        {
            StopCoroutine(cr1);
            cr1 = null;
        }
        if (cr2 != null)
        {
            StopCoroutine(cr2);
            cr2 = null;
        }
    }
    */

    private void FixedUpdate()
    {
        //if (LevelMenuManager.isMenuOpen) return;

        // Update timer to make collectible move along the sin wave and reset to 0 if it has made a full wave to prevent overflow
        timer += incrementFactor * Time.fixedDeltaTime;

        // Make sprite float
        sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, amplitude * Mathf.Sin(floatSpd * timer) * Time.fixedDeltaTime, sprite.transform.localPosition.z) + initialSpritePosition;


        if (timer >= 2f * Mathf.PI)
        {
            timer %= 2f * Mathf.PI;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("You are being touched.");
        if (collision.gameObject.CompareTag("Player"))
        {
            // Debug.Log("You are about to be collected");
            OnCollected(collision);
            //Destroy(gameObject);
        }
    }

    protected abstract void OnCollected(Collider2D collision);

    protected virtual void PlayPickupSFX(PlayerMovement playerRef)
    {
        if (playerRef == null) return;

        playerRef.soundPlayer.PlaySound(pickupSFXPath);
    }

    /*
    protected virtual void BeginHandleAmbientVFX()
    {
        cr1 = StartCoroutine(AmbientVFXLoop(new Vector3(-0.25f, 0.25f, 0)));
        cr2 = StartCoroutine(AmbientVFXLoop(new Vector3(0.25f, -0.25f, 0)));
    }

    protected virtual void DisplayPickupVFX()
    {
        var vfx1 = Instantiate(pickupVFX);
        vfx1.transform.localPosition = transform.localPosition + new Vector3(-0.25f, 0.25f, 0);
        var vfx2 = Instantiate(pickupVFX);
        vfx2.transform.localPosition = transform.localPosition + new Vector3(0.25f, -0.25f, 0);
    }

    protected IEnumerator AmbientVFXLoop(Vector3 pos)
    {
        while (true)
        {
            float delay = Random.Range(2.0f, 3.0f); // random interval each loop
            yield return new WaitForSeconds(delay);

            var vfx = Instantiate(ambientVFX, transform);
            vfx.transform.localPosition = pos;
        }
    }
    */
}
