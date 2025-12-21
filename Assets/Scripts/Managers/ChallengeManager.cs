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

        var _beatEctoplasm = ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatEctoplasm;
        var _beatLightsOut = ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatLightsOut;
        var _beatSpectralShuffle = ProgramManager.instance.saveData.WorldSaveData[_curWorldIndex].levels[_curLevelIndex].challenges.beatSpectralShuffle;

        if (ectoplasmEnabled)
        {
            if (!_beatEctoplasm)
            {
                // Beat it for the first time
                // Increase stat
                JetEngine.SteamUtils.TryIncrementStat("ep_count");
            }

            _beatEctoplasm = true;
        }
        if (lightsOutEnabled)
        {
            if (!_beatLightsOut)
            {
                // Beat it for the first time
                // Increase stat
                JetEngine.SteamUtils.TryIncrementStat("lo_count");
            }

            _beatLightsOut = true;
        }
        if (spectralShuffleEnabled)
        {
            if (!_beatSpectralShuffle)
            {
                // Beat it for the first time
                // Increase stat
                JetEngine.SteamUtils.TryIncrementStat("ss_count");
            }

            _beatSpectralShuffle = true;
        }

        ProgramManager.instance.saveData.SaveSaveData();
    }
}
