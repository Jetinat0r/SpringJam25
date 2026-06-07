using UnityEngine;

public class MinaAudioHelper : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;

    public void PlayLandingSound()
    {
        soundPlayer.PlaySound("EasterEgg.Landing");
    }

    public void PlayFallSound()
    {
        soundPlayer.PlaySound("EasterEgg.Fall");
    }

    public void PlayJumpSound()
    {
        soundPlayer.PlaySound("EasterEgg.Jump");
    }

    public void PlayWhipSound()
    {
        soundPlayer.PlaySound("EasterEgg.Whip");
    }

    public void PlayTypewriterSound()
    {
        soundPlayer.PlaySound("EasterEgg.Typewriter");
    }

    public void PlayItemGetSound()
    {
        soundPlayer.PlaySound("EasterEgg.ItemGet");
    }

    public void PlayItemGetShortSound()
    {
        soundPlayer.PlaySound("EasterEgg.ItemGetShort");
    }
}
