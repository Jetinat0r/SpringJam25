using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public string currentLevelName = "Level";
    public int currentLevelNumber = 1;

    public string nextLevelName = "Level";

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(currentLevelName);
    }
    
    public void CompleteLevel()
    {
        if (SettingsManager.completedLevels < currentLevelNumber)
        {
            SettingsManager.completedLevels = currentLevelNumber;
            SettingsManager.SaveSettings();
        }

        //TODO: Swap level
        SceneManager.LoadScene(nextLevelName);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
