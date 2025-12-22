using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootScreen : MonoBehaviour
{
    [SerializeField]
    //Time spent on the first frame of the boot animation
    public float timeBeforeAnimationStart = 0.25f;
    //Time spent on the last frame of the boot animation
    [SerializeField]
    public float timeAfterAnimationEnd = 0.5f;
    [SerializeField]
    public Animator bootAnimation;

    [SerializeField]
    public MainMenuManager mainMenu;

    [SerializeField]
    public List<Image> sidePanels = new();
    private List<Color> sidePanelOriginalColors = null;

    [SerializeField]
    private SoundPlayer soundPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Store side panel colors in case we need them
        sidePanelOriginalColors = new List<Color>(sidePanels.Count);
        for (int i = 0; i < sidePanels.Count; i++)
        {
            sidePanelOriginalColors.Add(sidePanels[i].color);
        }

        if (ProgramManager.instance.saveData.SkipIntro || !ProgramManager.instance.firstOpen)
        {
            MoveToMainMenu();
        }
        else
        {
            ShaderManager.instance.DisableShaders();
            gameObject.SetActive(true);

            //Temporarily change side panel colors because we've disabled shaders
            Color _sidePanelColor = ShaderManager.instance.GetActiveColor(0);
            for (int i = 0; i < sidePanels.Count; i++)
            {
                sidePanels[i].color = _sidePanelColor;
            }

            //Delay, and then start animation
            Invoke(nameof(StartAnimation), timeBeforeAnimationStart);
        }
    }

    public void StartAnimation()
    {
        bootAnimation.Play("IntroAnimation_Clip");
        soundPlayer.PlaySound("UI.Startup");
    }

    public void OnAnimationComplete()
    {
        //Delay, then move to Start Screen
        Invoke(nameof(MoveToMainMenu), timeAfterAnimationEnd);
    }

    public void MoveToMainMenu()
    {
        //Reset panel colors to allow shaders to work their magic
        for (int i = 0; i < sidePanels.Count; i++)
        {
            sidePanels[i].color = sidePanelOriginalColors[i];
        }

        gameObject.SetActive(false);
        ShaderManager.instance.EnableShaders();
        mainMenu.StartupMainMenu();
    }
}
