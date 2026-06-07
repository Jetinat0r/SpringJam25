using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public AudioManager.World world;
    public AudioManager.Environment environment = AudioManager.Environment.LEVEL;

    void Start()
    {
        FindFirstObjectByType<AudioManager>().ChangeBGM(world, environment);
    }
}
