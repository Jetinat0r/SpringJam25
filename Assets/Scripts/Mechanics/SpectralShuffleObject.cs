using UnityEngine;

public class SpectralShuffleObject : MonoBehaviour
{
    [Tooltip("Whether to keep or destroy in Spectral Shuffle mode. Default [false] is to destroy when NOT in spectral shuffle")]
    public bool destroyInSpectralShuffle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (destroyInSpectralShuffle)
        {
            if (ChallengeManager.instance.spectralShuffleEnabled)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (!ChallengeManager.instance.spectralShuffleEnabled)
            {
                Destroy(gameObject);
            }
        }
    }
}
