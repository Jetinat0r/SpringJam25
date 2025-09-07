using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer musicMixer, sfxMixer, globalSfxMixer;
    public AudioMixerGroup musicMixerGroup;
    public MusicClip currentSong = null;
    public World currentWorld;

    private int activePlayer = 0;
    public AudioSource[] BGM1, BGM2;
    private IEnumerator[] fader = new IEnumerator[2];
    public float musicVolume = 1.0f, sfxVolume = 10.0f, targetSFXVolume= -80.0f, actualSFXVolume = -80.0f;

    //Note: If the volumeChangesPerSecond value is higher than the fps, the duration of the fading will be extended!
    private int volumeChangesPerSecond = 15;

    public float fadeDuration = 1.0f;
    private bool firstSet = true;
    private bool firstSongPlayed = false;
    public bool paused = false;

    public AudioMixerSnapshot normal, hurt;
    public SoundCategory soundDatabase;
    public MusicCategory musicDatabase;
    public bool playingMenuMusic = false;

    /// <summary>
    /// List of all different game areas that may have different sets of music
    /// </summary>
    public enum World
    {
        CURRENT, WORLD1, WORLD2, WORLD3, WORLD4
    }

    /// <summary>
    /// Mutes all AudioSources, but does not stop them!
    /// </summary>
    public bool musicMute
    {
        set
        {
            foreach (AudioSource s in BGM1)
            {
                s.mute = value;
            }
            foreach (AudioSource s in BGM2)
            {
                s.mute = value;
            }
        }
        get
        {
            if (firstSet)
                return BGM1[activePlayer].mute;
            return BGM2[activePlayer].mute;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWorld = GetWorld(SettingsManager.currentLevel);
    }

    public World GetWorld(int level)
    {
        return level switch
        {
            >= 0 and <= 8 => World.WORLD1,
            > 8 and <= 16 => World.WORLD2,
            > 16 and <= 24 => World.WORLD3,
            > 24 and <= 32 => World.WORLD4,
            _ => World.WORLD1,
        };
    }

    public void CheckChangeWorlds(string _levelName)
    {
        int.TryParse(_levelName["Level".Length..], out int level);
        if (currentWorld != GetWorld(level))
        {
            Stop();
        }
    }

    /// <summary>
    /// Setup the AudioSources
    /// </summary>
    private void Awake()
    {
        //Generate two AudioSource lists
        BGM1 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        BGM2 = new AudioSource[2]{
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };

        //Set default values
        foreach (AudioSource s in BGM1)
        {
            s.loop = true;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = musicMixerGroup;
        }

        foreach (AudioSource s in BGM2)
        {
            s.loop = true;
            s.playOnAwake = false;
            s.volume = 0.0f;
            s.outputAudioMixerGroup = musicMixerGroup;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        musicVolume = SettingsManager.musicVolume;
        sfxVolume = SettingsManager.sfxVolume;
        /*
        if (SettingsManager.currentSettings != null)
        {
            
            //musicMute = SettingsManager.currentSettings.musicMute;
        }
        else
        {
            musicMixer.GetFloat("Volume", out musicVolume);
        }
        */

        if (FindObjectsByType<AudioManager>(FindObjectsSortMode.None).Length > 1)
        {
            instance = null;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO add states for these
        //if (GameManager.instance.paused || GameManager.instance.gameOver) return;

        if (instance == null)
        {
            instance = this;
        }

        // Check for desyncing during crossfades
        if ((fader[0] != null || fader[1] != null) && BGM2[activePlayer].isPlaying && BGM1[activePlayer].isPlaying)
        {
            if (firstSet && BGM2[activePlayer].timeSamples != BGM1[activePlayer].timeSamples)
            {
                BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples;
            }
            else if (!firstSet && BGM1[activePlayer].timeSamples != BGM2[activePlayer].timeSamples)
            {
                BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples;
            }
        }

        // Volume controls (hold down + or -)
        musicMixer.SetFloat("Volume", musicVolume);
        musicMixer.GetFloat("Volume", out musicVolume);

        if (Input.GetKey("="))
        {
            if (musicVolume < 0f)
                musicVolume += 0.1f;
        }

        if (Input.GetKey("-"))
        {
            if (musicVolume > -80f)
                musicVolume -= 0.1f;
        }
        SettingsManager.musicVolume = musicVolume;
        SettingsManager.sfxVolume = sfxVolume;

        // sfxVolume is a float from 0.0-1.0 but we'd want 1.0 to correspond to 10dB => *10f
        targetSFXVolume = SettingsManager.sfxVolume;
        // TODO in case we want fade between scene changes, add this sorta thing
        //if (ChangeScene.changingScene)
        //{
        //    actualSFXVolume = Mathf.Lerp(actualSFXVolume, -80, 0.1f);
        //}
        //else
        //{
        //    actualSFXVolume = Mathf.Lerp(actualSFXVolume, targetSFXVolume, 0.1f);
        //}
        actualSFXVolume = targetSFXVolume; // TEMP
        sfxMixer.SetFloat("Volume", actualSFXVolume);
    }

    public void ChangeBGM(World newWorld, bool fromMenu, float duration = 1f)
    {
        if (newWorld == World.CURRENT) newWorld = currentWorld;
        ChangeBGM((MusicClip)musicDatabase.children[(int)newWorld - 1], fromMenu, duration);
    }

    public void ChangeBGM(MusicClip music, bool fromMenu, float duration = 1f)
    {
        ChangeBGM(music, music.world, fromMenu, duration);
    }

    public void ChangeBGM(MusicClip music, World newWorld, bool fromMenu, float duration = 1f)
    {
        if (newWorld == World.CURRENT) newWorld = currentWorld;

        // carry on music if world has not changed
        bool carryOn = currentWorld == newWorld;
        currentWorld = newWorld;

        //Prevent fading the same clip on both players
        if (firstSongPlayed && carryOn && playingMenuMusic == fromMenu)
            return;
        playingMenuMusic = fromMenu;

        //Kill all playing
        foreach (IEnumerator i in fader)
        {
            if (i != null)
            {
                StopCoroutine(i);
            }
        }

        if (currentSong == null) duration = 0f; // No fades for world transitions

        if (firstSet)
        {
            //Fade-out the active play, if it is not silent (eg: first start)
            if (BGM1[activePlayer].volume > 0)
            {
                fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM1[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM2[activePlayer].clip = music.GetClip();
            if (carryOn && BGM1[activePlayer].isPlaying)
            {
                BGM2[activePlayer].timeSamples = BGM1[activePlayer].timeSamples; // syncs up time
            }
            else
            {
                BGM2[activePlayer].timeSamples = 0;
            }
            BGM2[activePlayer].Play();
            if (firstSongPlayed)
            {
                fader[1] = FadeAudioSource(BGM2[activePlayer], duration, 1.0f, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM2[activePlayer].volume = 1.0f;
            }
        }
        else
        {
            //Fade-out the active play, if it is not silent (eg: first start)
            if (BGM2[activePlayer].volume > 0)
            {
                fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 0.0f, () => { fader[0] = null; });
                StartCoroutine(fader[0]);
            }
            BGM2[1 - activePlayer].Stop();

            //Fade-in the new clip
            BGM1[activePlayer].clip = music.GetClip();
            if (carryOn && BGM2[activePlayer].isPlaying)
            {
                BGM1[activePlayer].timeSamples = BGM2[activePlayer].timeSamples; // syncs up time
            }
            else
            {
                BGM1[activePlayer].timeSamples = 0;
            }
            BGM1[activePlayer].Play();
            if (firstSongPlayed)
            {
                fader[1] = FadeAudioSource(BGM1[activePlayer], duration, 1.0f, () => { fader[1] = null; });
                StartCoroutine(fader[1]);
            }
            else
            {
                BGM1[activePlayer].volume = 1.0f;
            }
        }

        firstSet = !firstSet;
        firstSongPlayed = true;

        //Set new clip to current song
        currentSong = music;
    }

    /// <summary>
    /// Fades an AudioSource(player) during a given amount of time(duration) to a specific volume(targetVolume)
    /// </summary>
    /// <param name="player">AudioSource to be modified</param>
    /// <param name="duration">Duration of the fading</param>
    /// <param name="targetVolume">Target volume, the player is faded to</param>
    /// <param name="finishedCallback">Called when finshed</param>
    /// <returns></returns>
    IEnumerator FadeAudioSource(AudioSource player, float duration, float targetVolume, System.Action finishedCallback)
    {
        //Calculate the steps
        int Steps = (int)(volumeChangesPerSecond * duration);
        float StepTime = duration / Steps;
        float StepSize = (targetVolume - player.volume) / Steps;

        //Fade now
        for (int i = 1; i < Steps; i++)
        {
            player.volume += StepSize;
            yield return new WaitForSeconds(StepTime);
        }
        //Make sure the targetVolume is set
        player.volume = targetVolume;

        //Callback
        if (finishedCallback != null)
        {
            finishedCallback();
        }
    }

    public void SetPitch(float pitch)
    {
        musicMixer.SetFloat("Pitch", pitch);
    }

    public IEnumerator PitchDown()
    {
        for (float i = 0; i <= 1; i += 0.05f)
        {
            SetPitch(Mathf.Lerp(1f, 0f, i));
            yield return new WaitForSeconds(0.04f);
        }
    }

    public void FadeOutCurrent(float duration = 1f)
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 0.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
    }

    public void FadeInCurrent(float duration = 1f)
    {
        if (firstSet)
        {
            fader[0] = FadeAudioSource(BGM1[activePlayer], duration, 1.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
        else
        {
            fader[0] = FadeAudioSource(BGM2[activePlayer], duration, 1.0f, () => { fader[0] = null; });
            StartCoroutine(fader[0]);
        }
    }

    public void PauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].Pause();
            if (fader[0] != null)
            {
                BGM2[activePlayer].Pause();
            }
        }
        else
        {
            BGM2[activePlayer].Pause();
            if (fader[0] != null)
            {
                BGM1[activePlayer].Pause();
            }
        }
        paused = true;
    }

    public void UnPauseCurrent()
    {
        if (firstSet)
        {
            BGM1[activePlayer].UnPause();
            if (fader[0] != null)
            {
                BGM2[activePlayer].UnPause();
            }
        }
        else
        {
            BGM2[activePlayer].UnPause();
            if (fader[0] != null)
            {
                BGM1[activePlayer].UnPause();
            }
        }
        paused = false;
    }

    public void Stop()
    {
        foreach (AudioSource source in BGM1)
        {
            source.Stop();
            source.clip = null;
        }
        foreach (AudioSource source in BGM2)
        {
            source.Stop();
            source.clip = null;
        }
        currentSong = null;
        paused = false;
    }

    public AudioClip FindSound(string soundPath)
    {
        List<string> path = new List<string>(soundPath.Trim().Split("."));
        return FindSound(soundDatabase, path);
    }

    public AudioClip FindSound(SoundNode current, List<string> path)
    {
        if (current is SoundPlayable)
        {
            return ((SoundPlayable)current).GetClip();
        }
        else if (current is SoundCategory)
        {
            foreach (SoundNode node in ((SoundCategory)current).children)
            {
                if (path.Count > 0 && node.name.ToLower() == path[0].ToLower())
                {
                    // Debug.Log("Found " + path[0]);
                    current = node;
                    path.RemoveAt(0);
                    return FindSound(node, path);
                }
            }
            Debug.LogError("Invalid sound path provided!");
            return null;
        }
        Debug.LogError("Invalid sound path provided!");
        return null;
    }
}