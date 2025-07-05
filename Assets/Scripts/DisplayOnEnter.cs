using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayOnEnter : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> objectsToRevealOnEnter;
    [SerializeField]
    public List<GameObject> objectsToRevealOnInput;

    private bool playerInBox = false;

    [SerializeField]
    private PlayerInput playerInput;
    private InputAction actionShadow;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        foreach (GameObject obj in objectsToRevealOnEnter)
        {
            obj.SetActive(true);
        }

        playerInBox = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        playerInBox = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionShadow = playerInput.actions["Shadow"];

        actionShadow.started += ActionShadow_started;
    }

    private void OnDestroy()
    {
        actionShadow.started -= ActionShadow_started;
    }

    private void ActionShadow_started(InputAction.CallbackContext context)
    {
        if(playerInBox)
        {
            foreach (GameObject obj in objectsToRevealOnInput)
            {
                obj.SetActive(true);
            }

            actionShadow.started -= ActionShadow_started;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
