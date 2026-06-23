using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class EasterEggManager : SignalReceiver
{
    public static EasterEggManager instance = null;

    [SerializeField] private SoundPlayer soundPlayer;

    [SerializeField]
    PlayableDirector easterEggTimeline;

    #region Open Secret Wall
    [SerializeField]
    EasterEggTrigger secretWallTrigger;
    [SerializeField]
    GameObject darkSecretWall;
    [SerializeField]
    GameObject lightSecretWall;

    [SerializeField]
    ParticleSystem wallExplosionParticles;

    #endregion

    #region Persistant Open Wall
    [SerializeField]
    EasterEggTrigger enteredSecretRoomTrigger;

    bool enteredSecretRoom = false;
    #endregion

    [SerializeField]
    EasterEggTrigger startCutsceneTrigger;

    [SerializeField]
    RectTransform dialogueBoxTransform;
    [SerializeField]
    TextRevealer[] minaDialogues;
    private int curDialogue = 0;

    [SerializeField]
    float talkingBlinkLoopStartTime = 3f;
    [SerializeField]
    float postTalkWalkAwayStartTime = 5f;
    [SerializeField]
    float waitingBlinkLoopStartTime = 7f;
    [SerializeField]
    float victoryStartTime = 10f;

    [SerializeField]
    GameObject societalConvention;


    #region Persistant Cutscene Intro Skip
    bool completedIntroCutscene = false;
    #endregion

    #region Persistant Completed Secret
    [SerializeField]
    EasterEggTrigger winEasterEggTrigger;

    [SerializeField]
    Switch easterEggCompleteSwitch;

    [SerializeField]
    EasterEggTrigger exitedSecretRoomTrigger;

    bool completedSecretRoom;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance != null)
        {
            //Steal data from the old instance
            enteredSecretRoom = instance.enteredSecretRoom;
            completedIntroCutscene = instance.completedIntroCutscene;
            completedSecretRoom = instance.completedSecretRoom;

            //Remove the old one, as it no longer has references to relevant objects!
            Destroy(instance.gameObject);
        }

        //Become the one and only instance
        instance = this;
        DontDestroyOnLoad(gameObject);

        //Hook into relevant events
        SceneManager.sceneLoaded += OnSceneLoad;

        //The wall should remain closed FOREVER
        if (completedSecretRoom)
        {
            secretWallTrigger.gameObject.SetActive(false);
        }
        else if (completedIntroCutscene)
        {
            //Open the secret wall without particles
            OpenSecretWallSilent();

            //Connect to proper signals
            enteredSecretRoomTrigger.onTriggerEnter += OnSecretRoomEntered;
            exitedSecretRoomTrigger.onTriggerEnter += OnSecretRoomExited;

            societalConvention.SetActive(false);
            //Set timeline to past intro sequence (and play looping waiting mina)
            easterEggTimeline.initialTime = waitingBlinkLoopStartTime;
            startCutsceneTrigger.onTriggerEnter += StartCutscenePlayback;
            winEasterEggTrigger.onTriggerEnter += WinEasterEgg;
        }
        else if (enteredSecretRoom)
        {
            //Open the secret wall without particles
            OpenSecretWallSilent();

            //Connect to proper signals
            enteredSecretRoomTrigger.onTriggerEnter += OnSecretRoomEntered;
            exitedSecretRoomTrigger.onTriggerEnter += OnSecretRoomExited;
            startCutsceneTrigger.onTriggerEnter += StartCutscenePlayback;
            winEasterEggTrigger.onTriggerEnter += WinEasterEgg;
        }
        else
        {
            //Connect to proper signals
            secretWallTrigger.onTriggerEnter += OpenSecretWall;
            enteredSecretRoomTrigger.onTriggerEnter += OnSecretRoomEntered;
            exitedSecretRoomTrigger.onTriggerEnter += OnSecretRoomExited;
            startCutsceneTrigger.onTriggerEnter += StartCutscenePlayback;
            winEasterEggTrigger.onTriggerEnter += WinEasterEgg;
        }
    }

    private void OpenSecretWall()
    {
        darkSecretWall.SetActive(false);
        lightSecretWall.SetActive(false);

        //Play particles
        wallExplosionParticles.Play();

        // Play special sound
        soundPlayer?.PlaySound("EasterEgg.Door");
        
    }

    private void OpenSecretWallSilent()
    {
        darkSecretWall.SetActive(false);
        lightSecretWall.SetActive(false);
    }

    private void OnSecretRoomEntered()
    {
        enteredSecretRoom = true;
        exitedSecretRoomTrigger.gameObject.SetActive(true);
    }

    private void OnSecretRoomExited()
    {
        completedSecretRoom = true;
    }

    private void StartCutscenePlayback()
    {
        easterEggTimeline.Play();
    }

    public void StartDialogue()
    {
        Tween _tween = dialogueBoxTransform.DOAnchorPos(new Vector2(0, -4), 1.2f).SetEase(Ease.OutQuart);
        _tween.onComplete += PlayNextDialogue;
    }

    public void PlayNextDialogue()
    {
        if (curDialogue >= minaDialogues.Length)
        {
            Debug.LogError($"Trying to play dialogue [{curDialogue}] that doesn't exist!");
            return;
        }

        if (curDialogue + 1 < minaDialogues.Length)
        {
            minaDialogues[curDialogue].onTextFullyRevealed += PlayNextDialogue;
        }
        else
        {
            minaDialogues[curDialogue].onTextFullyRevealed += EndDialogue;
        }

        foreach (TextRevealer t in minaDialogues)
        {
            t.gameObject.SetActive(false);
        }

        minaDialogues[curDialogue].gameObject.SetActive(true);
        minaDialogues[curDialogue].Play();

        curDialogue++;
    }

    public void EndDialogue()
    {
        dialogueBoxTransform.DOAnchorPos(new Vector2(0, 60), 1.2f).SetEase(Ease.InQuart);
        societalConvention.SetActive(false);

        completedIntroCutscene = true;
        easterEggTimeline.Pause();
        easterEggTimeline.time = postTalkWalkAwayStartTime;
        easterEggTimeline.Resume();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene _scene, LoadSceneMode _sceneLoadMode)
    {
        if (_scene.name != "Level15")
        {
            Debug.Log("Clearing Easter Egg State");
            instance = null;
            Destroy(gameObject);
        }
    }

    public void MovePlayheadToTalkingLoopPoint()
    {
        easterEggTimeline.time = talkingBlinkLoopStartTime + Random.Range(0f, 0.5f);
    }

    public void MovePlayheadToWaitingLoopPoint()
    {
        easterEggTimeline.time = waitingBlinkLoopStartTime + Random.Range(0f, 0.5f);
    }

    public void WinEasterEgg()
    {
        easterEggTimeline.Pause();
        easterEggTimeline.time = victoryStartTime;
        easterEggTimeline.Resume();
    }

    public void FlipCompletionSwitch()
    {
        easterEggCompleteSwitch.MyInteraction();
    }
}
