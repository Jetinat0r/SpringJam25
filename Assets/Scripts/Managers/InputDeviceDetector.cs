using UnityEngine;
using TMPro;

public class InputDeviceDetector : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer glyphSprite;
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
        Sprite _newSprite = InputOverlord.instance.GetGlyph(action);
        if (_newSprite != null)
        {
            SetGlyphSprite(_newSprite);
        }
        else
        {
            SetFallbackText(InputOverlord.instance.GetFallbackText(action));
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
}
