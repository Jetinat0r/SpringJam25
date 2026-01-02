using System;
using System.Collections.Generic;
using UnityEngine;

public class BeatBlocks : MonoBehaviour
{
    private bool attachedAction = false;
    public GameObject[] affectedObjects;

    public int initialWaitBeats = 0;
    public int beatsBetween = 1;
    private bool hasFirstActivation = false;
    private int numBeatsSinceActivation = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ChallengeManager.instance.spectralShuffleEnabled)
        {
            Destroy(this);
        }

        AudioManager.instance.OnBeat += OnBeat;
        attachedAction = true;

        if (initialWaitBeats == 0)
        {
            hasFirstActivation = true;
        }
    }

    private void OnDestroy()
    {
        if (attachedAction)
        {
            AudioManager.instance.OnBeat -= OnBeat;
        }
    }

    public void OnBeat(int _beatNumber)
    {
        numBeatsSinceActivation++;

        if (!hasFirstActivation)
        {
            if (numBeatsSinceActivation >= initialWaitBeats)
            {
                TriggerObjects();
                numBeatsSinceActivation = 0;
                hasFirstActivation = true;
            }
        }
        else
        {
            if (numBeatsSinceActivation % beatsBetween == 0)
            {
                TriggerObjects();
                numBeatsSinceActivation = 0;
            }
        }
    }

    public void TriggerObjects()
    {
        for (int i = 0; i < affectedObjects.Length; i++)
        {
            if (affectedObjects[i].TryGetComponent(out IToggleable _toggleable))
            {
                _toggleable.OnToggle();
            }
        }
    }
}
