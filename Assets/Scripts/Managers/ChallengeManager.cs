#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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

    public void TryCompleteChallenges()
    {
        int _curLevelNumber = LevelManager.instance.currentLevelNumber - 1;
        int _curWorldIndex = _curLevelNumber / 8;
        int _curLevelIndex = _curLevelNumber % 8;

        if (ectoplasmEnabled) ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatEctoplasm = true;
        if (lightsOutEnabled) ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatLightsOut = true;
        if (spectralShuffleEnabled) ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatSpectralShuffle = true;

        ProgramManager.instance.saveData.SaveSaveData();
    }
}
