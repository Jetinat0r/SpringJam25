using UnityEngine;

public class VFX : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("I was summoned!!");
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
}
