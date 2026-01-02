using JetEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettingsMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject windowModeSettingParent;
    // [SerializeField]
    // public TMP_Dropdown windowModeDropdown;
    [SerializeField]
    public Toggle vsyncToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Just in case apply display settings here
        if (ProgramManager.instance.saveData.DisplaySettings.fullScreenMode < 0 || ProgramManager.instance.saveData.DisplaySettings.fullScreenMode >= 3)
        {
            ProgramManager.instance.saveData.DisplaySettings.fullScreenMode = 0;
        }

        // windowModeDropdown.SetValueWithoutNotify(ProgramManager.instance.saveData.DisplaySettings.fullScreenMode);
        vsyncToggle.SetIsOnWithoutNotify(ProgramManager.instance.saveData.DisplaySettings.vsync);

        //Shift down so it doesn't look gross
        RectTransform _toggleTransform = vsyncToggle.gameObject.GetComponent<RectTransform>();
        //Hard coded so it doesn't look like trash and so I don't have to work for it :)
        _toggleTransform.localPosition = new Vector3(_toggleTransform.localPosition.x, 0, _toggleTransform.localPosition.z);
        
        QualitySettings.vSyncCount = ProgramManager.instance.saveData.DisplaySettings.vsync ? 1 : 0;
    }

    public void UpdateFullScreenMode(System.Int32 _newIndex)
    {
        // ProgramManager.instance.saveData.DisplaySettings.fullScreenMode = _newIndex;
        // Screen.fullScreenMode = ProgramManager.IndexToFullScreenMode(_newIndex);

        // ProgramManager.instance.saveData.SaveSaveData();
    }

    public void UpdateVSync(bool _newVSync)
    {
        ProgramManager.instance.saveData.DisplaySettings.vsync = _newVSync;
        QualitySettings.vSyncCount = _newVSync ? 1 : 0;

        ProgramManager.instance.saveData.SaveSaveData();
    }
}
