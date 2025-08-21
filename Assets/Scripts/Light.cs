using UnityEngine;

public class Light : MonoBehaviour, IToggleable
{

    public void OnToggle()
    {
        var _children = GetComponentsInChildren<Transform>();
        foreach (Transform child in _children)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
    }
}