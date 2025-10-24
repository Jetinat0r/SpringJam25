using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class ShaderManager : MonoBehaviour
{
    public static ShaderManager instance;

    [SerializeField]
    public Material fullscreenShaderMat;
    [SerializeField]
    public Material uiShaderMat;
    //We don't actually set the default material, so we grab it and set it here
    private Material trueUiShaderMat;
    [SerializeField]
    public Material fontShaderMat;
    [SerializeField]
    public Texture2D defaultPalette;
    private Texture2D customPalette = null;
    [SerializeField]
    private bool forceDefaultPalette;
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
        switch (_sceneName)
        {
            //World 1
            case "Level1":
            case "Level2":
            case "Level3":
            case "Level4":
            case "Level5":
            case "Level6":
            case "Level7":
            case "Level8":
                return 0;

            //World 2
            case "Level9":
            case "Level10":
            case "Level11":
            case "Level11b":
            case "Level12":
            case "Level13":
            case "Level14":
            case "Level15":
            case "Level16":
            case "Level16b":
            case "Level16c":
            case "Level16d":
                return 1;

            //Keep current palette for main menu?
            case "MainMenu":
                return curPaletteIndex;

            //No World (Fallback to 0 palette)
            //      We want to revert during development to catch errors, but maybe allow whatever later?
            default:
                return 0;
        }
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

        if (File.Exists(Application.persistentDataPath + "/CustomPalette.png"))
        {
            Texture2D _newPalette = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(_newPalette, File.ReadAllBytes(Application.persistentDataPath + "/CustomPalette.png"));
            _newPalette.filterMode = FilterMode.Point;
            _newPalette.wrapMode = TextureWrapMode.Clamp;
            //customPalette = Sprite.Create(_newPalette, new Rect(0, 0, _newPalette.width, _newPalette.height), Vector2.zero, 100, 0, SpriteMeshType.Tight);

            customPalette = _newPalette;
            hasCustomPalette = true;

            //TODO: Make this accessible to the user via the setter function?
            //TODO: Figure out what to do with forceDefaultPalette
            usingCustomPalette = true;

            Debug.Log($"Loaded Custom Palette: {Application.persistentDataPath + "/CustomPalette.png"}");
        }

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

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (useDebugPalette)
        {
            debugPalette.SetPixel(0, 0, debugPalette0);
            debugPalette.SetPixel(1, 0, debugPalette1);
            debugPalette.SetPixel(2, 0, debugPalette2);
            debugPalette.SetPixel(3, 0, debugPalette3);
            debugPalette.Apply();
        }

        UpdateAllShaderMaterialsTexture();
#endif

        //TODO: Remove from build
        UpdateAllShaderMaterialsParams();
    }

    public void SetUseCustomPalette(bool _value)
    {
        usingCustomPalette = _value;
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
        //Debug.Log($"CONDENSE: {_value}");
        paletteCondenseAmount = _value;
        fullscreenShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
        uiShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
        fontShaderMat.SetFloat("_paletteCondenseAmount", paletteCondenseAmount);
    }

    public bool CheckNeedsPaletteTransition(int _newPaletteIndex)
    {
        return _newPaletteIndex != curPaletteIndex;
    }

    public void SetupPaletteTransition(int _newPaletteIndex)
    {
        nextPaletteIndex = _newPaletteIndex;
        fullscreenShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
        uiShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
        fontShaderMat.SetFloat("_nextPaletteIndex", nextPaletteIndex);
    }

    public void UpdatePaletteMixAmount(float _value)
    {
        //Debug.Log($"MIX: {_value}");
        paletteMixAmount = _value;
        fullscreenShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
        uiShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
        fontShaderMat.SetFloat("_paletteMixAmount", paletteMixAmount);
    }

    public void EndPaletteTransition()
    {
        //We've met our next palette, so now it's our current palette!
        curPaletteIndex = nextPaletteIndex;
        fullscreenShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);
        uiShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);
        fontShaderMat.SetFloat("_curPaletteIndex", curPaletteIndex);

        //Now that we've updated our current palette, we can go back to using it
        UpdatePaletteMixAmount(0f);
    }

    private void FinishTransition()
    {

    }
}
