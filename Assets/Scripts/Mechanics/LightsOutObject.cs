using UnityEngine;


public class LightsOutObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ChallengeManager.instance.lightsOutEnabled)
        {
            Destroy(gameObject);
        }
    }
}
