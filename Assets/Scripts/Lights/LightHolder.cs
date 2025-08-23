using System.ComponentModel;
using UnityEngine;

public class LightHolder : MonoBehaviour, IToggleable
{
    [SerializeField] private GameObject[] affectedObjs;
    public void OnToggle()
    {
        foreach (GameObject obj in affectedObjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}