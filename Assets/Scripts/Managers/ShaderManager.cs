using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderManager : MonoBehaviour
{
    public static ShaderManager instance;

    [SerializeField]
    public Material fullscreenShaderMat;
    [SerializeField]
    //NOTE: DO NOT SET SHADER PROPERTIES ON THIS MATERIAL. USE trueUiShaderMat INSTEAD!!!
    public Material uiShaderMat;
    //We don't actually set the default material, so we grab it and set it here
    private Material trueUiShaderMat;
    [SerializeField]
    public Material fontShaderMat;
    [SerializeField]
    public Texture2D defaultPalette;
    private Texture2D customPalette = null;
    [SerializeField]
    private bool forceDefaultPalette = false;
    private bool hasCustomPalette = false;
    private bool usingCustomPalette = false;

    [SerializeField, Min(0)] private int curPaletteIndex = 0;
    [SerializeField, Min(0)] private int nextPaletteIndex = 0;
    [SerializeField, Range(0, 3)] private int paletteCondenseAmount = 3;
    [SerializeField, Range(0, 1)] private float paletteMixAmount = 0f;

#if UNITY_EDITOR
    public bool useDebugPalette = false;
    private Texture2D debugPalette;
    public Color debugPalette0 = new Color(15f/255f, 55f/255f, 15f/255f, 1f);
    public Color debugPalette1 = new Color(47f/255f, 98f/255f, 47f/255f, 1f);
    public Color debugPalette2 = new Color(139f/255f, 172f/255f, 15f/255f, 1f);
    public Color debugPalette3 = new Color(156f/255f, 189f/255f, 16f/255f, 1f);
#endif

    public int GetWorldPaletteIndex(string _sceneName)
    {
        //Main menu keeps current active palette
        if (_sceneName == "MainMenu")
        {
            return curPaletteIndex;
        }

        //Credits forces world 1
        if (_sceneName == "Credits")
        {
            return 0;
        }

        if (int.TryParse(Regex.Match(_sceneName, @"\d+").Value, out int _level))
        {
            if (_level >= 1 && _level <= 8)
            {
                return 0;
            }
            else if (_level >= 9 && _level <= 16)
            {
                return 1;
            }
            else if (_level >= 17 && _level <= 24)
            {
                return 2;
            }
            else if (_level >= 25 && _level <= 32)
            {
                return 3;
            }
            
            else
            {
                Debug.LogWarning($"Unknown Level [{_sceneName}] Loading! Reverting to palette 0");
                return 0;
            }
        }

        Debug.LogWarning($"Unknown Scene [{_sceneName}] Loading! Reverting to palette 0");
        return 0;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }


        fullscreenShaderMat = Resources.Load<Material>("Shaders/FullscreenPaletteSwapperShader");
        uiShaderMat = Resources.Load<Material>("Shaders/UI_Palette");
        fontShaderMat = Resources.Load<TMP_FontAsset>("Fonts/Early GameBoy SDF").material;
        defaultPalette = Resources.Load<Texture2D>("Shaders/ScreenPalette");

        if (File.Exists(Application.persistentDataPath + "/CustomPalette.png"))
        {
            Texture2D _newPalette = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(_newPalette, File.ReadAllBytes(Application.persistentDataPath + "/CustomPalette.png"));
            _newPalette.filterMode = FilterMode.Point;
            _newPalette.wrapMode = TextureWrapMode.Clamp;
            //customPalette = Sprite.Create(_newPalette, new Rect(0, 0, _newPalette.width, _newPalette.height), Vector2.zero, 100, 0, SpriteMeshType.Tight);

            customPalette = _newPalette;
            hasCustomPalette = true;

            Debug.Log($"Loaded Custom Palette: {Application.persistentDataPath + "/CustomPalette.png"}");
        }

#if UNITY_EDITOR
        debugPalette = new Texture2D(4, 1, TextureFormat.RGBA32, false);
        debugPalette.filterMode = FilterMode.Point;
        debugPalette.wrapMode = TextureWrapMode.Clamp;
#endif
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trueUiShaderMat = Canvas.GetDefaultCanvasMaterial();
        trueUiShaderMat.shader = uiShaderMat.shader;

        //Force enable all shaders on start
        fullscreenShaderMat.SetFloat("_usePalette", 1f);
        trueUiShaderMat.SetFloat("_usePalette", 1f);
        fontShaderMat.SetFloat("_usePalette", 1f);


        //Debug.Log(Canvas.GetDefaultCanvasMaterial());
        Debug.Log($"Persistent Data Path: {Application.persistentDataPath}");

        if (hasCustomPalette)
        {
            usingCustomPalette = ProgramManager.instance.saveData.UseCustomPalette;
        }

        curPaletteIndex = GetWorldPaletteIndex(ProgramManager.instance.saveData.LastPlayedLevel.ToString());

        //Init shader state
        UpdateAllShaderMaterialsTexture();
        UpdateAllShaderMaterialsParams();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            //RenderPipelineManager.endContextRendering -= BlitPaletteShader;

            //Reset palette to default image
            //  Fixes some issues in the editor
            forceDefaultPalette = true;
            UpdateAllShaderMaterialsTexture();

            ResetShaderParams();
        }
    }

#if UNITY_EDITOR
    private bool useDebugPaletteLastState = false;
    // Update is called once per frame
    void Update()
    {
        if (useDebugPalette)
        {
            debugPalette.SetPixel(0, 0, debugPalette0);
            debugPalette.SetPixel(1, 0, debugPalette1);
            debugPalette.SetPixel(2, 0, debugPalette2);
            debugPalette.SetPixel(3, 0, debugPalette3);
            debugPalette.Apply();
        }

        if (useDebugPalette != useDebugPaletteLastState)
        {
            UpdateAllShaderMaterialsTexture();

            useDebugPaletteLastState = useDebugPalette;

            Debug.Log("Ping!");
        }
    }
#endif

    public void EnableShaders()
    {
        fullscreenShaderMat.SetFloat("_usePalette", 1f);
        trueUiShaderMat.SetFloat("_usePalette", 1f);
        fontShaderMat.SetFloat("_usePalette", 1f);
    }

    public void DisableShaders()
    {
        fullscreenShaderMat.SetFloat("_usePalette", 0f);
        trueUiShaderMat.SetFloat("_usePalette", 0f);
        fontShaderMat.SetFloat("_usePalette", 0f);
    }

    public void SetUseCustomPalette(bool _value)
    {
        usingCustomPalette = _value;
        UpdateAllShaderMaterialsTexture();
    }

    public void SetForceDefualtPalette(bool _value)
    {
        forceDefaultPalette = _value;
        UpdateAllShaderMaterialsTexture();
    }

    public void ResetShaderParams()
    {
        curPaletteIndex = 0;
        nextPaletteIndex = 0;
        paletteCondenseAmount = 0;
        paletteMixAmount = 0f;

        UpdateAllShaderMaterialsParams();
    }

    private void UpdateAllShaderMaterialsTexture()
    {
        UpdateShaderMaterialTexture(fullscreenShaderMat);
        UpdateShaderMaterialTexture(trueUiShaderMat);
        UpdateShaderMaterialTexture(fontShaderMat);
    }

    private void UpdateShaderMaterialTexture(Material _shaderMat)
    {
#if UNITY_EDITOR
        //Default palette check here is for OnDestroy so it gets properly reset for the editor
        if (useDebugPalette && !forceDefaultPalette)
        {
            _shaderMat.SetTexture("_paletteImage", debugPalette);
            _shaderMat.SetVector("_paletteImageTexelSize", debugPalette.texelSize);
            return;
        }
#endif

        if (!forceDefaultPalette && hasCustomPalette && usingCustomPalette)
        {
            _shaderMat.SetTexture("_paletteImage", customPalette);
            _shaderMat.SetVector("_paletteImageTexelSize", customPalette.texelSize);
            //Debug.Log($"PALETTE: {customPalette.texelSize}");
        }
        else
        {
            _shaderMat.SetTexture("_paletteImage", defaultPalette);
            //Texel size is 1 / pixel size
            _shaderMat.SetVector("_paletteImageTexelSize", defaultPalette.texelSize);
        }
    }

    private void UpdateAllShaderMaterialsParams()
    {
        //Debug.Log("Update ALL");
        UpdateShaderMaterialParams(fullscreenShaderMat);
        UpdateShaderMaterialParams(trueUiShaderMat);
        UpdateShaderMaterialParams(fontShaderMat);
    }

    private void UpdateShaderMaterialParams(Material _shaderMat)
    {
        _shaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);
        _shaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
        _shaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
        _shaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
        //Debug.Log($"SETTINGS: {curPaletteIndex} {nextPaletteIndex} {paletteCondenseAmount} {paletteMixAmount}");
    }

    public void UpdatePaletteCondenseAmount(int _value)
    {
        paletteCondenseAmount = _value;
        
        fullscreenShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
        trueUiShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
        fontShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
    }

    public bool CheckNeedsPaletteTransition(int _newPaletteIndex)
    {
        //Debug.Log($"NEEDS PALETTE TRANSITION: {_newPaletteIndex != curPaletteIndex} [{_newPaletteIndex} | {curPaletteIndex}]");
        return _newPaletteIndex != curPaletteIndex;
    }

    public void SetupPaletteTransition(int _newPaletteIndex)
    {
        nextPaletteIndex = _newPaletteIndex;
        fullscreenShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
        trueUiShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
        fontShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
    }

    public void UpdatePaletteMixAmount(float _value)
    {
        //Debug.Log($"MIX: {_value}");
        paletteMixAmount = _value;
        fullscreenShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
        trueUiShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
        fontShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
    }

    public void EndPaletteTransition()
    {
        //We've met our next palette, so now it's our current palette!
        curPaletteIndex = nextPaletteIndex;
        fullscreenShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);
        trueUiShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);
        fontShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);

        //Now that we've updated our current palette, we can go back to using it
        UpdatePaletteMixAmount(0f);
    }

    private void FinishTransition()
    {

    }

    //External things may not modify custom palette (yet) but may ask if one exists
    public bool GetHasCustomPalette()
    {
        return hasCustomPalette;
    }

    //Index must be 0-3
    //  I plan to use this EXACTLY ONCE! If you need it for something else, be warned that it could very well not work XP
    //NOTE: DOES NOT SUPPORT DEBUG PALETTE!
    public Color GetActiveColor(int _colorIndex)
    {
        if (hasCustomPalette && usingCustomPalette)
        {
            return customPalette.GetPixel(Mathf.Min((int)((_colorIndex / 3f) * customPalette.width), customPalette.width - 1),
                Mathf.Min(curPaletteIndex, customPalette.height - 1));
        }
        else
        {
            return defaultPalette.GetPixel(Mathf.Min((int)((_colorIndex / 3f) * defaultPalette.width), defaultPalette.width - 1),
                Mathf.Min(curPaletteIndex, defaultPalette.height - 1));
        }
    }
}
