using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardBindingGlyphDisplay : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetGlyph();

        InputOverlord.instance.ControlsChangedSignal += GetGlyph;
    }

    private void OnDestroy()
    {
        InputOverlord.instance.ControlsChangedSignal -= GetGlyph;
    }

    public void GetGlyph()
    {
        Sprite _newSprite = InputOverlord.instance.GetGlyph(action, InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD);
        if (_newSprite != null)
        {
            SetGlyphSprite(_newSprite);
        }
        else
        {
            SetFallbackText(InputOverlord.instance.GetFallbackText(action, InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD));
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
        rebindingSettingsMenu.RebindControl(action, InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD, rebindButton);
    }
}
