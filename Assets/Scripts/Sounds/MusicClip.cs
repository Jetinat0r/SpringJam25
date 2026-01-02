using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Clip", menuName = "AudioAssets/Music Clip")]
public class MusicClip : SoundPlayable
{
    public AudioClip menuSong, levelSong;
    public AudioManager.World world;
    public float BPM;
    public int sampleRate = 44100;
    public int beatFrequency = 1;
    public override AudioClip GetClip()
    {
        return MainMenuManager.inMenu ? menuSong : levelSong;
    }
}
