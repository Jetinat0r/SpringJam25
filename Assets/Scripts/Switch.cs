using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject[] affectedObjects;
    private PlayerMovement playerScript;
    [SerializeField] private SpriteRenderer sprite;
    public Sprite active, inactive;
    public bool on = false, wallSwitch = false;

    public void MyInteraction()
    {
        /*
         * TODO: Implement interaction code
         * This involves playing the animation and changing the states of the affected objects
         */

        // Temp code for testing. Remove when actual 
        Debug.Log("Yup, you sure did interact with this switch!");
        playerScript.soundPlayer.PlaySound("Game.Lever");
        on = !on;
        if (wallSwitch)
        {
            sprite.sprite = on ? active : inactive;
        }

        // Code stub for eventual implementation of changing affected objects
        foreach (var obj in affectedObjects)
        {
            if (obj != null)
            {
                // State-changing code/appropriate obj function call here
                obj.SetActive(on);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript = collision.gameObject.GetComponent<PlayerMovement>();
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
