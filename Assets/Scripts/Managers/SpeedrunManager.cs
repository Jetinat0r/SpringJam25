using UnityEngine;

public class SpeedrunManager : MonoBehaviour
{
    public static SpeedrunManager instance;
    public int timeLimit;
    private float timer;
    private bool timerActive = true;


    private void Awake()
    {
        // Singleton class
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        // This class is only useful for checking the time trial achievements in Steam. If Steam has not yet been initialized, destroy self
        //JET: Our game should be resilient enough to be fine running this stuff even without Steam.
        /*
        if (!SteamManager.Initialized)
        {
            Destroy(instance.gameObject);
        }
        */
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (timerActive && !LevelMenuManager.isMenuOpen)
        {
            timer += Time.unscaledDeltaTime;
            // Debug.Log("Timer at " + timer);
        }
    }

    public void ContinueTimer()
    {
        timerActive = true;
    }

    public void PauseTimer()
    {
        timerActive = false;
    }

    public float StopTimer()
    {
        float endingTime = timer;
        timerActive = false;
        timer = 0;

        return endingTime;
    }

    public void StartTimer(int minutes, int seconds)
    {
        timeLimit = minutes * 60 + seconds;
        timer = 0;
        timerActive = true;
    }
}
