using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    private const int maxNumKeys = 4;
    public Key[] myKeys = new Key[maxNumKeys];  // Fixed size in inspector. Will always denote the keys required by the door.
    [HideInInspector] public List<Key> remainingKeys;   // A mutable copy of the array as a list. Once the length of this list is 0, the door may be opened.
    
    [Tooltip("Ensure each sprite is indexed by the number of keys remaining needed to show it")]
    public Sprite[] sprites = new Sprite[maxNumKeys + 1];   // An array of sprites that dictate what the door will look like for every number of keys remaining
    
    private PlayerMovement playerScript = null;
    private SpriteRenderer spriteRenderer = null;

    [SerializeField] private GameObject keyholeVFX; // The prefab containing a sprite renderer and animator for the visual effect that plays upon keyhole removal

    private void Start()
    {
        remainingKeys = myKeys.ToList();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        UpdateKeyholes(false);

        Debug.Log(remainingKeys.Count);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (myKeys.Length > maxNumKeys)
        {
            Debug.LogError("Error: A door of this type may only have a maximum of " + maxNumKeys + " keys.");
            myKeys = new Key[maxNumKeys];
        }

        if (sprites.Length != maxNumKeys + 1)
        {
            Debug.LogError("Error: Attempted to create a mismatch in the number of sprites and the maximum allowable number of keys (" + maxNumKeys + ")");
            sprites = new Sprite[maxNumKeys + 1];
        }
    }
#endif

    public void MyInteraction()
    {
        // TODO: Talk with some scene manager or initiate some UI thing to initiate level complete
        if (remainingKeys.Count == 0)
        {
            //Debug.Log("Level Complete!");
            playerScript.Win();

            playerScript.OnInteract -= MyInteraction;
        }
        else
        {
            // Play a sound or something maybe
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Spawn interaction prompt over player head
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Entered");

            // This is where you'd summon the interact button prompt if we want to make it appear over their head regardless of whether or not they have a key
            PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            if (_playerScript != null)
            {
                _playerScript.OnInteract -= MyInteraction;
                _playerScript.OnInteract += MyInteraction;
                playerScript = _playerScript;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Exited");

            collision.gameObject.GetComponent<PlayerMovement>().OnInteract -= MyInteraction;
            playerScript = null;
        }
    }

    // This function changes the sprite depending on how many remaining keys are left to find
    public void UpdateKeyholes(bool showVFX)
    {
        spriteRenderer.sprite = sprites[remainingKeys.Count];

        if (showVFX)
        {
            var vfx1 = Instantiate(keyholeVFX, transform);
            vfx1.transform.localPosition = new Vector3(-0.25f, 0.25f, 0);
            var vfx2 = Instantiate(keyholeVFX, transform);
            vfx2.transform.localPosition = new Vector3(0.25f, -0.25f, 0);
        }
    }
}
