using UnityEngine;

public class Mirror : MonoBehaviour, IRotatable
{
    public void OnRotate()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, transform.localEulerAngles.z + 90));
    }
}
