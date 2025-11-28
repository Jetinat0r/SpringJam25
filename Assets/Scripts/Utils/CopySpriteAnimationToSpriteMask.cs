using UnityEngine;

public class CopySpriteAnimationToSpriteMask : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteMask spriteMask;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteMask == null)
        {
            spriteMask = GetComponent<SpriteMask>();
        }
    }

    private void LateUpdate()
    {
        spriteMask.sprite = spriteRenderer.sprite;
    }
}
