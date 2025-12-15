using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//controls "wipe" effect that occurs between scene changes
public class ScreenWipe : MonoBehaviour
{
    public Action PostWipe;
    public Action PostUnwipe;
    public static bool over = false;
    public static ScreenWipe current;
    [SerializeField] private Image ScreenBlocker;
    private float secondsPerPaletteOperation = 0.125f;
    [SerializeField]
    public bool autoWipeOut = true;
    private Sequence curSequence;

    public void Awake()
    {
        current = this;
    }

    private void Start()
    {
        //If we want to control the wipe out from code, we need it to have not happened yet!
        if (autoWipeOut)
        {
            //Debug.Log("Wipeout!");
            WipeOut();
        }
    }

    //Returns true if the wipe starts, and false if it couldn't be started
    //  Fades screen out
    public bool WipeIn(Action _postWipeAction, bool _forceTransition = false)
    {
        //Block transitions from hapenning mid transition
        if (!_forceTransition && !over) return false;

        PostWipe = _postWipeAction;

        over = false;
        ScreenBlocker.raycastTarget = true;
        //GetComponent<Animator>().SetTrigger("WipeIn");

        curSequence?.Kill();
        curSequence = DOTween.Sequence();
        curSequence.onKill += () => { curSequence = null; };

        //Condense palette
        curSequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(1));
        curSequence.AppendInterval(secondsPerPaletteOperation);
        curSequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(2));
        curSequence.AppendInterval(secondsPerPaletteOperation);
        curSequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(3));
        curSequence.AppendInterval(secondsPerPaletteOperation);

        curSequence.onComplete += CallPostWipe;
        curSequence.Play();

        return true;
    }

    //Fades screen in
    public void WipeOut()
    {
        //Debug.Log(secondsPerPaletteOperation);
        curSequence?.Kill();
        curSequence = DOTween.Sequence();
        curSequence.onKill += () => { curSequence = null; };
        curSequence.AppendInterval(secondsPerPaletteOperation);

        int _levelPaletteIndex = ShaderManager.instance.GetWorldPaletteIndex(SceneManager.GetActiveScene().name);
        if (ShaderManager.instance.CheckNeedsPaletteTransition(_levelPaletteIndex))
        {
            //Swap palettes
            //TODO: STOP MUSIC TRACK
            curSequence.AppendCallback(() => ShaderManager.instance.SetupPaletteTransition(_levelPaletteIndex));
            curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(.33f); });
            curSequence.AppendInterval(secondsPerPaletteOperation);
            curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(.66f); });
            curSequence.AppendInterval(secondsPerPaletteOperation);
            curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(1f); });
            curSequence.AppendInterval(secondsPerPaletteOperation * 2f);
            //TODO: START NEW MUSIC TRACK
            curSequence.AppendCallback(() => { ShaderManager.instance.EndPaletteTransition(); });
        }

        //Expand palette
        curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(2); });
        curSequence.AppendInterval(secondsPerPaletteOperation);
        curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(1); });
        curSequence.AppendInterval(secondsPerPaletteOperation);
        curSequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(0); });
        //curSequence.AppendInterval(secondsPerPaletteOperation);

        curSequence.onComplete += ScreenRevealed;
        curSequence.Play();


        //GetComponent<Animator>().SetTrigger("WipeOut");
        //ScreenBlocker.raycastTarget = false;
    }

    public void CallPostWipe()
    {
        PostWipe?.Invoke();
        PostWipe = null;
        ScreenBlocker.raycastTarget = false;
    }

    public void ScreenRevealed()
    {
        PostUnwipe?.Invoke();
        PostUnwipe = null;
        Invoke(nameof(PostCooldown), 0.1f);
    }

    public void PostCooldown()
    {
        //Debug.LogWarning("WIPE OFFICIALLY OVER!");
        over = true;
        ScreenBlocker.raycastTarget = false;
    }
}