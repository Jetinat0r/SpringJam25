using UnityEngine;

public class MinaAudioHelper : MonoBehaviour
{
    [SerializeField] private MusicClip world2Music;
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

    public void PlayHitWhipSound()
    {
        soundPlayer.PlaySound("EasterEgg.HitWhip");
    }

    public void PlayHitWhipFullSound()
    {
        soundPlayer.PlaySound("EasterEgg.HitWhipFull");
    }

    public void PlayHitBatBustSound()
    {
        soundPlayer.PlaySound("EasterEgg.HitBatBust");
    }

    public void PlayRoombaDeathSound()
    {
        soundPlayer.PlaySound("EasterEgg.RoombaDeath");
    }

    public void PlayEasterEggMusic()
    {
        AudioManager.instance.ChangeBGM(world2Music, AudioManager.World.WORLD2, AudioManager.Environment.EASTEREGG);
    }

    public void FadeOutMusic()
    {
        AudioManager.instance.FadeOutCurrent(1f);
        Invoke(nameof(ResetAudioPlayer), 1.1f);
    }

    void ResetAudioPlayer()
    {
        AudioManager.instance.ResetPlayer();
    }

    public void PlayLevelMusic()
    {
        AudioManager.instance.ChangeBGM(world2Music, AudioManager.World.WORLD2, AudioManager.Environment.LEVEL);
    }
}
