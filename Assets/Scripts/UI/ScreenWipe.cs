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

        Sequence _sequence = DOTween.Sequence();

        //Condense palette
        _sequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(1));
        _sequence.AppendInterval(secondsPerPaletteOperation);
        _sequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(2));
        _sequence.AppendInterval(secondsPerPaletteOperation);
        _sequence.AppendCallback(() => ShaderManager.instance.UpdatePaletteCondenseAmount(3));
        _sequence.AppendInterval(secondsPerPaletteOperation);

        _sequence.onComplete += CallPostWipe;
        _sequence.Play();

        return true;
    }

    //Fades screen in
    public void WipeOut()
    {
        //Debug.Log(secondsPerPaletteOperation);
        Sequence _sequence = DOTween.Sequence();
        _sequence.AppendInterval(secondsPerPaletteOperation);

        int _levelPaletteIndex = ShaderManager.instance.GetWorldPaletteIndex(SceneManager.GetActiveScene().name);
        if (ShaderManager.instance.CheckNeedsPaletteTransition(_levelPaletteIndex))
        {
            //Swap palettes
            //TODO: STOP MUSIC TRACK
            _sequence.AppendCallback(() => ShaderManager.instance.SetupPaletteTransition(_levelPaletteIndex));
            _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(.33f); });
            _sequence.AppendInterval(secondsPerPaletteOperation);
            _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(.66f); });
            _sequence.AppendInterval(secondsPerPaletteOperation);
            _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteMixAmount(1f); });
            _sequence.AppendInterval(secondsPerPaletteOperation * 2f);
            //TODO: START NEW MUSIC TRACK
            _sequence.AppendCallback(() => { ShaderManager.instance.EndPaletteTransition(); });
        }

        //Expand palette
        _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(2); });
        _sequence.AppendInterval(secondsPerPaletteOperation);
        _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(1); });
        _sequence.AppendInterval(secondsPerPaletteOperation);
        _sequence.AppendCallback(() => { ShaderManager.instance.UpdatePaletteCondenseAmount(0); });
        //_sequence.AppendInterval(secondsPerPaletteOperation);

        _sequence.onComplete += ScreenRevealed;
        _sequence.Play();


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
        Debug.LogWarning("WIPE OFFICIALLY OVER!");
        over = true;
        ScreenBlocker.raycastTarget = false;
    }
}