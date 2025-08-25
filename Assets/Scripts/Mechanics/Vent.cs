using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Vent : MonoBehaviour, IToggleable
{
    public bool isWallVent = true;
    public bool isOpen = false;
    public Vent counterpart;

    private BoxCollider2D objectTrigger = null;
    private CircleCollider2D interactionRadius = null;
    private GameObject player = null;

    private void Start()
    {
        objectTrigger = GetComponent<BoxCollider2D>();
        interactionRadius = GetComponent<CircleCollider2D>();
        player = GameObject.FindWithTag("Player");
    }

    public void OnToggle()
    {
        // TODO: Animate the vent opening/closing

        isOpen = !isOpen;
    }

    public void MyInteraction()
    {
        if (counterpart != null)
        {
            AcceptObject(player);
        }
    }

    private void AcceptObject(GameObject obj)
    {
        /* Pseudocode:
             * if counterpart != null:
             *      Play "shadow in" VFX
             *      play a sound effect
             *      make obj invisible
             *      make this vent shake back and forth slightly for a little bit
             *      teleport obj to counterpart location
             *      if obj == player:
             *          do jet's pathfinding thing
             *      when camera is settled on correct zone || obj != player and a small bit of time has passed:
             *          counterpart.EjectObject(obj)
            */
    }

    private void EjectObject(GameObject obj)
    {
        /* Pseudocode:
             * if !this.isOpen && obj != player: counterpart.EjectObject(obj); return;
             * ^ counterpart is not open yet, cannot eject object that is not the player. spit out object back at original vent.
             * 
             * make counterpart shake back and forth slightly for a little bit
             * shadow out VFX at obj new location
             * play sound effect at obj new location
             * make obj visible
             * if obj == player && this.isWallVent:
             *      make player remain a shadow
             */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();

        if (_playerScript != null)
        {
            _playerScript.OnInteract += this.MyInteraction;
        }
    }
}
