using DG.Tweening;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager instance;
    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void InitDOTween()
    {
        DOTween.Init(true, true, LogBehaviour.Default);
        //Debug.Log("DOTween Initialized");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AddGlobalManagers()
    {
        GameObject _managerGameObject = new GameObject();

        _managerGameObject.name = "GLOBAL_MANAGERS";
        DontDestroyOnLoad(_managerGameObject);

        _managerGameObject.AddComponent<ProgramManager>();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //LoadSettings();
    }

    private void LoadSettings()
    {
        
    }

    public void SaveSettings()
    {
        
    }
}
