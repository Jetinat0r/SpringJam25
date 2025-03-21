using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMenuManager : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject firstSelectedElement;

    bool isMenuOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void ToggleMenu()
    {
        if (!isMenuOpen)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);

        eventSystem.SetSelectedGameObject(firstSelectedElement);

        isMenuOpen = true;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);

        eventSystem.SetSelectedGameObject(null);

        isMenuOpen = false;
    }

    public void RestartLevel()
    {
        LevelManager.instance.ResetScene();

        CloseMenu();
    }

    public void ExitToMainMenu()
    {
        LevelManager.instance.ReturnToMainMenu();
    }
}
