using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager instance;

    public bool ectoplasmEnabled = false;
    public bool lightsOutEnabled = false;
    public bool spectralShuffleEnabled = false;

    //public static ChallengeMode currentMode = ChallengeMode.None;

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

#if UNITY_EDITOR
        ectoplasmEnabled = EditorPrefs.GetBool("EditorEctoplasmEnabled", false);
        lightsOutEnabled = EditorPrefs.GetBool("EditorLightsOutEnabled", false);
        spectralShuffleEnabled = EditorPrefs.GetBool("EditorSpectralShuffleEnabled", false);
#endif
    }
}
