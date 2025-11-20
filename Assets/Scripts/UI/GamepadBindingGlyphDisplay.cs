using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamepadBindingGlyphDisplay : MonoBehaviour
{
    [SerializeField]
    public RebindingSettingsMenu rebindingSettingsMenu;

    [SerializeField]
    public Selectable rebindButton;

    [SerializeField]
    public Image glyphSprite;
    [SerializeField]
    public TMP_Text fallbackText;

    [SerializeField]
    public InputOverlord.INPUT_ACTION action;
    [SerializeField]
    public InputOverlord.SUPPORTED_CONTROL_TYPE activeControllerType = InputOverlord.SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InputOverlord.instance.currentInputDeviceType != InputOverlord.SUPPORTED_CONTROL_TYPE.NONE &&
            InputOverlord.instance.currentInputDeviceType != InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD)
        {
            activeControllerType = InputOverlord.instance.currentInputDeviceType;
        }
        GetGlyph();

        InputOverlord.instance.ControlsChangedSignal += GetGlyph;
    }

    private void OnDestroy()
    {
        InputOverlord.instance.ControlsChangedSignal -= GetGlyph;
    }

    public void GetGlyph()
    {
        if (InputOverlord.instance.currentInputDeviceType == InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD ||
            InputOverlord.instance.currentInputDeviceType == InputOverlord.SUPPORTED_CONTROL_TYPE.NONE)
        {
            //Do not change activeControllerType
        }
        else
        {
            activeControllerType = InputOverlord.instance.currentInputDeviceType;
        }

        Sprite _newSprite = InputOverlord.instance.GetGlyph(action, activeControllerType);
        if (_newSprite != null)
        {
            SetGlyphSprite(_newSprite);
        }
        else
        {
            SetFallbackText(InputOverlord.instance.GetFallbackText(action, activeControllerType));
        }
    }

    public void SetGlyphSprite(Sprite _newSprite)
    {
        glyphSprite.sprite = _newSprite;
        glyphSprite.gameObject.SetActive(true);

        fallbackText.gameObject.SetActive(false);
    }

    public void SetFallbackText(string _newText)
    {
        fallbackText.SetText(_newText);
        fallbackText.gameObject.SetActive(true);

        glyphSprite.gameObject.SetActive(false);
    }

    public void StartRebind()
    {
        rebindingSettingsMenu.RebindControl(action, activeControllerType, rebindButton);
    }
}
