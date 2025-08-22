using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour
{
    public GameObject[] affectedObjects;
    private int weight = 0;
    private SpriteRenderer spriteRenderer = null;

    private List<GameObject> allowedObjects = new List<GameObject>();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Add a list of all objects that can affect the pad.
        Box[] _boxes = FindObjectsByType<Box>(FindObjectsSortMode.None);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (Box box in _boxes)
        {
            allowedObjects.Add(box.gameObject);
        }

        allowedObjects.Add(player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (allowedObjects.Contains(collision.gameObject))
            {
                int previousWeight = weight;
                weight++;
                Debug.Log("You added some weight. Current weight: " + weight);
                CheckWeight(previousWeight);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != null)
        {
            if (allowedObjects.Contains(collision.gameObject))
            {
                int previousWeight = weight;
                weight--;
                Debug.Log("You took off some weight. Current weight: " + weight);
                CheckWeight(previousWeight);
            }
        }
    }

    private void CheckWeight(int previousWeight)
    {
        // Pad just has more weight on it. No state change required.
        if (weight > previousWeight && previousWeight > 0)
        {
            Debug.Log("Pad state went unchanged. Heavier, but it was already weighed down before.");
            return;
        }

        if (weight < previousWeight && weight > 0)
        {
            Debug.Log("Pad state went unchanged. Lighter, but still > 0");
        }
        
        if (weight == 0)
        {
            // Pad is no longer burdened by weight. Lift it up.
            Debug.Log("Pad is now free of any and all weights.");

            ChangeAffectedObjects();
        }
        else if (weight == 1 && previousWeight == 0)
        {
            // Pad is burdened by new weight. Press it down.
            Debug.Log("Pad is now weighed down.");

            ChangeAffectedObjects();
        }
    }

    private void ChangeAffectedObjects()
    {
        foreach (GameObject obj in affectedObjects)
        {
            if (obj != null)
            {
                IToggleable _toggler;

                if (obj.TryGetComponent<IToggleable>(out _toggler))
                {
                    _toggler.OnToggle();
                }
            }
        }
    }

}
