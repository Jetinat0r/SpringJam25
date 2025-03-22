using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelMenuManager levelMenuManager;

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

    public void ToggleMenu()
    {
        levelMenuManager.ToggleMenu();
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

        StartCoroutine(GoToNextLevel(1.5f));
    }

    IEnumerator GoToNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextLevelName);
    }


    public void NextScene()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
