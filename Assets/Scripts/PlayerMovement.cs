using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    [SerializeField]
    private Animator shadowAnimator;
    [SerializeField]
    private Animator spriteAnimator;

    [SerializeField]
    public SoundPlayer soundPlayer;

    [SerializeField]
    private LightDetector lightDetector;
    [SerializeField]
    private WallDetector wallDetector;

    public GameObject playerLightSprite;
    public GameObject playerShadowSprite;

    // Input
    private PlayerInput playerInput;
    private InputAction actionMove;
    private InputAction actionInteract;
    private InputAction actionShadow;
    private InputAction actionToggleMenu;
    private InputAction actionResetLevel;

    private float moveX;
    private float moveY;
    private bool interacted;
    private bool toggledShadow;

    [SerializeField]
    private float moveDeadzone;

    // FSM
    private PlayerStates currState;

    // C# Actions
    public Action OnInteract;

    // Flags
    public bool canInteract;
    private bool grounded;
    public bool hasKey;

    // Mutables
    public float moveSpd = 1.0f;
    private Vector2 velocity = Vector2.zero;
    private bool inLight = false;
    private bool onWall = false;
    private bool isShadow = false;
    private bool isDead = false;

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
        actionShadow = playerInput.actions["Shadow"];
        actionToggleMenu = playerInput.actions["ToggleUI"];
        actionResetLevel = playerInput.actions["RestartLevel"];

        grav = rb.gravityScale;

        //Subscribe to light detector events
        lightDetector.onLightEnter += OnEnterLight;
        lightDetector.onLightExit += OnExitLight;

        wallDetector.onWallEnter += OnEnterWall;
        wallDetector.onWallExit += OnExitWall;
    }

    private void OnDestroy()
    {
        //Unsubscribe from light detector events
        lightDetector.onLightEnter -= OnEnterLight;
        lightDetector.onLightExit -= OnExitLight;

        wallDetector.onWallEnter -= OnEnterWall;
        wallDetector.onWallExit -= OnExitWall;
    }

    void Update()
    {
        if (actionToggleMenu.WasPressedThisFrame())
        {
            LevelManager.instance.ToggleMenu();
        }

        if (actionResetLevel.WasPressedThisFrame())
        {
            LevelManager.instance.ResetScene();
        }

        if (isDead) return;

        // Update input values
        moveX = actionMove.ReadValue<Vector2>().x;
        moveY = actionMove.ReadValue<Vector2>().y;

        if (actionInteract.WasPressedThisFrame()) interacted = true;
        if (actionShadow.WasPressedThisFrame()) toggledShadow = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log($"Light State: {(inLight ? "Light" : "Shadow")}");

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

        // Reset input vars
        interacted = false;
        toggledShadow = false;
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

    // FSM functions
    void IdleGhost(float moveX, bool interacted, bool toggledShadow)
    {
        playerShadowSprite.SetActive(false);
        playerLightSprite.SetActive(true);
        if (!grounded)
        {
            rb.linearVelocityX = 0f;
            currState = PlayerStates.Falling;
        }

        if (Mathf.Abs(moveX) > moveDeadzone)
        {
            currState = PlayerStates.WalkGhost;
            SetGhost();
        }

        if (toggledShadow && inLight && onWall)
        {
            // Zero out velocity to ensure it is reset on state change
            rb.linearVelocity = Vector2.zero;
            currState = PlayerStates.IdleShadow;
            SetShadow();
            rb.gravityScale = 0.0f;
        }

        if (interacted && canInteract)
        {
            //Debug.Log($"{OnInteract}");
            OnInteract?.Invoke();
        }
    }

    void WalkGhost(float moveX, bool interacted, bool toggledShadow)
    {
        playerShadowSprite.SetActive(false);
        playerLightSprite.SetActive(true);
        rb.gravityScale = grav;

        Vector2 targetVelocity = new Vector2(moveX * moveSpd, rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, groundAcceleration);

        // Flip sprite based on movement direction
        Vector3 sprScale = sprite.transform.localScale;  // This is here to make typing easier
        sprite.transform.localScale = new Vector3(Mathf.Sign(rb.linearVelocityX), sprScale.y, sprScale.z);

        if (Mathf.Abs(moveX) < moveDeadzone)
        {
            currState = PlayerStates.IdleGhost;
            SetGhost();
        }

        if (!grounded)
        {
            rb.linearVelocityX = 0f;
            currState = PlayerStates.Falling;
        }

        if (toggledShadow && inLight && onWall)
        {
            // Zero out velocity to ensure it is reset on state change
            rb.linearVelocity = Vector2.zero;
            currState = PlayerStates.IdleShadow;
            SetShadow();
            rb.gravityScale = 0.0f;
        }

        if (interacted && canInteract)
        {
            OnInteract?.Invoke();
        }
    }


    void Falling(float moveX, bool interacted, bool toggledShadow)
    {
        //playerShadowSprite.SetActive(true);
        //playerLightSprite.SetActive(false);
        // Add ability to move while falling
        //rb.linearVelocity = new Vector2(moveX * moveSpd, rb.linearVelocity.y);

        if (grounded)
        {
            currState = PlayerStates.IdleGhost;
            SetGhost();
        }

        if (toggledShadow && inLight && onWall)
        {
            // Zero out velocity to ensure it is reset on state change
            rb.linearVelocity = Vector2.zero;
            currState = PlayerStates.IdleShadow;
            SetShadow();
            rb.gravityScale = 0.0f;
        }

        if (interacted && canInteract)
        {
            OnInteract?.Invoke();
        }
    }


    void IdleShadow(float moveX, float moveY, bool interacted, bool toggledShadow)
    {
        if (Mathf.Abs(moveX) > moveDeadzone || Mathf.Abs(moveY) > moveDeadzone)
        {
            SetShadow();
            currState = PlayerStates.WalkShadow;
        }

        if (toggledShadow)
        {
            // Zero out velocity to ensure it is reset on state change
            rb.linearVelocity = Vector2.zero;
            currState = PlayerStates.IdleGhost;
            SetGhost();
            rb.gravityScale = grav;
        }

        if (interacted && canInteract)
        {
            OnInteract?.Invoke();
        }
    }

    void WalkShadow(float moveX, float moveY, bool interacted, bool toggledShadow)
    {
        rb.linearVelocity = new Vector2(moveX * moveSpd, moveY * moveSpd);

        if(moveX > 0 && (moveY >= 0))
        {
            //playerShadowSprite.transform.Rotate(0, 0, 270f);
            playerShadowSprite.transform.rotation = Quaternion.Euler(0, 0, 270f);
        }
        else if(moveX < 0 && (moveY >= 0))
        {
            playerShadowSprite.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }

        if(moveY < 0)
        {
            playerShadowSprite.transform.rotation = Quaternion.Euler(0, 0, 180f);
        }

        if (Mathf.Abs(moveX) < moveDeadzone && Mathf.Abs(moveY) < moveDeadzone)
        {
            currState = PlayerStates.IdleShadow;
            SetShadow();
            playerShadowSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (toggledShadow)
        {
            // Zero out velocity to ensure it is reset on state change
            rb.linearVelocity = Vector2.zero;
            currState = PlayerStates.IdleGhost;
            SetGhost();
            rb.gravityScale = grav;
        }

        if (interacted && canInteract)
        {
            OnInteract?.Invoke();
        }
    }

    void OnEnterLight()
    {
        inLight = true;
    }

    void OnExitLight()
    {
        inLight = false;

        //If top down, fall out!
        if(currState == PlayerStates.IdleShadow || currState == PlayerStates.WalkShadow)
        {
            currState = PlayerStates.IdleGhost;
            rb.linearVelocity = Vector2.zero;
            SetGhost();
            rb.gravityScale = grav;
        }
    }

    void OnEnterWall()
    {
        onWall = true;
    }

    void OnExitWall()
    {
        onWall = false;

        //If top down, fall out!
        if (currState == PlayerStates.IdleShadow || currState == PlayerStates.WalkShadow)
        {
            currState = PlayerStates.IdleGhost;
            rb.linearVelocity = Vector2.zero;
            SetGhost();
            rb.gravityScale = grav;
        }
    }

    void SetShadow()
    {
        if (isShadow) return;

        playerShadowSprite.SetActive(true);
        playerLightSprite.SetActive(false);

        GetComponent<BoxCollider2D>().size = new Vector2(0.65f, 0.65f);
        foreach (BoxCollider2D box in GetComponentsInChildren<BoxCollider2D>())
        {
            box.size = new Vector2(0.65f, 0.65f);
        }
        shadowAnimator.SetFloat("Speed", 2);
        if (shadowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0)
        {
            shadowAnimator.Play("ShadowAnimation", 0);
        }
        soundPlayer.PlaySound("Game.ShadowIn");
        isShadow = true;
    }

    void SetGhost()
    {
        if (!isShadow) return;

        playerShadowSprite.SetActive(false);
        playerLightSprite.SetActive(true);

        GetComponent<BoxCollider2D>().size = new Vector2(0.7f, 1f);
        foreach (BoxCollider2D box in GetComponentsInChildren<BoxCollider2D>())
        {
            box.size = new Vector2(0.7f, 1f);
        }
        shadowAnimator.SetFloat("Speed", -2);
        soundPlayer.PlaySound("Game.ShadowOut");
        isShadow = false;
    }

    public void Die()
    {
        isDead = true;
        spriteAnimator.SetTrigger("die");
        soundPlayer.PlaySound("Game.Death");
        rb.AddForceY(400f);
        collision.enabled = false;
    }
}
