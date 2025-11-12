using UnityEngine;

public class AudioSettingsMenu : MonoBehaviour
{
    public LabelledSliderLinker musicSetting, sfxSetting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Correct potentially invalid settings
        ProgramManager.instance.saveData.AudioSettings.musicVolume = Mathf.Clamp(ProgramManager.instance.saveData.AudioSettings.musicVolume, 0f, 100f);
        ProgramManager.instance.saveData.AudioSettings.sfxVolume = Mathf.Clamp(ProgramManager.instance.saveData.AudioSettings.sfxVolume, 0f, 100f);

        //These no longer behave as linked sliders, so we want to update the static text as well!
        //musicSetting.SetValueNotify(Mathf.Pow(10, ProgramManager.instance.saveData.AudioSettings.musicVolume / 20) * 100 - 0.00001f);
        //sfxSetting.SetValueNotify(Mathf.Pow(10, ProgramManager.instance.saveData.AudioSettings.sfxVolume / 20) * 100 - 0.00001f);
        musicSetting.SetValueNotify(ProgramManager.instance.saveData.AudioSettings.musicVolume);
        sfxSetting.SetValueNotify(ProgramManager.instance.saveData.AudioSettings.sfxVolume);

        AudioManager.instance.UpdateVolume(ProgramManager.instance.saveData.AudioSettings.musicVolume, ProgramManager.instance.saveData.AudioSettings.sfxVolume);

        musicSetting.slider.onValueChanged.AddListener(OnVolumeChange);
        sfxSetting.slider.onValueChanged.AddListener(OnVolumeChange);
    }

    private void OnDestroy()
    {
        musicSetting.slider.onValueChanged.RemoveListener(OnVolumeChange);
        sfxSetting.slider.onValueChanged.RemoveListener(OnVolumeChange);
    }

    public void UpdateSliders()
    {
        musicSetting.SetValueWithoutNotify(ProgramManager.instance.saveData.AudioSettings.musicVolume);
        sfxSetting.SetValueWithoutNotify(ProgramManager.instance.saveData.AudioSettings.sfxVolume);
    }

    public void OnVolumeChange(System.Single _newVal)
    {
        float _musicVolume = musicSetting.slider.value;
        float _sfxVolume = sfxSetting.slider.value;

        //Update audio manager
        AudioManager.instance.UpdateVolume(_musicVolume, _sfxVolume);

        //Update save data
        ProgramManager.instance.saveData.AudioSettings.musicVolume = _musicVolume;
        ProgramManager.instance.saveData.AudioSettings.sfxVolume = _sfxVolume;
        ProgramManager.instance.SaveSettings();
    }
}
