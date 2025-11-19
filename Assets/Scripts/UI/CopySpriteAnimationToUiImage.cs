using UnityEngine;
using UnityEngine.UI;

//Stupid hack to work around aseprite importer not supporting UI animations
//https://www.reddit.com/r/Unity2D/comments/1ev2xjg/aseprite_importer_animation_clips_using_image/
public class CopySpriteAnimationToUiImage : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Image image;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void LateUpdate()
    {
        image.sprite = spriteRenderer.sprite;
    }
}
