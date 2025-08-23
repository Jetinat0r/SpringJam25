using System;
using UnityEngine;

public class Crank : MonoBehaviour
{
    public GameObject[] affectedObjects;
    private PlayerMovement playerScript;
    public Animator myAnim;

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

        foreach (GameObject obj in affectedObjects)
        {
            if (obj != null)
            {
                if (obj.TryGetComponent(out IRotatable _rotator))
                {
                    _rotator.OnRotate();
                }
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
