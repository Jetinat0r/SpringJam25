using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public MusicClip newTrack;
    public AudioManager.GameArea newArea;
    private AudioManager theAM;
    public float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (newTrack != null)
        {
            theAM = FindFirstObjectByType<AudioManager>();
            theAM.ChangeBGM(newTrack, newArea);
            if (delay > 0)
            {
                theAM.PauseCurrent();
                StartCoroutine(PlayMusicDelayed());
            }
            else
            {
                theAM = FindFirstObjectByType<AudioManager>();
                theAM.ChangeBGM(newTrack, newArea);
            }
        }
    }

    IEnumerator PlayMusicDelayed()
    {
        yield return new WaitForSeconds(delay);
        AudioManager.instance.UnPauseCurrent();
    }
}
