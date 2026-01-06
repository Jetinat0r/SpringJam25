using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IdleMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool subscribed = false;
    [SerializeField] private SoundPlayer soundPlayer;

    private void Awake()
    {
        videoPlayer.url = "file://" + Application.persistentDataPath + "/" + ProgramManager.instance.demoData.idleVideoName;
    }

    private void Start()
    {
        ShaderManager.instance.DisableShaders();
        InputOverlord.instance.playerInput.actions["Interact"].started += ReturnToMainMenu;
        subscribed = true;
    }

    private void OnDestroy()
    {
        if (subscribed)
        {
            InputOverlord.instance.playerInput.actions["Interact"].started -= ReturnToMainMenu;
            subscribed = false;
        }
    }

    private void ReturnToMainMenu(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        subscribed = false;
        soundPlayer.PlaySound("UI.Select");
        InputOverlord.instance.playerInput.actions["Interact"].started -= ReturnToMainMenu;
        ScreenWipe.current.WipeIn(() => { SceneManager.LoadScene("MainMenu"); });
        ShaderManager.instance.EnableShaders();
    }
}
