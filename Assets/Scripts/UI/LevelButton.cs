using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Button clickableLevelButton;
    public SelectOnHover selectOnHoverComponent;
    
    public string levelName = "Level";

    public bool isLocked = true;

    public GameObject levelText;
    public GameObject levelLockIcon;
    public GameObject ectoplasmIcon;
    public GameObject lightsOutIcon;
    public GameObject spectralShuffleIcon;
    public GameObject poltergeistIcon;

    public void UpdateState(bool _isUnlocked, SaveData.LevelSaveData _levelSaveData)
    {
        HideChallengeBadges();

        if (_isUnlocked || _levelSaveData.completed)
        {
            UnlockLevel();
            ShowCompletedChallengeBadges(_levelSaveData);
        }
        else
        {
            LockLevel();
        }
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

    public void HideChallengeBadges()
    {
        ectoplasmIcon.SetActive(false);
        lightsOutIcon.SetActive(false);
        spectralShuffleIcon.SetActive(false);
        poltergeistIcon.SetActive(false);
    }

    public void ShowCompletedChallengeBadges(SaveData.LevelSaveData _levelSaveData)
    {
        if (_levelSaveData.challenges.beatEctoplasm)
        {
            ectoplasmIcon.SetActive(true);
        }

        if (_levelSaveData.challenges.beatLightsOut)
        {
            lightsOutIcon.SetActive(true);
        }

        if (_levelSaveData.challenges.beatSpectralShuffle)
        {
            spectralShuffleIcon.SetActive(true);
        }

        //Poltergeist has been repurposed as completion
        if (_levelSaveData.challenges.beatEctoplasm && _levelSaveData.challenges.beatLightsOut && _levelSaveData.challenges.beatSpectralShuffle)
        {
            poltergeistIcon.SetActive(true);
        }
    }
}
