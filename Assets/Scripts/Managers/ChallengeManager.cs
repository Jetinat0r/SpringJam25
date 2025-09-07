using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager instance;

    public enum ChallengeMode
    {
        None,
        Ectoplasm,
        ThreeStrike,
        Hardcore
    }

    public static ChallengeMode currentMode = ChallengeMode.Ectoplasm;

    // Singleton stuff in case we decide to not use static vars and methods
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
