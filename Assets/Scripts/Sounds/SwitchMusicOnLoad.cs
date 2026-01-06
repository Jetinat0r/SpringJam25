using UnityEngine;

public class SwitchMusicOnLoad : MonoBehaviour
{
    public AudioManager.World world;
    public bool fromMenu = false;
    public bool spoofMenu = false;
    
    void Start()
    {
        if (spoofMenu) MainMenuManager.inMenu = true;
        FindFirstObjectByType<AudioManager>().ChangeBGM(world, fromMenu);
    }
}
