using UnityEngine;
using System.Collections.Generic;

public class Vent : MonoBehaviour, IToggleable
{
    public LayerMask boxLayer = 0;

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

    public Transform CustomMagicLinePivot { get; set; }

#if UNITY_EDITOR
    public Color DebugLineColor = Color.magenta;
    //Used so we don't have both sides of the pair draw a line
    public bool DrawDebugLine = false;
    public Vector3 DebugCenterOffset = new Vector3(0f, 0.25f, 0f);
#endif

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
        if (isOpen && counterpart != null && !counterpart.CheckIsBlocked())
        {
            //AcceptObject(player);
            player.EnterVent(this);
        }
        else
        {
            player.PlayClangSound();
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
        return touchingBoxes.Count > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Entered {collision.name}");
        if (collision.gameObject.CompareTag("Player"))
        {
            //PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            //_playerScript.OnInteract += this.MyInteraction;
            player.OnInteract -= this.MyInteraction;
            player.OnInteract += this.MyInteraction;

            //TODO: No entry icon
        }
        else if (collision.gameObject.TryGetComponent(out Box _box))
        {
            touchingBoxes.Add(_box);
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
        else if (collision.gameObject.TryGetComponent(out Box _box))
        {
            touchingBoxes.Remove(_box);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
      if (DrawDebugLine && counterpart != null)
        {
            Color _originalColor = Gizmos.color;
            Gizmos.color = DebugLineColor;

            Gizmos.DrawLine(transform.position + DebugCenterOffset, counterpart.transform.position + DebugCenterOffset);

            Gizmos.color = _originalColor;
        }  
    }
#endif
}
