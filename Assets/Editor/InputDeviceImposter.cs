using UnityEditor;
using UnityEngine;

//Custom Editor class to pretend to be different input devices so I don't have to plug in various controllers to test things
public class InputDeviceImposter : MonoBehaviour
{
    /*
    [MenuItem("PhantomFeline/InputDeviceImposter/Enable")]
    static void EnableInputDeviceImposter()
    {
        
    }

    [MenuItem("PhantomFeline/InputDeviceImposter/Disable")]
    static void DisableInputDeviceImposter()
    {

    }
    */

    [MenuItem("PhantomFeline/InputDeviceImposter/Keyboard")]
    static void MimicKeyboard()
    {
        if (Application.isPlaying)
        {
            InputOverlord.instance.currentInputDeviceType = InputOverlord.SUPPORTED_CONTROL_TYPE.KEYBOARD;
            InputOverlord.instance.ControlsChangedSignal?.Invoke();
            Debug.Log("Mimicing Keyboard");
        }
        else
        {
            Debug.LogWarning("Can't mimic input devices outside of playmode");
        }
    }

    [MenuItem("PhantomFeline/InputDeviceImposter/Generic")]
    static void MimicGeneric()
    {
        if (Application.isPlaying)
        {
            InputOverlord.instance.currentInputDeviceType = InputOverlord.SUPPORTED_CONTROL_TYPE.GENERIC_GAMEPAD;
            InputOverlord.instance.ControlsChangedSignal?.Invoke();
            Debug.Log("Mimicing Generic Gamepad");
        }
        else
        {
            Debug.LogWarning("Can't mimic input devices outside of playmode");
        }
    }

    [MenuItem("PhantomFeline/InputDeviceImposter/Nintendo")]
    static void MimicNintendo()
    {
        if (Application.isPlaying)
        {
            InputOverlord.instance.currentInputDeviceType = InputOverlord.SUPPORTED_CONTROL_TYPE.NINTENDO;
            InputOverlord.instance.ControlsChangedSignal?.Invoke();
            Debug.Log("Mimicing Nintendo Gamepad");
        }
        else
        {
            Debug.LogWarning("Can't mimic input devices outside of playmode");
        }
    }

    [MenuItem("PhantomFeline/InputDeviceImposter/Playstation")]
    static void MimicPlaystation()
    {
        if (Application.isPlaying)
        {
            InputOverlord.instance.currentInputDeviceType = InputOverlord.SUPPORTED_CONTROL_TYPE.PLAYSTATION;
            InputOverlord.instance.ControlsChangedSignal?.Invoke();
            Debug.Log("Mimicing Playstation Gamepad");
        }
        else
        {
            Debug.LogWarning("Can't mimic input devices outside of playmode");
        }
    }

    [MenuItem("PhantomFeline/InputDeviceImposter/Xbox")]
    static void MimicXbox()
    {
        if (Application.isPlaying)
        {
            InputOverlord.instance.currentInputDeviceType = InputOverlord.SUPPORTED_CONTROL_TYPE.XBOX;
            InputOverlord.instance.ControlsChangedSignal?.Invoke();
            Debug.Log("Mimicing Xbox Gamepad");
        }
        else
        {
            Debug.LogWarning("Can't mimic input devices outside of playmode");
        }
    }
}
