using UnityEngine;

public class EasterEggTrigger : MonoBehaviour
{
    public delegate void EasterEggTriggerEvent();
    public event EasterEggTriggerEvent onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter?.Invoke();
        gameObject.SetActive(false);
    }
}
