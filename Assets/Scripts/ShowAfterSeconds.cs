using UnityEngine;

public class ShowAfterSeconds : MonoBehaviour
{
    [SerializeField]
    public GameObject objectToShow;
    [SerializeField]
    public float seconds = 1f;
    private float t = 0f;

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;

        if (t > seconds)
        {
            objectToShow.SetActive(true);
        }
    }
}
