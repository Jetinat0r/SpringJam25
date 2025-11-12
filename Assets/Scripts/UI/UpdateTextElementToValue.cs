using TMPro;
using UnityEngine;

public class UpdateTextElementToValue : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textElement;

    private void Start()
    {
        if (textElement == null)
        {
            textElement = GetComponent<TMP_Text>();
        }
    }

    public void OnValueChange(System.Single _newValue)
    {
        textElement.text = _newValue.ToString();
    }
}
