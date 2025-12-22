using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RebindingSettingsMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject rebindingPopupMenu;
    [SerializeField]
    public TMP_Text popupTitle;
    [SerializeField]
    public TMP_Text popupActionName;
    [SerializeField]
    public KeyboardBindingGlyphDisplay popupKeyboardGlyph;
    [SerializeField]
    public GamepadBindingGlyphDisplay popupGamepadGlyph;

    [SerializeField]
    public GameObject resetBindingsPopupMenu;
    [SerializeField]
    public Button resetBindingsPopupFirstSelected;
    [SerializeField]
    public Button resetBindingsButton;
    [SerializeField]
    public SoundPlayer soundPlayer;


    public void RebindControl(InputOverlord.INPUT_ACTION _inputAction, InputOverlord.SUPPORTED_CONTROL_TYPE _controlType, Selectable _returnTarget)
    {
        rebindingPopupMenu.SetActive(true);

        popupKeyboardGlyph.action = _inputAction;
        popupGamepadGlyph.action = _inputAction;

        //Setup Keyboard or Gamepad Visuals
        if (_controlType == InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD)
        {
            popupTitle.text = "Rebinding\nKEYBOARD Action:";
            popupKeyboardGlyph.gameObject.SetActive(true);
            popupKeyboardGlyph.GetGlyph();

            popupGamepadGlyph.gameObject.SetActive(false);
        }
        else
        {
            popupTitle.text = "Rebinding\nGAMEPAD Action:";
            popupGamepadGlyph.gameObject.SetActive(true);
            popupGamepadGlyph.GetGlyph();

            popupKeyboardGlyph.gameObject.SetActive(false);
        }

        //Setup proper Action text
        switch (_inputAction)
        {
            case InputOverlord.INPUT_ACTION.UP:
                popupActionName.SetText("UP");
                break;
            case InputOverlord.INPUT_ACTION.DOWN:
                popupActionName.SetText("DOWN");
                break;
            case InputOverlord.INPUT_ACTION.LEFT:
                popupActionName.SetText("LEFT");
                break;
            case InputOverlord.INPUT_ACTION.RIGHT:
                popupActionName.SetText("RIGHT");
                break;
            case InputOverlord.INPUT_ACTION.INTERACT:
                popupActionName.SetText("INTERACT");
                break;
            case InputOverlord.INPUT_ACTION.SHADOW:
                popupActionName.SetText("SHADOW");
                break;
            case InputOverlord.INPUT_ACTION.PAUSE:
                popupActionName.SetText("PAUSE");
                break;
            case InputOverlord.INPUT_ACTION.RESTART:
                popupActionName.SetText("RESTART");
                break;
            default:
                popupActionName.SetText("???");
                break;
        }

        //Effectively disable controls
        EventSystem.current.SetSelectedGameObject(null);
        MenuPanelWatcher.instance.activePanel = MenuPanel.POPUP;

        soundPlayer.PlaySound("UI.Move");

        //Start Rebind Operation
        InputOverlord.instance.RebindControl(_inputAction, _controlType,
            () =>
            {
                //Success Operation
                soundPlayer.PlaySound("UI.Select");
                ClosePopupMenu(_returnTarget);
            },
            () =>
            {
                //Failure Operation
                soundPlayer.PlaySound("UI.Back");
                ClosePopupMenu(_returnTarget);
            });
    }

    public void ClosePopupMenu(Selectable _returnTarget)
    {
        MenuPanelWatcher.instance.activePanel = MenuPanel.SETTINGS;
        EventSystem.current.SetSelectedGameObject(_returnTarget.gameObject);

        rebindingPopupMenu.SetActive(false);
    }

    public void ResetBindings()
    {
        soundPlayer.PlaySound("UI.Select");
        InputOverlord.instance.ResetBindings();
    }

    public void OpenResetBindingsPopup()
    {
        resetBindingsPopupMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(resetBindingsPopupFirstSelected.gameObject);
        MenuPanelWatcher.instance.activePanel = MenuPanel.POPUP;
    }

    public void CloseResetBindingsPopup()
    {
        resetBindingsPopupMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(resetBindingsButton.gameObject);
        MenuPanelWatcher.instance.activePanel = MenuPanel.SETTINGS;
    }
}
