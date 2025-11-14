using UnityEngine;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ProgramManager.instance.saveData.SkipIntro || !ProgramManager.instance.firstOpen)
        {
            MoveToMainMenu();
        }
        else
        {
            ShaderManager.instance.DisableShaders();
            gameObject.SetActive(true);

            //Delay, and then start animation
            Invoke(nameof(StartAnimation), timeBeforeAnimationStart);
        }
    }

    public void StartAnimation()
    {
        bootAnimation.Play("IntroAnimation_Clip");
    }

    public void OnAnimationComplete()
    {
        //Delay, then move to Start Screen
        Invoke(nameof(MoveToMainMenu), timeAfterAnimationEnd);
    }

    public void MoveToMainMenu()
    {
        gameObject.SetActive(false);
        ShaderManager.instance.EnableShaders();
        mainMenu.StartupMainMenu();
    }
}
