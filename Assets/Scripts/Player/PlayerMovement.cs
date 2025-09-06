using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private BoxCollider2D lightCollision, wallCollision;
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
    private bool checkIsForcedOutShadow = false;

    // Mutables
    public float moveSpd = 1.0f;
    public float spdBoost = 0.0f;
    private Vector2 velocity = Vector2.zero;
    private bool inLight = false;
    private bool onWall = false;
    private bool isShadow = false;
    public bool isDead { get; private set; } = false;
    public bool hasWon { get; private set; } = false;
    private bool isPushing = false;
    //The temptation to call this "isSus" is so strong, but I remain stronger
    public bool isVenting = false;

    // Constants
    private float grav;
    [SerializeField] private float groundAcceleration;

    private Vector2 ghostSize = new(0.6875f, 0.8125f);
    private Vector2 ghostOffset = new(0f, 0.40625f);
    private Vector2 ghostDetectSize = new(0.6875f, 1f);

    // Physics Materials
    [SerializeField] private PhysicsMaterial2D slippery, friction;

    // Singleton
    public static PlayerMovement instance;

    //The vent the player is actively moving through
    //private Vent enteredVent = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.RegisterPlayer(this);
        }

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

        LevelMenuManager.playerOverride = false;

        instance = this;
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
        if (isVenting) { return; }

        if (actionToggleMenu.WasPressedThisFrame() && ScreenWipe.over)
        {
            LevelManager.instance.ToggleMenu();
        }

        if (LevelMenuManager.isMenuOpen) return;

        if (actionResetLevel.WasPressedThisFrame() && ScreenWipe.over && !isDead && !hasWon)
        {
            LevelManager.instance.ResetScene();
        }

        if (isDead || hasWon) return;
        if (LevelMenuManager.isMenuClosedThisFrame)
        {
            LevelMenuManager.isMenuClosedThisFrame = false;
            return;
        }

        // Update input values
        moveX = actionMove.ReadValue<Vector2>().x;
        moveY = actionMove.ReadValue<Vector2>().y;

        // Cancel and de-normalize vertical inputs as a ghost
        if (!isShadow && moveX != 0)
        {
            float mag = new Vector2(moveX, moveY).magnitude;
            moveX = Mathf.Sign(moveX) * mag;
        }
        
        //Debug.Log($"MOVE: {moveX}, {moveY}");

        //These things are why we can de-shadow mid camera transition lol
        if (actionInteract.WasPressedThisFrame()) interacted = true;
        if (actionShadow.WasPressedThisFrame()) toggledShadow = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelMenuManager.isMenuOpen || hasWon || isDead || isVenting) return;

        //Debug.Log($"Light State: {(inLight ? "Light" : "Shadow")}");
        if (checkIsForcedOutShadow)
        {
            checkIsForcedOutShadow = false;

            if (!onWall || !inLight)
            {
                ExitShadow();
            }
            //else we're in shadow and don't do anything
        }

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

        // Relay falling state to animator
        spriteAnimator.SetBool("isFalling", currState == PlayerStates.Falling);

        // Reset input vars
        interacted = false;
        toggledShadow = false;

        // Ground/box push check
        grounded = false;
        isPushing = false;
        spdBoost = 0;
        List<ContactPoint2D> contacts = new();
        rb.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            // TODO: Amend this check for other ground collidable objects
            if (contact.collider.gameObject.layer == 6 || contact.collider.gameObject.layer == 10 || contact.collider.gameObject.layer == 11)
            {
                if (Mathf.Abs((contact.normal - Vector2.up).magnitude) <= 0.001f)
                {
                    grounded = true;
                    if (contact.collider.gameObject.TryGetComponent(out ConveyorBelt belt) && currState == PlayerStates.WalkGhost)
                    {
                        if ((belt.clockwise && moveX > 0) || (!belt.clockwise && moveX < 0))
                        {
                            spdBoost = belt.speed;
                        }
                        else if ((belt.clockwise && moveX < 0) || (!belt.clockwise && moveX > 0))
                        {
                            spdBoost = -belt.speed / 2;
                        }
                    }
                }
            }
            // Detect pushing boxes
            if (contact.collider.gameObject.layer == 10 && (currState == PlayerStates.WalkGhost || currState == PlayerStates.WalkShadow))
            {
                // Only works if touching the left or right side
                if ((moveX < 0 && Mathf.Abs((contact.normal - Vector2.right).magnitude) <= 0.001f) || (moveX > 0 && Mathf.Abs((contact.normal - Vector2.left).magnitude) <= 0.001f))
                {
                    // Only works if touching below the top
                    if (contact.point.y < contact.collider.bounds.extents.y + contact.collider.bounds.center.y - 0.01f)
                    {
                        isPushing = true;
                    }
                }
            }
        }
        spriteAnimator.SetBool("isPushing", isPushing);

        // Set physics material for proper friction behavior
        if (!grounded && !isShadow)
        {
            rb.sharedMaterial = slippery;
        }
        else
        {
            rb.sharedMaterial = friction;
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

        Vector2 targetVelocity = new Vector2(moveX * (moveSpd + spdBoost), rb.linearVelocity.y);
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocity, groundAcceleration);

        // Flip sprite based on movement direction
        Vector3 sprScale = sprite.transform.localScale;  // This is here to make typing easier
        // TODO: This is not a good check! Has a right bias
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
        rb.linearVelocity = Vector2.zero;

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

        if(!(moveX == 0f && moveY == 0f))
        {
            float _deg = Mathf.Atan2(moveY, moveX);

            //Debug.Log($"Deg: {_deg} {moveY} {moveX}");

            playerShadowSprite.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Rad2Deg * _deg) - 90f);
            /*
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
            */
        }

        if (Mathf.Abs(moveX) < moveDeadzone && Mathf.Abs(moveY) < moveDeadzone)
        {
            currState = PlayerStates.IdleShadow;
            SetShadow();
            //playerShadowSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
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
        if (isDead) return;
        inLight = false;

        checkIsForcedOutShadow = true;
    }

    void OnEnterWall()
    {
        onWall = true;
    }

    void OnExitWall()
    {
        if(isDead) { return; }
        onWall = false;

        checkIsForcedOutShadow = true;
    }

    void ExitShadow()
    {
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

        collision.size = 0.59375f * Vector2.one;
        collision.offset = 0.5f * Vector2.up;

        lightCollision.size = collision.size;
        lightCollision.offset = collision.offset;

        wallCollision.size = collision.size;
        wallCollision.offset = collision.offset;

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

        collision.size = ghostSize;
        collision.offset = ghostOffset;

        lightCollision.size = ghostDetectSize;
        lightCollision.offset = Vector2.up * 0.5f;

        wallCollision.size = ghostDetectSize;
        wallCollision.offset = lightCollision.offset;

        shadowAnimator.SetFloat("Speed", -2);
        soundPlayer.PlaySound("Game.ShadowOut");
        if (shadowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            shadowAnimator.Play("ShadowAnimation", 0, 1);
        }
        isShadow = false;
    }

    public void EnterVent(Vent _vent)
    {
        if (isVenting)
        {
            return;
        }

        if (_vent.counterpart == null)
        {
            //TODO:
            //Display NO ENTRY or NO VENT symbol
            Debug.LogError($"NO VENT COUNTERPART: {_vent.name}");
            return;
        }

        if (_vent.CheckIsBlocked())
        {
            //TODO:
            //Display NO ENTRY or BLOCKAGE symbol
            Debug.Log($"Vent blocked {_vent.name}");
            return;
        }

        if(!LevelManager.instance.GetVentPath(_vent, out List<int> _zonePath))
        {
            //TODO:
            //Display NO ENTRY or NO PATH symbol
            Debug.LogError($"NO VALID VENT PATH: {_vent.name}");
            return;
        }
        else
        {
            Debug.Log(_zonePath.Count);
        }

        isVenting = true;

        playerLightSprite.SetActive(false);
        playerShadowSprite.SetActive(false);

        //enteredVent = _vent;
        rb.position = _vent.counterpart.transform.position;

        CameraTarget[] _targets = FindObjectsByType<CameraTarget>(FindObjectsSortMode.None);
        _targets[0].TakeVentPath(_zonePath, ExitVent);
    }

    public void ExitVent()
    {
        //TODO: Allow venting and keeping shadow state?
        playerLightSprite.SetActive(true);
        //playerShadowSprite.SetActive(false);

        
        //enteredVent = null;

        isVenting = false;
    }

    public void Die()
    {
        if (hasWon || LevelManager.instance.isResetting || isDead) return;
        LevelMenuManager.playerOverride = true;
        isDead = true;
        playerShadowSprite.SetActive(false);
        playerLightSprite.SetActive(true);
        spriteAnimator.SetTrigger("die");
        soundPlayer.PlaySound("Game.Death");
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = grav;
        rb.AddForceY(400f);
        collision.enabled = false;
        LevelManager.instance.ResetSceneIn(2f);
    }

    public void Win()
    {
        if (isDead || LevelManager.instance.isResetting) return;
        LevelMenuManager.playerOverride = true;
        rb.linearVelocity = Vector2.zero;
        playerShadowSprite.SetActive(false);
        playerLightSprite.SetActive(true);
        spriteAnimator.SetTrigger("win");
        soundPlayer.PlaySound("Game.LevelClear", 0.6f);
        hasWon = true;
        LevelManager.instance.CompleteLevel();
    }
}
