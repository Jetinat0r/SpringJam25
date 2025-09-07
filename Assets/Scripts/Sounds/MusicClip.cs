using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Music Clip", menuName = "AudioAssets/Music Clip")]
public class MusicClip : SoundPlayable
{
    public AudioClip menuSong, levelSong;
    public AudioManager.World world;
    public override AudioClip GetClip()
    {
        return MainMenuManager.inMenu ? menuSong : levelSong;
    }
}
