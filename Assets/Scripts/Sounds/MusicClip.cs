using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Clip", menuName = "AudioAssets/Music Clip")]
public class MusicClip : SoundPlayable
{
    public AudioClip menuSong, levelSong, easterEggSong;
    public AudioManager.World world;
    public float BPM;
    public int sampleRate = 44100;
    public int beatFrequency = 1;
    public AudioClip GetClip(AudioManager.Environment environment)
    {
        switch (environment)
        {
            case AudioManager.Environment.MENU:
                return menuSong;
            case AudioManager.Environment.LEVEL:
                return levelSong;
            case AudioManager.Environment.EASTEREGG:
                return easterEggSong;
        }
        return levelSong; // default fallback
    }

    public override AudioClip GetClip()
    {
        return GetClip(AudioManager.Environment.LEVEL);
    }

}
