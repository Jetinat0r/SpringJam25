using UnityEngine;

public class EctoplasmBar : MonoBehaviour
{
    // Pixel measurements for proper tiling
    [SerializeField] private const int barPixelsWide = 6;
    [SerializeField] private const int barPixelsTall = 27;
    private const float pixelUnit = 0.03125f;   // 1/32
    private const float initialTilingW = barPixelsWide * pixelUnit;  // 6/32
    private const float initialTilingH = barPixelsTall * pixelUnit; // 27/32
    
    private GameObject player = null;
    private PlayerMovement playerScript = null;

    [Header("Sprite Components")]
    [SerializeField] private SpriteRenderer barFillSprite;
    [SerializeField] private SpriteRenderer waveSprite;

    [SerializeField] private float maxTime = 5f;
    private float timeRemaining;
    private float maxCapacity = 100f;
    private float currentAmount;

    private void Awake()
    {
        if (ChallengeManager.currentMode == ChallengeManager.ChallengeMode.Ectoplasm)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        player = transform.parent.gameObject;
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerMovement>();
        }

        // Always start bar at full
        timeRemaining = maxTime;
    }

    private void Update()
    {
        if (playerScript.hasWon) return;

        // Make it so that the bar is always behind the player and not in the way
        transform.localScale = player.GetComponentInChildren<SpriteRenderer>().gameObject.transform.localScale;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            if (!playerScript.isDead)
            {
                playerScript.Die();
            }

            timeRemaining = 0;
        }
        currentAmount = timeRemaining / maxTime * maxCapacity;

        // Apply current amount to bar
        AdjustSprites();
    }

    public void FullRestore()
    {
        timeRemaining = maxTime;
        AdjustSprites();
    }

    private void AdjustSprites()
    {
        // Adjust bar fill
        var tilingY = (timeRemaining / maxTime) * initialTilingH;
        barFillSprite.size = new Vector2(barFillSprite.size.x, tilingY);

        // Adjust waves
        Vector3 wavePos = waveSprite.transform.localPosition;
        wavePos.y = tilingY - initialTilingH;
        waveSprite.transform.localPosition = wavePos;
    }
}
