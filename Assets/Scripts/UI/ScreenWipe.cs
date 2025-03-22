using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//controls "wipe" effect that occurs between scene changes
public class ScreenWipe : MonoBehaviour
{
    public Action PostWipe;
    public Action PostUnwipe;
    public static bool over = false;
    public static ScreenWipe current;
    [SerializeField] private Image ScreenBlocker;

    public void Awake()
    {
        WipeOut();
        current = this;
    }

    public void WipeIn()
    {
        over = false;
        ScreenBlocker.raycastTarget = true;
        GetComponent<Animator>().SetTrigger("WipeIn");
    }

    public void WipeOut()
    {
        GetComponent<Animator>().SetTrigger("WipeOut");
        ScreenBlocker.raycastTarget = false;
    }

    public void CallPostWipe()
    {
        PostWipe?.Invoke();
        PostWipe = null;
    }

    public void ScreenRevealed()
    {
        PostUnwipe?.Invoke();
        PostUnwipe = null;
        Invoke("PostCooldown", 0.1f);
    }

    public void PostCooldown()
    {
        over = true;
        ScreenBlocker.raycastTarget = false;
    }
}