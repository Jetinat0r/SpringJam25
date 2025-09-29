using System;
using UnityEngine;

public class Crank : MonoBehaviour
{
    [SerializeField] public MagicInteractionLine magicInteractionLinePrefab;
    [SerializeField] public Transform customMagicLinePivot = null;
    private MagicInteractionLine[] magicInteractionLines;
    public GameObject[] affectedObjects;
    private PlayerMovement playerScript;
    public Animator myAnim;

    private void Awake()
    {
        if (customMagicLinePivot == null)
        {
            customMagicLinePivot = transform;
        }
    }

    private void Start()
    {
        magicInteractionLines = new MagicInteractionLine[affectedObjects.Length];

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i].TryGetComponent(out IRotatable _rotatable))
            {
                magicInteractionLines[i] = Instantiate(magicInteractionLinePrefab);
                magicInteractionLines[i].SetupLine(customMagicLinePivot.position, _rotatable.CustomMagicLinePivot.position);
            }
            else
            {
                throw new Exception($"Affected object [{affectedObjects[i]}] in Crank [{name}] is not IRotatable!");
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
        //Debug.Log("Yup, you sure did interact with this crank!");
        playerScript.soundPlayer.PlaySound("Game.Crank");
        myAnim.SetTrigger("crankTrig");

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i] != null)
            {
                if (affectedObjects[i].TryGetComponent(out IRotatable _rotator))
                {
                    _rotator.OnRotate();

                    magicInteractionLines[i].PlayParticles();
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
