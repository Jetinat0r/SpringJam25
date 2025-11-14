using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameSettingsManager : MonoBehaviour
{
    [SerializeField]
    public Toggle skipIntroToggle;
    [SerializeField]
    public GameObject useCustomPaletteParent;
    [SerializeField]
    public Toggle useCustomPaletteToggle;
    [SerializeField]
    public Button resetProgressButton;
    [SerializeField]
    public GameObject resetProgressPopup;
    [SerializeField]
    public Button resetProgressCancelButton;

    private void Start()
    {
        skipIntroToggle.SetIsOnWithoutNotify(ProgramManager.instance.saveData.SkipIntro);
        useCustomPaletteToggle.SetIsOnWithoutNotify(ProgramManager.instance.saveData.UseCustomPalette);

        //Definitely overkill, but encompases both cases for clarity and safety
        if (ShaderManager.instance.GetHasCustomPalette())
        {
            //If we have a custom palette, include the button in navigation
            useCustomPaletteParent.SetActive(true);

            Navigation _skipIntroToggleNav = skipIntroToggle.navigation;
            _skipIntroToggleNav.selectOnDown = useCustomPaletteToggle;
            skipIntroToggle.navigation = _skipIntroToggleNav;

            Navigation _useCustomPaletteToggleNav = useCustomPaletteToggle.navigation;
            _useCustomPaletteToggleNav.selectOnUp = skipIntroToggle;
            _useCustomPaletteToggleNav.selectOnDown = resetProgressButton;
            useCustomPaletteToggle.navigation = _useCustomPaletteToggleNav;

            Navigation _resetButtonNav = resetProgressButton.navigation;
            _resetButtonNav.selectOnUp = useCustomPaletteToggle;
            resetProgressButton.navigation = _resetButtonNav;
        }
        else
        {
            //If we don't have a custom palette, remove the button from navigation
            useCustomPaletteParent.SetActive(false);

            Navigation _skipIntroToggleNav = skipIntroToggle.navigation;
            _skipIntroToggleNav.selectOnDown = resetProgressButton;
            skipIntroToggle.navigation = _skipIntroToggleNav;

            Navigation _resetButtonNav = resetProgressButton.navigation;
            _resetButtonNav.selectOnUp = skipIntroToggle;
            resetProgressButton.navigation = _resetButtonNav;
        }
    }

    public void OnUpdateSkipIntro(bool _value)
    {
        ProgramManager.instance.saveData.SkipIntro = _value;
        ProgramManager.instance.saveData.SaveSaveData();
    }

    public void OnUpdateUseCustomPalette(bool _value)
    {
        ProgramManager.instance.saveData.UseCustomPalette = _value;
        ProgramManager.instance.saveData.SaveSaveData();

        ShaderManager.instance.SetUseCustomPalette(_value);
    }

    public void OpenResetProgressPopup()
    {
        resetProgressPopup.SetActive(true);
        MenuPanelWatcher.instance.activePanel = MenuPanel.POPUP;
        EventSystem.current.SetSelectedGameObject(resetProgressCancelButton.gameObject);
        //resetProgressCancelButton.Select();
    }

    public void CloseResetProgressPopup()
    {
        MenuPanelWatcher.instance.activePanel = MenuPanel.SETTINGS;
        resetProgressPopup.SetActive(false);
        EventSystem.current.SetSelectedGameObject(resetProgressButton.gameObject);
        //resetProgressButton.Select();
    }
}
