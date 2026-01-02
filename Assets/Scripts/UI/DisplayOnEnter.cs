using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisplayOnEnter : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> objectsToRevealOnEnter;
    [SerializeField]
    public List<GameObject> objectsToRevealOnInput;

    [SerializeField]
    public bool hideEnterObjectsOnExit = false;

    private bool playerInBox = false;

    private PlayerInput playerInput;
    private InputAction actionShadow;
    private bool completedShadowAction = false;

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

        if (hideEnterObjectsOnExit)
        {
            foreach (GameObject obj in objectsToRevealOnEnter)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        playerInBox = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = InputOverlord.instance.playerInput;

        actionShadow = playerInput.actions["Shadow"];

        actionShadow.started += ActionShadow_started;
    }

    private void OnDestroy()
    {
        if (!completedShadowAction)
        {
            actionShadow.started -= ActionShadow_started;
        }
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
            completedShadowAction = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
