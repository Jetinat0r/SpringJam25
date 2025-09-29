using UnityEngine;

public interface IRotatable
{
    public Transform CustomMagicLinePivot { get; set; }

    void OnRotate();
}
