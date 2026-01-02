using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GlyphMap : MonoBehaviour
{
    [System.Serializable]
    public class GlyphEntry
    {
        public string inputPath;
        public Sprite glyph;
    }

    //Input lists:
    //  Keyboard: https://discussions.unity.com/t/list-of-all-inputcontrolpath/909946/10
    //  Gamepad: https://discussions.unity.com/t/how-can-you-get-all-gamepad-paths-via-code/845873/7

    [SerializeField]
    //USE ACCESS FUNCTIONS INSTEAD
    public List<GlyphEntry> keyboardGlyphMap;
    private Dictionary<string, GlyphEntry> keyboardGlyphDict = null;
    [SerializeField]
    //USE ACCESS FUNCTIONS INSTEAD
    public List<GlyphEntry> genericGlyphMap;
    private Dictionary<string, GlyphEntry> genericGlyphDict = null;
    [SerializeField]
    //USE ACCESS FUNCTIONS INSTEAD
    public List<GlyphEntry> nintendoGlyphMap;
    private Dictionary<string, GlyphEntry> nintendoGlyphDict = null;
    [SerializeField]
    //USE ACCESS FUNCTIONS INSTEAD
    public List<GlyphEntry> playstationGlyphMap;
    private Dictionary<string, GlyphEntry> playstationGlyphDict = null;
    [SerializeField]
    //USE ACCESS FUNCTIONS INSTEAD
    public List<GlyphEntry> xboxGlyphMap;
    private Dictionary<string, GlyphEntry> xboxGlyphDict = null;

    //Create slightly faster methods of accessing each element
    private void Awake()
    {
        keyboardGlyphDict = new Dictionary<string, GlyphEntry>(keyboardGlyphMap.Count);
        foreach (GlyphEntry g in keyboardGlyphMap)
        {
            keyboardGlyphDict.Add(g.inputPath, g);
        }

        genericGlyphDict = new Dictionary<string, GlyphEntry>(genericGlyphMap.Count);
        foreach (GlyphEntry g in genericGlyphMap)
        {
            genericGlyphDict.Add(g.inputPath, g);
        }

        nintendoGlyphDict = new Dictionary<string, GlyphEntry>(nintendoGlyphMap.Count);
        foreach (GlyphEntry g in nintendoGlyphMap)
        {
            nintendoGlyphDict.Add(g.inputPath, g);
        }

        playstationGlyphDict = new Dictionary<string, GlyphEntry>(playstationGlyphMap.Count);
        foreach (GlyphEntry g in playstationGlyphMap)
        {
            playstationGlyphDict.Add(g.inputPath, g);
        }

        xboxGlyphDict = new Dictionary<string, GlyphEntry>(xboxGlyphMap.Count);
        foreach (GlyphEntry g in xboxGlyphMap)
        {
            xboxGlyphDict.Add(g.inputPath, g);
        }
    }

    public Sprite GetGlyph(InputOverlord.SUPPORTED_CONTROL_TYPE _controlType, string _inputPath)
    {
        if (Regex.IsMatch(_inputPath, "<.+>/(.*)"))
        {
            _inputPath = Regex.Match(_inputPath, "<.+>/(.*)").Groups[1].Value;
        }

        GlyphEntry _glyphEntry;
        switch (_controlType)
        {
            case InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD:
                _glyphEntry = keyboardGlyphDict.GetValueOrDefault(_inputPath, null);
                break;
            case InputOverlord.SUPPORTED_CONTROL_TYPE.NINTENDO:
                _glyphEntry = nintendoGlyphDict.GetValueOrDefault(_inputPath, null);
                break;
            case InputOverlord.SUPPORTED_CONTROL_TYPE.PLAYSTATION:
                _glyphEntry = playstationGlyphDict.GetValueOrDefault(_inputPath, null);
                break;
            case InputOverlord.SUPPORTED_CONTROL_TYPE.XBOX:
                _glyphEntry = xboxGlyphDict.GetValueOrDefault(_inputPath, null);
                break;
            case InputOverlord.SUPPORTED_CONTROL_TYPE.NONE:
            case InputOverlord.SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD:
            case InputOverlord.SUPPORTED_CONTROL_TYPE.UNKNOWN:
            default:
                _glyphEntry = genericGlyphDict.GetValueOrDefault(_inputPath, null);
                break;
        }

        if (_glyphEntry != null)
        {
            return _glyphEntry.glyph;
        }
        else
        {
            return null;
        }
    }
}
