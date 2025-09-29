using System.ComponentModel;
using UnityEngine;

public class LightHolder : MonoBehaviour, IToggleable
{
    [SerializeField]
    private Transform _customMagicLinePivot;
    public Transform CustomMagicLinePivot { get => _customMagicLinePivot != null ? _customMagicLinePivot : transform; set => _customMagicLinePivot = value; }

    [SerializeField] private GameObject[] affectedObjs;


    public void OnToggle()
    {
        foreach (GameObject obj in affectedObjs)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}