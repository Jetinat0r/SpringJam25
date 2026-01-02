using UnityEngine;

public class CamFlip : MonoBehaviour
{
    [SerializeField]
    public LevelMenuManager levelMenuManager;

    [SerializeField]
    public string challengeName = "Flip Side";

    [SerializeField]
    public bool flipX;
    [SerializeField]
    public bool flipY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ChallengeManager.instance.spectralShuffleEnabled)
        {
            Destroy(this);
            return;
        }

        if (levelMenuManager == null)
        {
            Debug.LogWarning("Level Menu Manager not assigned, searching scene...");
            levelMenuManager = FindFirstObjectByType<LevelMenuManager>();
        }

        levelMenuManager.DisplaySpectralShuffleChallenge(challengeName);

        Vector3 _camScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1);

        //https://discussions.unity.com/t/flip-mirror-camera/4804
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat *= Matrix4x4.Scale(_camScale);
        Camera.main.projectionMatrix = mat;
    }
}
