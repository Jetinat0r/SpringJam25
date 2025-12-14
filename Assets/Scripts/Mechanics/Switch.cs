using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] public MagicInteractionLine magicInteractionLinePrefab;
    [SerializeField] public Transform customMagicLinePivot = null;
    private MagicInteractionLine[] magicInteractionLines;
    public GameObject[] affectedObjects;
    public CrankableObject[] crankableObjects;
    private PlayerMovement playerScript;
    [SerializeField] private SpriteRenderer sprite;
    public Sprite active, inactive;
    public bool on = false, wallSwitch = false;
    public Animator myAnim;
    private bool flipped = false;

    private void Awake()
    {
        if (customMagicLinePivot == null)
        {
            customMagicLinePivot = transform;
        }
    }

    private void Start()
    {
        magicInteractionLines = new MagicInteractionLine[affectedObjects.Length + crankableObjects.Length];

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i].TryGetComponent(out IToggleable _toggleable))
            {
                magicInteractionLines[i] = Instantiate(magicInteractionLinePrefab);
                magicInteractionLines[i].SetupLine(customMagicLinePivot.position, _toggleable.CustomMagicLinePivot.position);
            }
            else
            {
                Debug.LogError($"Affected object [{affectedObjects[i]}] in Switch [{name}] is not IToggleable!");
                throw new Exception($"Affected object [{affectedObjects[i]}] in Switch [{name}] is not IToggleable!");
            }
        }

        for (int j = affectedObjects.Length; j < affectedObjects.Length + crankableObjects.Length; j++)
        {
            int i = j - affectedObjects.Length;
            if (crankableObjects[i].affectedObject.TryGetComponent(out IRotatable _rotateable))
            {
                magicInteractionLines[j] = Instantiate(magicInteractionLinePrefab);
                magicInteractionLines[j].SetupLine(customMagicLinePivot.position, _rotateable.CustomMagicLinePivot.position);
            }
            else
            {
                Debug.LogError($"Affected object [{crankableObjects[i]}] in Switch [{name}] is not IRotateable!");
                throw new Exception($"Affected object [{crankableObjects[i]}] in Switch [{name}] is not IRotateable!");
            }
        }
    }

    public void MyInteraction()
    {
        /*
         * TODO: Implement interaction code
         * This involves playing the animation and changing the states of the affected objects
         */

        // Temp code for testing. Remove when actual 
        //Debug.Log("Yup, you sure did interact with this switch!");
        playerScript.soundPlayer.PlaySound("Game.Lever");
        on = !on;
        if (wallSwitch)
        {
            sprite.sprite = on ? active : inactive;
        }
        else{
            if(flipped == false)
            {
                myAnim.SetTrigger("leverTrig");
                flipped = true;
            }
            else if(flipped == true)
            {
                //print("DUCK");
                myAnim.SetTrigger("back");
                flipped = false;
            }
        }

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i] != null)
            {
                if (affectedObjects[i].TryGetComponent(out IToggleable _toggler))
                {
                    _toggler.OnToggle();

                    magicInteractionLines[i].SetupLine(customMagicLinePivot.position, _toggler.CustomMagicLinePivot.position);
                    magicInteractionLines[i].PlayParticles();
                }
            }
        }

        for (int j = affectedObjects.Length; j < affectedObjects.Length + crankableObjects.Length; j++)
        {
            int i = j - affectedObjects.Length;
            if (crankableObjects[i].affectedObject != null)
            {
                if (crankableObjects[i].affectedObject.TryGetComponent(out IRotatable _rotateable))
                {
                    _rotateable.OnRotate(crankableObjects[i].rotateClockwise);
                    //Flip bool so next "interact" flips it the other way
                    crankableObjects[i].rotateClockwise = !crankableObjects[i].rotateClockwise;

                    magicInteractionLines[j].SetupLine(customMagicLinePivot.position, _rotateable.CustomMagicLinePivot.position);
                    magicInteractionLines[j].PlayParticles();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            playerScript.OnInteract -= MyInteraction;
            playerScript.OnInteract += MyInteraction;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript = collision.gameObject.GetComponent<PlayerMovement>();
            playerScript.OnInteract -= MyInteraction;
        }
    }
}
