using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabelledSliderLinker : MonoBehaviour
{
    [SerializeField]
    public Slider slider;
    [SerializeField]
    public TMP_InputField inputField;

    private float curValue;

    private void Start()
    {
        curValue = float.Parse(inputField.text);
    }

    public void OnSliderUpdate(System.Single _newValue)
    {
        inputField.SetTextWithoutNotify(_newValue.ToString());
    }

    public void OnInputFieldUpdate(System.String _newValue)
    {
        if (_newValue == string.Empty)
        {
            _newValue = "0";
        }

        slider.SetValueWithoutNotify(float.Parse(_newValue));
    }
}