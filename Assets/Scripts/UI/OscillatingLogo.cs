using UnityEngine;
using UnityEngine.UI;

public class OscillatingLogo : MonoBehaviour
{
    [SerializeField] private float floatSpd;   // This will act as the frequency modulation for the sin wave
    [SerializeField] private float incrementFactor; // keyTimer will increase by this amount every frame.
    [SerializeField] private float amplitude;    // This is the amplitude of the sin wave.
    [HideInInspector] public float timer = 0.0f;  // This will reset to 0 after it has made one full sinusoidal cycle
    [SerializeField] private Image img;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Make sprite float
        img.transform.localPosition = new Vector3(img.transform.localPosition.x, amplitude * Mathf.Sin(floatSpd * timer) * Time.fixedDeltaTime, img.transform.localPosition.z);

        // Update key timer to make key move along the sin wave and reset to 0 if it has made a full wave to prevent overflow
        timer += incrementFactor;

        if (timer >= 2 * Mathf.PI)
        {
            timer = 0.0f;
        }
    }
}
