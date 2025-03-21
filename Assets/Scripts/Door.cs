using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public int levelNumber = 1;
    public string nextLevel = "Level";

    public bool requiresKey = true;
    private PlayerMovement playerScript = null;

    public void MyInteraction()
    {
        // TODO: Talk with some scene manager or initiate some UI thing to initiate level complete
        if(!requiresKey || playerScript.hasKey)
        {
            Debug.Log("Level Complete!");

            if(SettingsManager.completedLevels < levelNumber)
            {
                SettingsManager.completedLevels = levelNumber;
                SettingsManager.SaveSettings();
            }

            //TODO: Swap level
            SceneManager.LoadScene(nextLevel);

            playerScript.OnInteract -= MyInteraction;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Spawn interaction prompt over player head
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Entered");

            // This is where you'd summon the interact button prompt if we want to make it appear over their head regardless of whether or not they have a key
            PlayerMovement _playerScript = collision.gameObject.GetComponent<PlayerMovement>();

            if (_playerScript != null)
            {
                _playerScript.OnInteract += MyInteraction;
                playerScript = _playerScript;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exited");

            collision.gameObject.GetComponent<PlayerMovement>().OnInteract -= MyInteraction;
            playerScript = null;
        }
    }
}
