using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Button clickableLevelButton;
    
    public string levelName = "Level";

    public bool isLocked = true;

    public GameObject levelText;
    public GameObject levelLockIcon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        if (isLocked)
        {
            LockLevel();
        }
        else
        {
            UnlockLevel();
        }
        */
    }

    public void LockLevel()
    {
        isLocked = true;
        clickableLevelButton.interactable = false;
        levelText.SetActive(false);
        levelLockIcon.SetActive(true);
    }

    public void UnlockLevel()
    {
        isLocked = false;
        clickableLevelButton.interactable = true;
        levelText.SetActive(true);
        levelLockIcon.SetActive(false);
    }
}
