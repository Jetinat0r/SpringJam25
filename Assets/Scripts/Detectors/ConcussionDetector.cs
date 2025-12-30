using UnityEngine;

public class ConcussionDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("You got: Concussed!");
            JetEngine.SteamUtils.TryGetAchievement("CONCUSSION");
            ProgramManager.instance.saveData.Concussed = true;
            ProgramManager.instance.saveData.SaveSaveData();
        }
    }
}
