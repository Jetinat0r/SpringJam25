using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicTrigger : MonoBehaviour
{
    public MusicClip newTrack;
    private MusicClip oldTrack;
    private AudioManager theAM;
    private AudioManager.Environment oldEnvironment;
    [SerializeField] private AudioManager.Environment environment;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && newTrack != null)
        {
            theAM = FindFirstObjectByType<AudioManager>();
            oldTrack = theAM.currentSong;
            oldEnvironment = theAM.currentEnvironment;

            // hacked in but don't mind it :)
            if (environment == AudioManager.Environment.EASTEREGG)
                MinaAudioHelper.InEasterEgg = true;

            theAM.ChangeBGM(newTrack, theAM.currentWorld, environment);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && oldTrack != null)
        {
            theAM = FindFirstObjectByType<AudioManager>();

            // hacked in but don't mind it :)
            if (oldEnvironment == AudioManager.Environment.EASTEREGG)
                MinaAudioHelper.InEasterEgg = true;

            theAM.ChangeBGM(oldTrack, theAM.currentWorld, oldEnvironment);
        }
    }
}
