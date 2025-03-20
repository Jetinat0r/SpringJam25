using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LabelledSliderLinker : MonoBehaviour
{
    [SerializeField]
    public Slider slider;
    [SerializeField]
    public TMP_InputField inputField;
    [SerializeField]
    public UnityEvent OnUpdate;

    private float curValue;

    private void Start()
    {
        curValue = float.Parse(inputField.text);
    }

    public void SetValue(System.Single _newValue)
    {
        slider.SetValueWithoutNotify(_newValue);
        inputField.SetTextWithoutNotify(((int)_newValue).ToString());
        OnUpdate?.Invoke();
    }

    public void OnSliderUpdate(System.Single _newValue)
    {
        inputField.SetTextWithoutNotify(((int)_newValue).ToString());
        OnUpdate?.Invoke();
    }

    public void OnInputFieldUpdate(System.String _newValue)
    {
        if (_newValue == string.Empty)
        {
            _newValue = "0";
        }

        slider.SetValueWithoutNotify(float.Parse(_newValue));
        OnUpdate?.Invoke();
    }

    public void OnMusicVolumeUpdate()
    {
        AudioManager.instance.musicVolume = Mathf.Log10(slider.value / 100f + 0.00001f) * 20;
    }

    public void OnSoundVolumeUpdate()
    {
        AudioManager.instance.sfxVolume = Mathf.Log10(slider.value / 100f + 0.00001f) * 20;
    }
}