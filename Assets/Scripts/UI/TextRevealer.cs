using TMPro;
using UnityEngine;

public class TextRevealer : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI textMeshPro;

    public float timePerCharacter = 0.1f;
    public bool skipSpaces = false;
    public int initialRevealedCharacters = 14;

    private int revealedCharacters;
    public bool isRevealing = false;
    private float t = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textMeshPro == null)
        {
            textMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        }

        if (initialRevealedCharacters < 0)
        {
            initialRevealedCharacters = 0;
        }
        revealedCharacters = initialRevealedCharacters;
        textMeshPro.maxVisibleCharacters = revealedCharacters;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRevealing && revealedCharacters < textMeshPro.text.Length)
        {
            t += Time.deltaTime;

            while (t >= timePerCharacter)
            {
                t -= timePerCharacter;

                while (revealedCharacters < textMeshPro.text.Length)
                {
                    revealedCharacters += 1;
                    textMeshPro.maxVisibleCharacters = revealedCharacters;
                    if (skipSpaces && textMeshPro.text[revealedCharacters - 1] == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    void StartReveal()
    {
        isRevealing = true;
    }
}
