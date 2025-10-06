using UnityEngine;

public class Ectoplasm : Collectible
{
    [SerializeField] protected ParticleSystem ambientSmallVfx;
    [SerializeField] protected ParticleSystem pickupVfx;

    private void Awake()
    {
        if (ChallengeManager.currentMode != ChallengeManager.ChallengeMode.Ectoplasm)
        {
            Destroy(gameObject);
        }
    }

    /*
    protected override void BeginHandleAmbientVFX()
    {
        ambience = StartCoroutine(AmbientVFXLoop(new Vector3(0f, 0.25f, 0f)));
    }
    */

    protected override void OnCollected(Collider2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        PlayPickupSFX(player);

        EctoplasmBar eBar = FindFirstObjectByType<EctoplasmBar>();
        eBar.FullRestore();

        ambientSmallVfx.Stop();
        pickupVfx.Play();

        Destroy(gameObject);
    }
}
