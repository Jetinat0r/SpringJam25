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

        //Attempt to update steam stats
        if (ectoplasmEnabled)
        {
            int _ectoplasmCompleted = ProgramManager.instance.saveData.GetNumCompletedEctoplasm();
            JetEngine.SteamUtils.TrySetStat("ep_count", _ectoplasmCompleted);
            if (_ectoplasmCompleted == 32)
            {
                JetEngine.SteamUtils.TryGetAchievement("CHALLENGECLEAR_EP");
            }
        }
        if (lightsOutEnabled)
        {
            int _lightsOutCompleted = ProgramManager.instance.saveData.GetNumCompletedLightsOut();
            JetEngine.SteamUtils.TrySetStat("lo_count", _lightsOutCompleted);
            if (_lightsOutCompleted == 32)
            {
                JetEngine.SteamUtils.TryGetAchievement("CHALLENGECLEAR_LO");
            }
        }
        if (spectralShuffleEnabled)
        {
            int _spectralShuffleCompleted = ProgramManager.instance.saveData.GetNumCompletedSpectralShuffle();
            JetEngine.SteamUtils.TrySetStat("ss_count", _spectralShuffleCompleted);
            if (_spectralShuffleCompleted == 32)
            {
                JetEngine.SteamUtils.TryGetAchievement("CHALLENGECLEAR_SS");
            }
        }
        if (ectoplasmEnabled || lightsOutEnabled || spectralShuffleEnabled)
        {
            int _crowned = ProgramManager.instance.saveData.GetNumCrownedLevels();
            JetEngine.SteamUtils.TrySetStat("crown_count", _crowned);
            if (_crowned == 32)
            {
                JetEngine.SteamUtils.TryGetAchievement("CHALLENGECLEAR_ALL");
            }
        }

        ProgramManager.instance.saveData.SaveSaveData();
    }
}
