using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections.Generic;

public class Vent : MonoBehaviour, IToggleable
{
    [SerializeField]
    public GameObject noEntryIcon;

    public bool isWallVent = true;
    public bool isOpen = false;
    public Vent counterpart;

    private BoxCollider2D objectTrigger = null;
    private CircleCollider2D interactionRadius = null;
    private PlayerMovement player = null;
    
    //List of all boxes currently touching the vent
    //  We have to allow travel time, and can't suck in > 1 box at the same time (they'll spawn into each other)
    public List<Box> touchingBoxes = new();
    public List<GameObject> objectsInTransit = new();

    private void Start()
    {
        objectTrigger = GetComponent<BoxCollider2D>();
        interactionRadius = GetComponent<CircleCollider2D>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void OnToggle()
    {
        // TODO: Animate the vent opening/closing

        isOpen = !isOpen;
    }

    public void MyInteraction()
    {
        if (isOpen && counterpart != null)
        {
            //AcceptObject(player);
            player.EnterVent(this);
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

    //TODO: Implement
    //If the exit is blocked by a box, return true
    public bool CheckIsBlocked()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Entered {collision.name}");
        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            //_playerScript.OnInteract += this.MyInteraction;
            player.OnInteract += this.MyInteraction;

            //TODO: No entry icon
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();

            //_playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            //_playerScript.OnInteract -= MyInteraction;
            player.OnInteract -= this.MyInteraction;

            //noEntryIcon.SetActive(false);
        }
    }
}
