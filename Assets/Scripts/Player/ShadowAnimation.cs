using UnityEngine;

public class ShadowAnimation : MonoBehaviour
{
    public void ShadowSetToZero()
    {
        var anim = GetComponentInParent<Animator>();

        if (anim.GetFloat("Speed") > 0)
        {
            GetComponentInParent<Animator>().SetFloat("Speed", 0);
        }
        
    }

    public void GhostSetToZero()
    {
        var anim = GetComponentInParent<Animator>();

        if (anim.GetFloat("Speed") < 0)
        {
            GetComponentInParent<Animator>().SetFloat("Speed", 0);
        }
    }
}
