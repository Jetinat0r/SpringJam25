using DG.Tweening;
using UnityEngine;

public class BetterMalfunction : MonoBehaviour
{
    public IToggleable toggleable;

    public int numFlickers = 3;
    public float flickerPeriod = 0.1f;
    public float downATime = 3f;
    public float downBTime = 0.5f;

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
        flickerSequence.AppendInterval(downATime);
        for (int i = 0; i < numFlickers; i++)
        {
            flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });
            flickerSequence.AppendInterval(flickerPeriod);
            flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });
            flickerSequence.AppendInterval(flickerPeriod);
        }
        flickerSequence.AppendCallback(() => { toggleable.OnToggle(); });
        flickerSequence.AppendInterval(downBTime);
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
