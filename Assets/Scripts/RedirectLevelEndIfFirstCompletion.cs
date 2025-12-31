using UnityEngine;

public class RedirectLevelEndIfFirstCompletion : MonoBehaviour
{
    public LevelManager levelManager;
    [Tooltip("Should be our credits scene")]
    public string redirectedSceneName = "Credits";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ProgramManager.instance.saveData.WorldSaveData[3].levels[7].completed)
        {
            levelManager.nextLevelName = redirectedSceneName;
            ProgramManager.instance.showChallengeUnlock = true;
            Debug.Log("Redirected Victory!");
        }
        else
        {
            Debug.Log("Didn't redirect victory.");
        }
    }
}
