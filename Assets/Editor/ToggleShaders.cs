using TMPro;
using UnityEditor;
using UnityEngine;

public class ToggleShaders : MonoBehaviour
{
    [MenuItem("PhantomFeline/Enable Shaders")]
    static void EnableShaders()
    {
        AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/FullscreenPaletteSwapperShader.mat").SetFloat("_usePalette", 1f);
        if (Application.isPlaying)
        {
            Canvas.GetDefaultCanvasMaterial().SetFloat("_usePalette", 1f);
        }
        else
        {
            AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/UI_Palette.mat").SetFloat("_usePalette", 1f);
        }
        AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Early GameBoy SDF.asset").material.SetFloat("_usePalette", 1f);
    }

    [MenuItem("PhantomFeline/Disable Shaders")]
    static void DisableShaders()
    {
        AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/FullscreenPaletteSwapperShader.mat").SetFloat("_usePalette", 0f);
        if (Application.isPlaying)
        {
            Canvas.GetDefaultCanvasMaterial().SetFloat("_usePalette", 0f);
        }
        else
        {
            AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/UI_Palette.mat").SetFloat("_usePalette", 0f);
        }
        AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Early GameBoy SDF.asset").material.SetFloat("_usePalette", 0f);
    }
}
