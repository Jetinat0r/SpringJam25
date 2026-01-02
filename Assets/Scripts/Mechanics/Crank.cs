using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CrankableObject
{
    public GameObject affectedObject = null;
    public bool rotateClockwise = true;
}

public class Crank : MonoBehaviour
{
    [SerializeField] public MagicInteractionLine magicInteractionLinePrefab;
    [SerializeField] public Transform customMagicLinePivot = null;
    private MagicInteractionLine[] magicInteractionLines;
    [SerializeField]
    public CrankableObject[] affectedObjects;
    private PlayerMovement playerScript;
    //public Animator myAnim;
    public SpriteRenderer spriteRenderer;
    private int activeState = 0;
    public int framesPerState = 2;
    public float animationTime = 0.2f;
    public List<Sprite> spriteSheet;
    private Sequence rotationSequence = null;

    private void Awake()
    {
        if (customMagicLinePivot == null)
        {
            customMagicLinePivot = transform;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        magicInteractionLines = new MagicInteractionLine[affectedObjects.Length];

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i].affectedObject.TryGetComponent(out IRotatable _rotatable))
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
        //myAnim.SetTrigger("crankTrig");

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i] != null)
            {
                if (affectedObjects[i].affectedObject.TryGetComponent(out IRotatable _rotator))
                {
                    _rotator.OnRotate(affectedObjects[i].rotateClockwise);

                    magicInteractionLines[i].SetupLine(customMagicLinePivot.position, _rotator.CustomMagicLinePivot.position);
                    magicInteractionLines[i].PlayParticles();
                }
            }
        }

        //Increment State
        activeState += 1;
        activeState %= 4;

        rotationSequence?.Kill();
        rotationSequence = DOTween.Sequence(this);
        rotationSequence.onKill = () => { rotationSequence = null; };
        int f = 0;
        for (; f < framesPerState - 1; f++)
        {
            //Needs copied or else it gets a stale reference and explodes
            int _fCopy = f;
            rotationSequence.AppendCallback(() => { UpdateSprite(activeState * framesPerState + _fCopy); });
            rotationSequence.AppendInterval(animationTime / framesPerState);
        }
        rotationSequence.AppendCallback(() => { UpdateSprite(activeState * framesPerState + f); });
        rotationSequence.Play();

    }

    public void UpdateSprite(int _newSprite)
    {
        _newSprite %= framesPerState * 4;
        spriteRenderer.sprite = spriteSheet[_newSprite];
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
