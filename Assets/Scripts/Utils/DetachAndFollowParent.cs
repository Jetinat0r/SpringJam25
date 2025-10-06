using UnityEngine;

public class DetachAndFollowParent : MonoBehaviour
{
    private Transform parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.parent != null)
        {
            parent = transform.parent;
            transform.parent = null;
        }
        else
        {
            Debug.LogWarning($"Detach And Follow Parent is not a Child of anything! {name}");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (parent != null)
        {
            transform.position = parent.position;
        }
    }
}
