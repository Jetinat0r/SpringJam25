using UnityEditor;
using UnityEngine;

public class ToggleChallenges : MonoBehaviour
{
    private const string ECTOPLASM_KEY = "EditorEctoplasmEnabled";
    private const string LIGHTS_OUT_KEY = "EditorLightsOutEnabled";
    private const string SPECTRAL_SHUFFLE_KEY = "EditorSpectralShuffleEnabled";

    [MenuItem("PhantomFeline/Challenges/Display Challenge States")]
    static void DisplayChallengeStates()
    {
        bool _ectoplasm = EditorPrefs.GetBool(ECTOPLASM_KEY, false);
        bool _lightsOut = EditorPrefs.GetBool(LIGHTS_OUT_KEY, false);
        bool _spectralShuffle = EditorPrefs.GetBool(SPECTRAL_SHUFFLE_KEY, false);

        Debug.Log($"Ectoplasm: {(_ectoplasm ? "ENABLED" : "DISABLED")} | Lights Out: {(_lightsOut ? "ENABLED" : "DISABLED")} | Spectral Shuffle: {(_spectralShuffle ? "ENABLED" : "DISABLED")}");
    }

    [MenuItem("PhantomFeline/Challenges/Toggle Ectoplasm")]
    static void ToggleEctoplasm()
    {
        bool _ectoplasm = !EditorPrefs.GetBool(ECTOPLASM_KEY, false);
        EditorPrefs.SetBool(ECTOPLASM_KEY, _ectoplasm);

        if (Application.isPlaying)
        {
            ChallengeManager.instance.ectoplasmEnabled = _ectoplasm;
        }

        Debug.Log($"Ectoplasm Mode {(_ectoplasm ? "ENABLED" : "DISABLED")}");
    }

    [MenuItem("PhantomFeline/Challenges/Toggle Lights Out")]
    static void ToggleLightsOut()
    {
        bool _lightsOut = !EditorPrefs.GetBool(LIGHTS_OUT_KEY, false);
        EditorPrefs.SetBool(LIGHTS_OUT_KEY, _lightsOut);

        if (Application.isPlaying)
        {
            ChallengeManager.instance.lightsOutEnabled = _lightsOut;
        }

        Debug.Log($"Lights Out Mode {(_lightsOut ? "ENABLED" : "DISABLED")}");
    }

    [MenuItem("PhantomFeline/Challenges/Toggle Spectral Shuffle")]
    static void ToggleSpectralShuffle()
    {
        bool _spectralShuffle = !EditorPrefs.GetBool(SPECTRAL_SHUFFLE_KEY, false);
        EditorPrefs.SetBool(SPECTRAL_SHUFFLE_KEY, _spectralShuffle);

        if (Application.isPlaying)
        {
            ChallengeManager.instance.spectralShuffleEnabled = _spectralShuffle;
        }

        Debug.Log($"Spectral Shuffle Mode {(_spectralShuffle ? "ENABLED" : "DISABLED")}");
    }
}
