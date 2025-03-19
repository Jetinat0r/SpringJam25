using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public void MyInteraction()
    {
        // TODO: Talk with some scene manager or initiate some UI thing to initiate level complete
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Spawn interaction prompt over player head
        if (collision.gameObject.CompareTag("Player"))
        {
            // This is where you'd summon the interact button prompt if we want to make it appear over their head regardless of whether or not they have a key
            PlayerMovement playerScript = collision.gameObject.GetComponent<PlayerMovement>();

            if (playerScript != null)
            {
                if (playerScript.hasKey)
                {
                    playerScript.OnInteract += MyInteraction;
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        // TODO: Spawn interaction prompt over player
        collision.gameObject.GetComponent<PlayerMovement>().OnInteract -= MyInteraction;
    }
}
