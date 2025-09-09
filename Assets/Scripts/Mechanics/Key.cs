using System;
using UnityEngine;

public class Key : Collectible
{
    protected override void OnCollected(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            PlayPickupSFX(player);
            Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
            foreach (Door door in doors)
            {
                if (door.remainingKeys.Contains(this))
                {
                    door.remainingKeys.Remove(this);
                    door.UpdateKeyholes(true);
                    Debug.Log("Keyholes updated");
                }
            }

            DisplayPickupVFX();
            Destroy(gameObject);
        }
    }
}
