using UnityEngine;

public class Ectoplasm : Collectible
{

    private void Awake()
    {
        if (ChallengeManager.currentMode != ChallengeManager.ChallengeMode.Ectoplasm)
        {
            Destroy(gameObject);
        }
    }
    protected override void OnCollected(Collider2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        player.soundPlayer.PlaySound("Game.Key");

        EctoplasmBar eBar = FindFirstObjectByType<EctoplasmBar>();
        eBar.FullRestore();
    }
}
