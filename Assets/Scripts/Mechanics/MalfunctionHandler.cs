using DG.Tweening;
using UnityEngine;

public class MalfunctionHandler : MonoBehaviour
{
    public IToggleable toggleable;

    public int numFlickers = 3;
    public float flickerPeriod = 0.1f;
    public float downTime = 1f;

    private Sequence flickerSequence;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ChallengeManager.instance.spectralShuffleEnabled)
        {
            //Don't destroy the whole gameobject, as these will be attached to lights and such XP
            Destroy(this);
            return;
        }

        //Grab closest toggleable
        toggleable = GetComponent<IToggleable>();

        flickerSequence = DOTween.Sequence();
        flickerSequence.onKill += () => { flickerSequence = null; };
        flickerSequence.AppendInterval(downTime);
        for (int i = 0; i < numFlickers; i++)
        {
            flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });
            flickerSequence.AppendInterval(flickerPeriod);
            flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });
            flickerSequence.AppendInterval(flickerPeriod);
        }
        flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });

        flickerSequence.SetLoops(-1);
        flickerSequence.Play();
    }

    private void OnDestroy()
    {
        flickerSequence?.Kill();
    }
}
