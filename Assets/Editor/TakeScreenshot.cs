using System.IO;
using UnityEditor;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    private const string NUM_CAPTURED_SCREENSHOTS_PREF_KEY = "NumCapturedScreenshots";

    [MenuItem("PhantomFeline/Screenshots/Take Screenshot")]
    static void TakeFullscreenScreenshot()
    {
        int _numCapturedScreenshots = 0;
        if (EditorPrefs.HasKey(NUM_CAPTURED_SCREENSHOTS_PREF_KEY))
        {
            _numCapturedScreenshots = EditorPrefs.GetInt(NUM_CAPTURED_SCREENSHOTS_PREF_KEY);
        }

        string _outPath = Application.persistentDataPath + "/Screenshots";
        if (!Directory.Exists(_outPath))
        {
            Directory.CreateDirectory(_outPath);
        }
        _outPath += $"/Screenshot_{_numCapturedScreenshots}.png";
        
        ScreenCapture.CaptureScreenshot(_outPath, 1);
        Debug.Log($"Saved screenshot to {_outPath}");

        _numCapturedScreenshots += 1;
        EditorPrefs.SetInt(NUM_CAPTURED_SCREENSHOTS_PREF_KEY, _numCapturedScreenshots);
    }

    [MenuItem("PhantomFeline/Screenshots/Reset Screenshot Count")]
    static void ResetScreenshotCount()
    {
        EditorPrefs.SetInt(NUM_CAPTURED_SCREENSHOTS_PREF_KEY, 0);
        Debug.Log("Reset number of captured screenshots to 0");
    }
}
