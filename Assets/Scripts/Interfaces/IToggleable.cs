using UnityEngine;

public interface IToggleable
{
    public Transform CustomMagicLinePivot { get; set; }

    void OnToggle();
}
