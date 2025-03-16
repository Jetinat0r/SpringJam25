using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerStates
{
    IdleGhost,
    WalkGhost,
    Falling,
    IdleShadow,
    WalkShadow
}

public class PlayerMovement : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D collision;
    private SpriteRenderer sprite;

    // Input
    private PlayerInput playerInput;
    private InputAction actionMove;
    private InputAction actionInteract;
    private InputAction actionShadow;

    private float moveX;
    private float moveY;
    private bool interacted;
    private bool toggledShadow;

    [SerializeField]
    private float moveDeadzone;

    // FSM
    private PlayerStates currState;

    // Flags
    public bool canShadow;
    public bool canInteract;
    private bool grounded;

    // Mutables
    public float moveSpd = 1.0f;
    private Vector2 velocity = Vector2.zero;

    // Constants
    private float grav;
    [SerializeField] private float groundAcceleration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<BoxCollider2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        currState = PlayerStates.IdleGhost;

        // Set Input Bindings
        actionMove = playerInput.actions["Move"];
        actionInteract = playerInput.actions["Interact"];
        // TODO: Rename or make new action called Shadow in place of Sprint
        actionShadow = playerInput.actions["Sprint"];

        grav = rb.gravityScale;
    }

    void Update()
    {
        // Update input values
        moveX = actionMove.ReadValue<Vector2>().x;
        moveY = actionMove.ReadValue<Vector2>().y;
        interacted = actionInteract.WasPressedThisFrame();
        toggledShadow = actionInteract.WasPressedThisFrame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Execute current state
        switch (currState)
        {
            case PlayerStates.IdleGhost:
                IdleGhost(moveX, interacted, toggledShadow);
                break;
            case PlayerStates.WalkGhost:
                WalkGhost(moveX, interacted, toggledShadow);
                break;
            case PlayerStates.Falling:
                Falling(moveX, interacted, toggledShadow);
                break;
            case PlayerStates.IdleShadow:
                IdleShadow(moveX, moveY, interacted, toggledShadow);
                break;
            case PlayerStates.WalkShadow:
                WalkShadow(moveX, moveY, interacted, toggledShadow);
                break;
        }
    }

    // Set/unset grounded
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            grounded = false;
        }
    }

    void IdleGhost(float moveX, bool interacted, bool toggledShadow)
    {
        rb.gravityScale = grav;

        if (!grounded)
        {
            currState = PlayerStates.Falling;
        }

        if (Mathf.Abs(moveX) > moveDeadzone)
        {
            currState = PlayerStates.WalkGhost;
        }

        if (toggledShadow && canShadow)
        {
            currState = PlayerStates.IdleShadow;
        }

        if (interacted && canInteract)
        {
            // TODO: Invoke interactable object's event(?)
            // NOTE: Probably an interface for all interactable objects idk we'll talk aobut it some more or somthn
        }
    }

    void WalkGhost(float moveX, bool interacted, bool toggledShadow)
    {
        rb.gravityScale = grav;

        Vector2 targetVelocity = new Vector2(moveX * moveSpd, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, groundAcceleration);

        if (Mathf.Abs(moveX) < moveDeadzone)
        {
            currState = PlayerStates.IdleGhost;
        }

        if (!grounded)
        {
            currState = PlayerStates.Falling;
        }

        if (toggledShadow && canShadow)
        {
            currState = PlayerStates.IdleShadow;
        }

        if (interacted && canInteract)
        {
            // TODO: Invoke interactable object's event(?)
            // NOTE: Probably an interface for all interactable objects idk we'll talk aobut it some more or somthn
        }
    }


    void Falling(float moveX, bool interacted, bool toggledShadow)
    {
        rb.gravityScale = grav;

        // Add ability to move while falling
        rb.linearVelocity = new Vector2(moveX * moveSpd, rb.linearVelocity.y);

        if (grounded)
        {
            currState = PlayerStates.IdleGhost;
        }

        if (toggledShadow && canShadow)
        {
            currState = PlayerStates.IdleShadow;
        }

        if (interacted && canInteract)
        {
            // TODO: Invoke interactable object's event(?)
            // NOTE: Probably an interface for all interactable objects idk we'll talk aobut it some more or somthn
        }
    }


    void IdleShadow(float moveX, float moveY, bool interacted, bool toggledShadow)
    {
        rb.gravityScale = 0.0f;

        if (Mathf.Abs(moveX) > moveDeadzone || Mathf.Abs(moveY) > moveDeadzone)
        {
            currState = PlayerStates.WalkShadow;
        }

        if (toggledShadow && canShadow)
        {
            currState = PlayerStates.IdleGhost;
        }

        if (interacted && canInteract)
        {
            // TODO: Invoke interactable object's event(?)
            // NOTE: Probably an interface for all interactable objects idk we'll talk aobut it some more or somthn
        }
    }


    void WalkShadow(float moveX, float moveY, bool interacted, bool toggledShadow)
    {
        rb.gravityScale = 0.0f;

        rb.linearVelocity = new Vector2(moveX * moveSpd, moveY * moveSpd);

        if (Mathf.Abs(moveX) < moveDeadzone && Mathf.Abs(moveY) < moveDeadzone)
        {
            currState = PlayerStates.IdleShadow;
        }

        if (toggledShadow && canShadow)
        {
            currState = PlayerStates.IdleGhost;
        }

        if (interacted && canInteract)
        {
            // TODO: Invoke interactable object's event(?)
            // NOTE: Probably an interface for all interactable objects idk we'll talk aobut it some more or somthn
        }
    }
}
