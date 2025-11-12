using UnityEngine;

public class FullScreenDetector : MonoBehaviour
{
    public static bool fullscreen;

    void Start()
    {
        fullscreen = Screen.fullScreen;
    }

    void Update()
    {
        if (Screen.fullScreen != fullscreen)
        {
            fullscreen = Screen.fullScreen;
            if (fullscreen)
            {
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
            }
            else
            {
                //SettingsManager.SetResolution(SettingsManager.resolution);
            }
        }
    }
}