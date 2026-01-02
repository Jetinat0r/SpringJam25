using UnityEngine;
/*
 * THIS SCRIPT IS USED FOR CHALLENGES WITH MULTIPLE COMPONENTS, LIKE MALFUNCTION, TO ACT AS A SINGULAR OBJECT TO ANNOUNCE THE ACTIVE CHALLENGE
 * CHALLENGES LIKE FLIP SIDE (Script: CamFlip.cs) ARE SINGULAR OBJECTS THAT ANNOUNCE THEMSELVES, AND THUS DO NOT NEED THIS SCRIPT
 */
public class SpectralShuffleChallengeAnnouncer : MonoBehaviour
{
    [SerializeField]
    public LevelMenuManager levelMenuManager;

    [SerializeField]
    public string challengeName = "CUSTOM CHALLENGE";

    void Start()
    {
        if (!ChallengeManager.instance.spectralShuffleEnabled)
        {
            Destroy(gameObject);
            return;
        }

        if (levelMenuManager == null)
        {
            Debug.LogWarning("Level Menu Manager not assigned, searching scene...");
            levelMenuManager = FindFirstObjectByType<LevelMenuManager>();
        }

        levelMenuManager.DisplaySpectralShuffleChallenge(challengeName);
    }
}
