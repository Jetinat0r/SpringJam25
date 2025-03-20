using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerCheck : MonoBehaviour
{
    public GameObject audioManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!FindFirstObjectByType<AudioManager>())
        {
            Instantiate(audioManager, transform.position, transform.rotation);
        }
    }
}
