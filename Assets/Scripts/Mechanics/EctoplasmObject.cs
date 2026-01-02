using UnityEngine;

public class EctoplasmObject : MonoBehaviour
{
    void Awake()
    {
        if (!ChallengeManager.instance.ectoplasmEnabled)
        {
            Destroy(gameObject);
        }
    }
}
