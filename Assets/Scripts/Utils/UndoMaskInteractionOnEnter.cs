using UnityEngine;

//This script exists only for level 8's key
public class UndoMaskInteractionOnEnter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        }
    }
}
