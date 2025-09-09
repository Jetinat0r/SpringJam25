using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New World Variable Sound Clip", menuName = "AudioAssets/World Variable Sound Clip")]
public class WorldVariableSoundClip : SoundPlayable
{
    public AudioClip[] clip = new AudioClip[System.Enum.GetNames(typeof(AudioManager.World)).Length-1];
    public override AudioClip GetClip()
    {
        return clip[(int)AudioManager.instance.currentWorld-1];
    }
    public float length()
    {
        return GetClip().length;
    }
}
