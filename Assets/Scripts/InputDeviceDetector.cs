using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

public class InputDeviceDetector : MonoBehaviour
{
    [SerializeField]
    public PlayerInput playerInput;
    [SerializeField]
    public GameObject keyboardControl;
    [SerializeField]
    public GameObject nintendoControl;
    [SerializeField]
    public GameObject xboxControl;
    [SerializeField]
    public GameObject playstationControl;
    [SerializeField]
    public GameObject defaultControl;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerInput.devices.Count > 0)
        {
            SwapControls(playerInput.devices[0].name);
        }
        else
        {
            SwapControls(string.Empty);
        }

        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDestroy()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void OnControlsChanged(PlayerInput obj)
    {
        if(playerInput.devices.Count > 0)
        {
            SwapControls(playerInput.devices[0].name);
        }
    }

    public void SwapControls(string _activeDevice)
    {
        Debug.Log(_activeDevice);

        keyboardControl.SetActive(false);
        nintendoControl.SetActive(false);
        xboxControl.SetActive(false);
        playstationControl.SetActive(false);
        defaultControl.SetActive(false);

        _activeDevice = _activeDevice.ToLower();

        if (_activeDevice.Contains("keyboard"))
        {
            keyboardControl.SetActive(true);
        }
        else if (_activeDevice.Contains("switch"))
        {
            nintendoControl.SetActive(true);
        }
        else if (_activeDevice.Contains("xinput"))
        {
            xboxControl.SetActive(true);
        }
        else if (_activeDevice.Contains("dual"))
        {
            playstationControl.SetActive(true);
        }
        else
        {
            defaultControl.SetActive(true);
        }
    }
}
