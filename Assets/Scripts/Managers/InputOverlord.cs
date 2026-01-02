using JetEngine;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputOverlord : MonoBehaviour
{
    public enum SUPPORTED_CONTROL_TYPE
    {
        NONE,
        UNKNOWN,
        KEYBOARD,
        XBOX,
        PLAYSTATION,
        NINTENDO,
        GENERIC_GAMEPAD
    }

    public enum INPUT_ACTION
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        SHADOW,
        INTERACT,
        PAUSE,
        RESTART,
        UI_ACCEPT
    }

    public static InputOverlord instance;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public InputSystemUIInputModule uiInput;
    public GlyphMap glyphMap;

    public SUPPORTED_CONTROL_TYPE currentInputDeviceType;
    public Action ControlsChangedSignal;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject _playerInputPrefab = Instantiate(Resources.Load<GameObject>("GlobalPlayerInput"), gameObject.transform);
        playerInput = _playerInputPrefab.GetComponent<PlayerInput>();
        eventSystem = _playerInputPrefab.GetComponent<EventSystem>();
        uiInput = _playerInputPrefab.GetComponent<InputSystemUIInputModule>();
        glyphMap = _playerInputPrefab.GetComponent<GlyphMap>();

        SceneManager.activeSceneChanged += OnSceneChanged;
        playerInput.onControlsChanged += OnControlsChanged;
        LoadBindingOverrides();
        //RebindControl(INPUT_ACTION.SHADOW, SUPPORTED_CONTROL_TYPE.KEYBOARD, () => { }, () => { });
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        playerInput.onControlsChanged -= OnControlsChanged;
    }

    //The stupid gameobject disables itself on scene transitions, so we have to work around it >:(
    //  I'm pretty sure it's the event system's fault
    private void OnSceneChanged(Scene arg0, Scene arg1)
    {
        eventSystem.gameObject.SetActive(true);
    }

    private void OnControlsChanged(PlayerInput obj)
    {
        SwapControls();
    }

    public void SwapControls()
    {
        if (playerInput.devices.Count > 0)
        {
            currentInputDeviceType = GetControlTypeByName(playerInput.devices[0].name);
            ControlsChangedSignal?.Invoke();
        }
        else
        {
            Debug.LogError("No input devices detected at all!");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //If no devices are plugged in (how did the game start?) default to Generic gamepad
        if (InputSystem.devices.Count == 0)
        {
           
            currentInputDeviceType = SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD;
        }
        else
        {
            bool _foundKeyboard = false;
            foreach (InputDevice _device in InputSystem.devices)
            {
                if (_device is Keyboard)
                {
                    currentInputDeviceType = SUPPORTED_CONTROL_TYPE.KEYBOARD;
                    _foundKeyboard = true;
                    break;
                }
            }

            if (!_foundKeyboard)
            {
                //If there's any gamepads, select the first one as default
                if (Gamepad.all.Count > 0)
                {
                    currentInputDeviceType = GetControlTypeByName(Gamepad.all[0].name);
                }
                //If there's any unknown gamepads, select the first one as generic
                else if (Joystick.all.Count > 0)
                {
                    currentInputDeviceType = GetControlTypeByName(Joystick.all[0].name);
                }
                else
                {
                    currentInputDeviceType = SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD;
                }
            }
        }
    }

    public SUPPORTED_CONTROL_TYPE GetControlTypeByName(string _name)
    {
        _name = _name.ToLower();

        if (_name.Contains("keyboard"))
        {
            return SUPPORTED_CONTROL_TYPE.KEYBOARD;
        }
        else if (_name.Contains("switch"))
        {
            return SUPPORTED_CONTROL_TYPE.NINTENDO;
        }
        else if (_name.Contains("xinput"))
        {
            return SUPPORTED_CONTROL_TYPE.XBOX;
        }
        else if (_name.Contains("dual"))
        {
            return SUPPORTED_CONTROL_TYPE.PLAYSTATION;
        }
        else
        {
            if (SteamUtils.IsOnSteamDeck())
            {
                return SUPPORTED_CONTROL_TYPE.XBOX;
            }
            return SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD;
        }
    }

    public InputAction GetAction(INPUT_ACTION _inputAction, SUPPORTED_CONTROL_TYPE _controlType)
    {
        switch (_inputAction)
        {
            case INPUT_ACTION.UP:
                return playerInput.actions["Move"];
            case INPUT_ACTION.DOWN:
                return playerInput.actions["Move"];
            case INPUT_ACTION.LEFT:
                return playerInput.actions["Move"];
            case INPUT_ACTION.RIGHT:
                return playerInput.actions["Move"];
            case INPUT_ACTION.SHADOW:
                return playerInput.actions["Shadow"];
            case INPUT_ACTION.INTERACT:
                return playerInput.actions["Interact"];
            case INPUT_ACTION.PAUSE:
                return playerInput.actions["ToggleUI"];
            case INPUT_ACTION.RESTART:
                return playerInput.actions["RestartLevel"];
            case INPUT_ACTION.UI_ACCEPT:
                return playerInput.actions["UiAccept"];
            default:
                return null;
        }
    }

    public int GetBindIndex(INPUT_ACTION _inputAction, SUPPORTED_CONTROL_TYPE _controlType)
    {
        //Keyboard controls are always one above the respective gamepad controls
        //  If not keyboard, assume Gamepad
        int _indexBump = _controlType == SUPPORTED_CONTROL_TYPE.KEYBOARD ? 0 : 1;
        switch (_inputAction)
        {
            case INPUT_ACTION.UP:
                return 2 + _indexBump;
            case INPUT_ACTION.DOWN:
                return 4 + _indexBump;
            case INPUT_ACTION.LEFT:
                return 6 + _indexBump;
            case INPUT_ACTION.RIGHT:
                return 8 + _indexBump;
            case INPUT_ACTION.SHADOW:
                return 0 + _indexBump;
            case INPUT_ACTION.INTERACT:
                return 0 + _indexBump;
            case INPUT_ACTION.PAUSE:
                return 0 + _indexBump;
            case INPUT_ACTION.RESTART:
                return 0 + _indexBump;
            case INPUT_ACTION.UI_ACCEPT:
                return 0 + _indexBump;
            default:
                return -1;
        }
    }

    public void RebindControl(INPUT_ACTION _inputAction, SUPPORTED_CONTROL_TYPE _controlType, Action _rebindSuccess, Action _rebindFailure)
    {
        InputAction _targetAction = GetAction(_inputAction, _controlType);
        if (_targetAction == null)
        {
            Debug.LogError("Bad Input Action!");
            _rebindFailure?.Invoke();
            return;
        }
        int _targetBindIndex = GetBindIndex(_inputAction, _controlType);

        _targetAction.Disable();
        RebindingOperation _rebindingOperation = _targetAction.PerformInteractiveRebinding(_targetBindIndex);

        if (_controlType == SUPPORTED_CONTROL_TYPE.KEYBOARD)
        {
            _rebindingOperation = _rebindingOperation
                .OnPotentialMatch(operation =>
                    {
                        /*
                        if (operation.selectedControl.parent.name == "Mouse" ||
                            operation.selectedControl.parent.name == "Gamepad" ||
                            operation.selectedControl.parent.name == "Joystick")
                        */
                        if (operation.selectedControl.parent.name != "Keyboard")
                        {
                            operation.Cancel();
                        }
                    }
                );
        }
        else
        {
            _rebindingOperation = _rebindingOperation.WithControlsExcluding("<Gamepad>/leftStick/up")
                .WithControlsExcluding("<Gamepad>/leftStick/down")
                .WithControlsExcluding("<Gamepad>/leftStick/left")
                .WithControlsExcluding("<Gamepad>/leftStick/right")
                .WithControlsExcluding("<Gamepad>/rightStick/up")
                .WithControlsExcluding("<Gamepad>/rightStick/down")
                .WithControlsExcluding("<Gamepad>/rightStick/left")
                .WithControlsExcluding("<Gamepad>/rightStick/right")
                .WithControlsExcluding("<Gamepad>/systemButton")
                .WithControlsExcluding("<Gamepad>/touchpadButton")
                .OnPotentialMatch(operation =>
                    {
                        if (operation.selectedControl.parent.name == "Mouse" ||
                            operation.selectedControl.parent.name == "Keyboard")
                        {
                            //Debug.Log(operation.selectedControl.parent.name);
                            operation.Cancel();
                        }
                    }
                );
        }

        _rebindingOperation.OnCancel((operation) =>
                {
                    Debug.Log("Rebind Cancelled");
                    _targetAction.Enable();
                    _rebindFailure?.Invoke();
                }
             )
            .OnComplete((operation) =>
                {
                    Debug.Log($"REBOUND: {_targetAction.name} | {_targetAction.bindings[_targetBindIndex].path} | {_targetAction.bindings[_targetBindIndex].effectivePath} | {_targetAction.bindings[_targetBindIndex].overridePath}");
                    _targetAction.Enable();
                    SaveBindingOverrides();
                    ControlsChangedSignal?.Invoke();
                    _rebindSuccess?.Invoke();
                }
            ).Start();
    }

    public void SaveBindingOverrides()
    {
        ProgramManager.instance.saveData.CustomBindings = playerInput.actions.SaveBindingOverridesAsJson();
        ProgramManager.instance.SaveSettings();
    }

    public void LoadBindingOverrides()
    {
        try
        {
            playerInput.actions.LoadBindingOverridesFromJson(ProgramManager.instance.saveData.CustomBindings);
            ControlsChangedSignal?.Invoke();
        }
        catch
        {
            Debug.LogWarning("Failed to load bindings from Settings; Resetting all bindings!");
            //Reset bindings calls controls changed signal
            ResetBindings();
        }
    }

    public void ResetBindings()
    {
        playerInput.actions.RemoveAllBindingOverrides();
        ProgramManager.instance.saveData.CustomBindings = string.Empty;
        ProgramManager.instance.SaveSettings();
        ControlsChangedSignal?.Invoke();
    }

    public Sprite GetGlyph(INPUT_ACTION _inputAction)
    {
        //currentInputDeviceType;
        InputAction _targetAction = GetAction(_inputAction, currentInputDeviceType);
        if (_targetAction == null)
        {
            Debug.LogError("Bad Input Action!");
            return null;
        }
        int _targetBindIndex = GetBindIndex(_inputAction, currentInputDeviceType);

        return glyphMap.GetGlyph(currentInputDeviceType, _targetAction.bindings[_targetBindIndex].effectivePath);
    }

    public Sprite GetGlyph(INPUT_ACTION _inputAction, SUPPORTED_CONTROL_TYPE _controlType)
    {
        //currentInputDeviceType;
        InputAction _targetAction = GetAction(_inputAction, _controlType);
        if (_targetAction == null)
        {
            Debug.LogError("Bad Input Action!");
            return null;
        }
        int _targetBindIndex = GetBindIndex(_inputAction, _controlType);

        return glyphMap.GetGlyph(_controlType, _targetAction.bindings[_targetBindIndex].effectivePath);
    }

    public string GetFallbackText(INPUT_ACTION _inputAction)
    {
        InputAction _targetAction = GetAction(_inputAction, currentInputDeviceType);
        if (_targetAction == null)
        {
            return "???";
        }
        int _targetBindIndex = GetBindIndex(_inputAction, currentInputDeviceType);

        return _targetAction.bindings[_targetBindIndex].ToDisplayString();
    }

    public string GetFallbackText(INPUT_ACTION _inputAction, SUPPORTED_CONTROL_TYPE _controlType)
    {
        InputAction _targetAction = GetAction(_inputAction, _controlType);
        if (_targetAction == null)
        {
            return "???";
        }
        int _targetBindIndex = GetBindIndex(_inputAction, _controlType);

        return _targetAction.bindings[_targetBindIndex].ToDisplayString();
    }
}
