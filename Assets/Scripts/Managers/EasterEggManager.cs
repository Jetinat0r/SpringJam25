using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class EasterEggManager : SignalReceiver
{
    public static EasterEggManager instance = null;

    [SerializeField]
    DirectorControlPlayable easterEggTimeline;

    #region Open Secret Wall
    [SerializeField]
    EasterEggTrigger secretWallTrigger;
    [SerializeField]
    GameObject darkSecretWall;
    [SerializeField]
    GameObject lightSecretWall;

    [SerializeField]
    ParticleSystem wallExplosionParticles;

    bool openedSecretRoom = false;
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
    GameObject societalConvention;

    #region Persistant Cutscene Intro Skip
    bool completedIntroCutscene = false;
    #endregion

    #region Persistant Completed Secret
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

            //TODO: Set timeline to past intro sequence (and play looping waiting mina)
            societalConvention.SetActive(false);
            startCutsceneTrigger.onTriggerEnter += StartCutscenePlayback;
        }
        else if (enteredSecretRoom)
        {
            //Open the secret wall without particles
            OpenSecretWallSilent();

            //Connect to proper signals
            enteredSecretRoomTrigger.onTriggerEnter += OnSecretRoomEntered;
            exitedSecretRoomTrigger.onTriggerEnter += OnSecretRoomExited;
            startCutsceneTrigger.onTriggerEnter += StartCutscenePlayback;
        }
        else
        {
            //Connect to proper signals
            secretWallTrigger.onTriggerEnter += OpenSecretWall;
            enteredSecretRoomTrigger.onTriggerEnter += OnSecretRoomEntered;
            exitedSecretRoomTrigger.onTriggerEnter += OnSecretRoomExited;
        }
    }

    private void OpenSecretWall()
    {
        openedSecretRoom = true;

        darkSecretWall.SetActive(false);
        lightSecretWall.SetActive(false);

        //Play particles
        wallExplosionParticles.Play();
    }

    private void OpenSecretWallSilent()
    {
        openedSecretRoom = true;

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
        easterEggTimeline.director.Play();
    }

    public void StartDialogue()
    {
        //TODO: Use some magic to lower the box (DOTween?)
        PlayNextDialogue();
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
        minaDialogues[curDialogue].Play();
    }

    public void EndDialogue()
    {
        //TODO: Use some magic to raise the box (DOTween?)
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
}
