using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class LevelSelectMenu : MonoBehaviour
{
    private int activeLevelPage = 0;
    [SerializeField]
    [Tooltip("Move time in seconds")]
    public float tweenMoveTime = 0.75f;
    [SerializeField]
    public Ease tweenEaseType = Ease.OutQuint;
    private Tween pageSlideTween = null;
    private Tween worldNameSlideTween = null;

    [SerializeField]
    public RectTransform worldContainerPanel;
    [SerializeField]
    public List<RectTransform> levelContainerPanels = new();
    private List<Vector2> levelContainerPanelInitialAnchoredPositions = new();

    [SerializeField]
    public RectTransform worldNameContainerPanel;
    [SerializeField]
    public List<RectTransform> worldNamePanels = new();
    private List<Vector2> worldNamePanelInitialAnchoredPositions = new();
    [SerializeField]
    public List<GameObject> worldCompletionCrowns = new();

    [Serializable]
    public class LevelButtonCollection
    {
        public List<LevelButton> levelButtons = new List<LevelButton>();
    }

    [SerializeField]
    public List<LevelButtonCollection> levelButtonCollections;

    public Button pageLeftButton;
    public GameObject leftArrow;
    public GameObject leftArrowBlock;
    public Button pageRightButton;
    public GameObject rightArrow;
    public GameObject rightArrowBlock;

    [Header("Challenge Buttons")]
    public Button lockedChallengesButton;

    public Button ectoplasmChallengeButton;
    public GameObject disabledEctoplasmIcon;
    public GameObject enabledEctoplasmIcon;

    public Button lightsOutChallengeButton;
    public GameObject disabledLightsOutIcon;
    public GameObject enabledLightsOutIcon;

    public Button spectralShuffleChallengeButton;
    public GameObject disabledSpectralShuffleIcon;
    public GameObject enabledSpectralShuffleIcon;

    [Header("Challenge Text")]
    public GameObject ectoplasmEnabledText;
    public GameObject ectoplasmDisabledText;

    public GameObject lightsOutEnabledText;
    public GameObject lightsOutDisabledText;

    public GameObject spectralShuffleEnabledText;
    public GameObject spectralShuffleDisabledText;

    private void Awake()
    {
        for (int i = 0; i < levelContainerPanels.Count; i++)
        {
            levelContainerPanelInitialAnchoredPositions.Add(levelContainerPanels[i].anchoredPosition);
        }

        for (int i = 0; i < worldNamePanels.Count; i++)
        {
            worldNamePanelInitialAnchoredPositions.Add(worldNamePanels[i].anchoredPosition);
        }
    }

    public void EnterLevelSelectMenu()
    {
        MenuPanelWatcher.instance.activePanel = MenuPanel.LEVELS;

        EventSystem.current.SetSelectedGameObject(levelButtonCollections[activeLevelPage].levelButtons[0].clickableLevelButton.gameObject);
    }

    //Locks and unlocks proper levels, challenges, and badges
    public void UpdateMenuState()
    {
        int _completedLevels = ProgramManager.instance.saveData.GetNumCompletedLevels();

        bool _allLevelsCompleted = true;
        int _fullClearedWorlds = 0;
        bool _levelUnlocked = true;
        for (int i = 0; i < 4; i++)
        {
            int _worldEctoplasmCompleted = 0;
            int _worldLightsOutCompleted = 0;
            int _worldSpectralShuffleCompleted = 0;
            for (int j = 0; j < 8; j++)
            {
                SaveData.LevelSaveData _curLevelSaveData = ProgramManager.instance.saveData.WorldSaveData[i].levels[j];

                //Update button state
                levelButtonCollections[i].levelButtons[j].UpdateState(_levelUnlocked, _curLevelSaveData);
                if (_curLevelSaveData.completed)
                {
                    //Unlock button
                    _levelUnlocked = true;
                }
                else
                {
                    //Lock button
                    _levelUnlocked = false;

                    //We can't unlock challenges
                    _allLevelsCompleted = false;
                }

                //Update badge completeness
                if (_curLevelSaveData.challenges.beatEctoplasm) _worldEctoplasmCompleted++;
                if (_curLevelSaveData.challenges.beatLightsOut) _worldLightsOutCompleted++;
                if (_curLevelSaveData.challenges.beatSpectralShuffle) _worldSpectralShuffleCompleted++;
            }

            //Check for total challenge completion
            if (_worldEctoplasmCompleted == 8 && _worldLightsOutCompleted == 8 && _worldSpectralShuffleCompleted == 8)
            {
                //Reveal world crowns
                worldCompletionCrowns[i].SetActive(true);

                _fullClearedWorlds++;
            }
            else
            {
                worldCompletionCrowns[i].SetActive(false);
            }
        }


        if (_allLevelsCompleted)
        {
            UnlockChallenges();
        }
        else
        {
            LockChallenges();
        }

        if (_fullClearedWorlds == 4)
        {
            //TODO: Final reward?
        }

        UpdateChallengeButtonDisplayStates();

        //Init Level Select Page
        ScrollToPage((ProgramManager.instance.saveData.LastPlayedLevel - 1) / 8, true);
    }

    public void UnlockChallenges()
    {
        lockedChallengesButton.gameObject.SetActive(false);

        ectoplasmChallengeButton.gameObject.SetActive(true);
        lightsOutChallengeButton.gameObject.SetActive(true);
        spectralShuffleChallengeButton.gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 4; j < 8; j++)
            {
                Navigation _nav = levelButtonCollections[i].levelButtons[j].clickableLevelButton.navigation;
                _nav.selectOnDown = ectoplasmChallengeButton;
                levelButtonCollections[i].levelButtons[j].clickableLevelButton.navigation = _nav;
            }
        }
    }

    public void LockChallenges()
    {
        lockedChallengesButton.gameObject.SetActive(true);

        ectoplasmChallengeButton.gameObject.SetActive(false);
        lightsOutChallengeButton.gameObject.SetActive(false);
        spectralShuffleChallengeButton.gameObject.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 4; j < 8; j++)
            {
                Navigation _nav = levelButtonCollections[i].levelButtons[j].clickableLevelButton.navigation;
                _nav.selectOnDown = lockedChallengesButton;
                levelButtonCollections[i].levelButtons[j].clickableLevelButton.navigation = _nav;
            }
        }
    }

    public void PageLeft()
    {
        if (activeLevelPage == 0)
        {
            //TODO: play bad sound
            return;
        }

        ScrollToPage(activeLevelPage - 1, false);
    }

    public void PageRight()
    {
        if (activeLevelPage == 3)
        {
            //TODO: play bad sound
            return;
        }

        ScrollToPage(activeLevelPage + 1, false);
    }

    public void ScrollToPage(int _destinationPage, bool _instant)
    {
        activeLevelPage = Mathf.Clamp(_destinationPage, 0, 3);

        //Update Arrow Graphics
        if (activeLevelPage == 0)
        {
            leftArrow.SetActive(false);
            leftArrowBlock.SetActive(true);

            rightArrow.SetActive(true);
            rightArrowBlock.SetActive(false);
        }
        else if (activeLevelPage >= 3)
        {
            leftArrow.SetActive(true);
            leftArrowBlock.SetActive(false);

            rightArrow.SetActive(false);
            rightArrowBlock.SetActive(true);
        }
        else
        {
            leftArrow.SetActive(true);
            leftArrowBlock.SetActive(false);

            rightArrow.SetActive(true);
            rightArrowBlock.SetActive(false);
        }

        //Update button hover selection logic
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                levelButtonCollections[i].levelButtons[j].selectOnHoverComponent.parentPanel = MenuPanel.NONE;
            }
        }

        //Allow hovering active page's buttons
        for (int j = 0; j < 8; j++)
        {
            levelButtonCollections[activeLevelPage].levelButtons[j].selectOnHoverComponent.parentPanel = MenuPanel.LEVELS;
        }

        //Allow buttons to nav to level buttons
        //Left page arrow
        Navigation _nav = pageLeftButton.navigation;
        _nav.selectOnDown = levelButtonCollections[activeLevelPage].levelButtons[0].clickableLevelButton;
        pageLeftButton.navigation = _nav;

        //Right page arrow
        _nav = pageRightButton.navigation;
        _nav.selectOnDown = levelButtonCollections[activeLevelPage].levelButtons[0].clickableLevelButton;
        pageRightButton.navigation = _nav;

        //Locked challenges button
        _nav = lockedChallengesButton.navigation;
        _nav.selectOnUp = levelButtonCollections[activeLevelPage].levelButtons[4].clickableLevelButton;
        lockedChallengesButton.navigation = _nav;

        //Ectoplasm button
        _nav = ectoplasmChallengeButton.navigation;
        _nav.selectOnUp = levelButtonCollections[activeLevelPage].levelButtons[4].clickableLevelButton;
        ectoplasmChallengeButton.navigation = _nav;

        //Lights Out button
        _nav = lightsOutChallengeButton.navigation;
        _nav.selectOnUp = levelButtonCollections[activeLevelPage].levelButtons[4].clickableLevelButton;
        lightsOutChallengeButton.navigation = _nav;

        //Spectral Shuffle button
        _nav = spectralShuffleChallengeButton.navigation;
        _nav.selectOnUp = levelButtonCollections[activeLevelPage].levelButtons[4].clickableLevelButton;
        spectralShuffleChallengeButton.navigation = _nav;

        //Tween!
        pageSlideTween?.Kill();
        worldNameSlideTween?.Kill();
        if (_instant)
        {
            worldContainerPanel.anchoredPosition = new Vector2(-levelContainerPanelInitialAnchoredPositions[activeLevelPage].x, worldContainerPanel.anchoredPosition.y);
            worldNameContainerPanel.anchoredPosition = new Vector2(-worldNamePanelInitialAnchoredPositions[activeLevelPage].x, worldNameContainerPanel.anchoredPosition.y);
        }
        else
        {
            //TODO: Play sound
            pageSlideTween = worldContainerPanel.DOAnchorPosX(-levelContainerPanelInitialAnchoredPositions[activeLevelPage].x, tweenMoveTime).SetEase(tweenEaseType);
            worldNameSlideTween = worldNameContainerPanel.DOAnchorPosX(-worldNamePanelInitialAnchoredPositions[activeLevelPage].x, tweenMoveTime).SetEase(tweenEaseType);
        }
    }

    public void UpdateChallengeButtonDisplayStates()
    {
        disabledEctoplasmIcon.SetActive(!ChallengeManager.instance.ectoplasmEnabled);
        enabledEctoplasmIcon.SetActive(ChallengeManager.instance.ectoplasmEnabled);
        ectoplasmDisabledText.SetActive(!ChallengeManager.instance.ectoplasmEnabled);
        ectoplasmEnabledText.SetActive(ChallengeManager.instance.ectoplasmEnabled);

        disabledLightsOutIcon.SetActive(!ChallengeManager.instance.lightsOutEnabled);
        enabledLightsOutIcon.SetActive(ChallengeManager.instance.lightsOutEnabled);
        lightsOutDisabledText.SetActive(!ChallengeManager.instance.lightsOutEnabled);
        lightsOutEnabledText.SetActive(ChallengeManager.instance.lightsOutEnabled);

        disabledSpectralShuffleIcon.SetActive(!ChallengeManager.instance.spectralShuffleEnabled);
        enabledSpectralShuffleIcon.SetActive(ChallengeManager.instance.spectralShuffleEnabled);
        spectralShuffleDisabledText.SetActive(!ChallengeManager.instance.spectralShuffleEnabled);
        spectralShuffleEnabledText.SetActive(ChallengeManager.instance.spectralShuffleEnabled);
    }

    public void ToggleEctoplasmMode()
    {
        //TODO: Play sound. Sound should be dependent on resultant state (good sound for enabled, bad for disabled)
        ChallengeManager.instance.ectoplasmEnabled = !ChallengeManager.instance.ectoplasmEnabled;
        UpdateChallengeButtonDisplayStates();
    }

    public void ToggleLightsOutMode()
    {
        //TODO: Play sound. Sound should be dependent on resultant state (good sound for enabled, bad for disabled)
        ChallengeManager.instance.lightsOutEnabled = !ChallengeManager.instance.lightsOutEnabled;
        UpdateChallengeButtonDisplayStates();
    }

    public void ToggleSpectralShuffleMode()
    {
        //TODO: Play sound. Sound should be dependent on resultant state (good sound for enabled, bad for disabled)
        ChallengeManager.instance.spectralShuffleEnabled = !ChallengeManager.instance.spectralShuffleEnabled;
        UpdateChallengeButtonDisplayStates();
    }

    public void ShowObject(GameObject _obj)
    {
        _obj.SetActive(true);
    }

    public void HideObject(GameObject _obj)
    {
        _obj.SetActive(false);
    }
}
