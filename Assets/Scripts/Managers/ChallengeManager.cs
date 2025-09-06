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
