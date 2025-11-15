using UnityEngine;

public class CamFlip : MonoBehaviour
{
    [SerializeField]
    public Vector3 camScale = new Vector3(-1, 1, 1);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //https://discussions.unity.com/t/flip-mirror-camera/4804
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat *= Matrix4x4.Scale(camScale);
        Camera.main.projectionMatrix = mat;
    }
}
