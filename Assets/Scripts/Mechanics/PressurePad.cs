using System;
using System.Collections.Generic;
using JetEngine;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    [SerializeField] public MagicInteractionLine magicInteractionLinePrefab;
    [SerializeField] public Transform customMagicLinePivot = null;
    private MagicInteractionLine[] magicInteractionLines;
    public GameObject[] affectedObjects;
    private int weight = 0;
    private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Sprite unpressed, pressed;
    [SerializeField] private bool localSound = false;
    private bool onScreen = true;
    private bool depressed = false;

    private void Awake()
    {
        if (customMagicLinePivot == null)
        {
            customMagicLinePivot = transform;
        }

        CameraSnapToPlayerZone.changeZone += OnZoneChanged;
    }

    private void OnZoneChanged(Vector2Int newZone)
    {
        Vector2Int _myZone = new Vector2(MathUtils.GetClosestEvenDivisor(transform.position.x, LevelManager.instance.zoneSize.x) / LevelManager.instance.zoneSize.x, MathUtils.GetClosestEvenDivisor(transform.position.y, LevelManager.instance.zoneSize.y) / LevelManager.instance.zoneSize.y).ToVector2Int();
        onScreen = _myZone == newZone;
    }

    private void OnDestroy()
    {
        CameraSnapToPlayerZone.changeZone -= OnZoneChanged;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        magicInteractionLines = new MagicInteractionLine[affectedObjects.Length];

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i].TryGetComponent(out IToggleable _toggleable))
            {
                magicInteractionLines[i] = Instantiate(magicInteractionLinePrefab);
                magicInteractionLines[i].SetupLine(customMagicLinePivot.position, _toggleable.CustomMagicLinePivot.position);
            }
            else
            {
                throw new Exception($"Affected object [{affectedObjects[i]}] in Pressure Pad [{name}] is not IToggleable!");
            }
        }
    }

    private void FixedUpdate()
    {
        if (!depressed)
        {
            if (weight == 1)
            {
                depressed = true;

                spriteRenderer.sprite = pressed;
                ChangeAffectedObjects();
                if (!localSound || (localSound && onScreen))
                {
                    PlayerMovement.instance.soundPlayer.PlaySound("Game.Lever");
                }
            }
        }
        else
        {
            if (weight == 0)
            {
                depressed = false;

                spriteRenderer.sprite = unpressed;
                ChangeAffectedObjects();
                if (!localSound || (localSound && onScreen))
                {
                    PlayerMovement.instance.soundPlayer.PlaySound("Game.Lever");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.layer == 10 || collision.gameObject.CompareTag("Player"))
            {
                int previousWeight = weight;
                weight++;
                // Debug.Log("You added some weight. Current weight: " + weight);
                CheckWeight(previousWeight);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (collision.gameObject.layer == 10 || collision.gameObject.CompareTag("Player"))
            {
                int previousWeight = weight;
                weight--;
                // Debug.Log("You took off some weight. Current weight: " + weight);
                CheckWeight(previousWeight);
            }
        }
    }

    private void CheckWeight(int previousWeight)
    {
        // Pad just has more weight on it. No state change required.
        if (weight > previousWeight && previousWeight > 0)
        {
            // Debug.Log("Pad state went unchanged. Heavier, but it was already weighed down before.");
            return;
        }

        if (weight < previousWeight && weight > 0)
        {
            // Debug.Log("Pad state went unchanged. Lighter, but still > 0");
        }

        if (weight == 0)
        {
            // Pad is no longer burdened by weight. Lift it up.
            // Debug.Log("Pad is now free of any and all weights.");
            /*
            spriteRenderer.sprite = unpressed;
            ChangeAffectedObjects();
            if (!localSound || (localSound && onScreen))
            {
                PlayerMovement.instance.soundPlayer.PlaySound("Game.Lever");
            }
            */
        }
        else if (weight == 1 && previousWeight == 0)
        {
            // Pad is burdened by new weight. Press it down.
            // Debug.Log("Pad is now weighed down.");
            /*
            spriteRenderer.sprite = pressed;
            ChangeAffectedObjects();
            if (!localSound || (localSound && onScreen))
            {
                PlayerMovement.instance.soundPlayer.PlaySound("Game.Lever");
            }
            */
        }
    }

    private void ChangeAffectedObjects()
    {
        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i] != null)
            {
                if (affectedObjects[i].TryGetComponent(out IToggleable _toggler))
                {
                    _toggler.OnToggle();

                    magicInteractionLines[i].PlayParticles();
                }
            }
        }
    }
}
