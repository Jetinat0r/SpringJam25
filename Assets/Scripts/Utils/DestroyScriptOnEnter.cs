using UnityEngine;

public class DestroyScriptOnEnter : MonoBehaviour
{
    [SerializeField]
    public MonoBehaviour scriptToDestroy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Hewwo?");
        Destroy(scriptToDestroy);
        Destroy(gameObject);
    }
}
