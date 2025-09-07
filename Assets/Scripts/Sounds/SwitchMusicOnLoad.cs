using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public AudioManager.World world;
    public bool fromMenu = false;

    void Start()
    {
        FindFirstObjectByType<AudioManager>().ChangeBGM(world, fromMenu);
    }
}
