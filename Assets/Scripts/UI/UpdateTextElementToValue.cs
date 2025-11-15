using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateTextElementToValue : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textElement;
    [SerializeField]
    private Slider parentSlider;

    private void Start()
    {
        if (textElement == null)
        {
            textElement = GetComponent<TMP_Text>();
        }

        textElement.text = parentSlider.value.ToString();
    }

    public void OnValueChange(System.Single _newValue)
    {
        textElement.text = _newValue.ToString();
    }
}
