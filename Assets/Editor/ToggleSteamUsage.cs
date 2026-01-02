using JetEngine;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ToggleSteamUsage : MonoBehaviour
{
    static void GuaranteeSteamEditorPrefKey()
    {
        if (!EditorPrefs.HasKey("noSteam"))
        {
            EditorPrefs.SetBool("noSteam", false);
        }
    }

    [MenuItem("PhantomFeline/Steam/Print Steam Usage State")]
    static void PrintSteamActiveState()
    {
        GuaranteeSteamEditorPrefKey();

        Debug.Log($"Using Steam API: {!EditorPrefs.GetBool("noSteam")}");
    }

    [MenuItem("PhantomFeline/Steam/Enable Steam")]
    static void EnableSteamApi()
    {
        EditorPrefs.SetBool("noSteam", false);

        Debug.Log("Steam API Usage set to true");
    }

    [MenuItem("PhantomFeline/Steam/Disable Steam")]
    static void DisableSteamApi()
    {
        EditorPrefs.SetBool("noSteam", true);

        Debug.Log("Steam API Usage set to false; Restart Editor to guarantee this takes effect!");
    }

    [MenuItem("PhantomFeline/Steam/Reset Achievements")]
    static void ResetAchievements()
    {
        SteamUtils.ResetAchievements();
    }

    [MenuItem("PhantomFeline/Steam/Reset Achievements", true)]
    static bool ValidateResetAchievements()
    {
        return Application.isPlaying && SteamUtils.IsSteamApiLoaded();
    }
}
