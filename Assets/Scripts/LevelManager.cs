using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelMenuManager levelMenuManager;

    public string currentLevelName = "Level";
    public int currentLevelNumber = 1;

    public string nextLevelName = "Level";
    public Tilemap conveyorTilemap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }

    public void ToggleMenu(PlayerMovement player)
    {
        levelMenuManager.ToggleMenu(player);
    }

    public void ResetScene()
    {
        ScreenWipe.current.WipeIn();
        ScreenWipe.current.PostWipe += () => { SceneManager.LoadScene(currentLevelName); };
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
        ScreenWipe.current.WipeIn();
        ScreenWipe.current.PostWipe += NextScene;
    }

    public void NextScene()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public void ReturnToMainMenu()
    {
        ScreenWipe.current.WipeIn();
        ScreenWipe.current.PostWipe += () => { SceneManager.LoadScene("MainMenu"); };
    }
}
