using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusicTrigger : MonoBehaviour
{
    private bool triggered = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            triggered = true;
            AudioManager.instance.FadeOutCurrent(1f);
            Invoke(nameof(ResetAudioPlayer), 1.1f);
        }
    }

    void ResetAudioPlayer()
    {
        AudioManager.instance.ResetPlayer();
    }
}
