using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using JetEngine;
using Steamworks;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public LevelMenuManager levelMenuManager;

    public string currentLevelName = "Level";
    public int currentLevelNumber = 1;

    public string nextLevelName = "Level";
    public Tilemap conveyorTilemap;

    private PlayerMovement player;
    // private SpeedrunManager speedrunManager;

    public Vector2Int zoneSize = new Vector2Int(10, 9);
    private CameraTarget cameraTarget;

    [SerializeField]
    public LevelGrid levelGrid;
    //Which tile on the grid is world (0, 0)
    //  TL Corner is grid (0, 0)
    private Vector2Int zeroRoomReferenceCell = new Vector2Int(0, 0);

    public static bool isResetting = false;

    public float ectoplasmTime = 5f;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        if (levelGrid == null)
        {
            levelGrid = GetComponent<LevelGrid>();
        }

        FindZeroRoomReferenceCell();
    }

    private void Start()
    {
        List<int> _path = new List<int>();
        //Debug.Log(VentSearch(new Vector2Int(0, 1), new Vector2Int(2, 2), ref _path));
        // Debug.Log(VentSearch(new Vector2Int(1, 1), new Vector2Int(2, 0), ref _path));
        foreach (int i in _path)
        {
            Debug.Log($"Path: {i}");
        }
        SettingsManager.currentLevel = currentLevelNumber;
        SettingsManager.SaveSettings();

        // speedrunManager = FindFirstObjectByType<SpeedrunManager>();
    }

    private void FindZeroRoomReferenceCell()
    {
        for (int i = 0; i < levelGrid.width; i++)
        {
            for (int j = 0; j < levelGrid.height; j++)
            {
                if (levelGrid.GetCell(i, j) == "*")
                {
                    zeroRoomReferenceCell = new Vector2Int(i, j);
                    return;
                }
            }
        }
    }

    //Leaves room to support multiple player instances
    public void RegisterPlayer(PlayerMovement _player)
    {
        player = _player;

        //Spaghetti City
        /*
        GameObject _camTarget = new GameObject("Camera Target");
        cameraTarget = _camTarget.AddComponent<CameraTarget>();
        cameraTarget.Init(player.transform);
        Camera.main.GetComponent<CameraSnapToPlayerZone>().Init(cameraTarget.transform);
        */

        Camera.main.GetComponent<CameraSnapToPlayerZone>().Init(player.cameraTarget);
        GameObject _cameraTargetObject = new GameObject("CameraTarget");
        CameraTarget _camTarget = _cameraTargetObject.AddComponent<CameraTarget>();
        _camTarget.Init(player.cameraTarget);
    }

    public void ToggleMenu()
    {
        levelMenuManager.ToggleMenu(player);
    }

    public void ResetSceneIn(float delay)
    {
        //Block perfectly timed double resets
        //  I believe it's impossible to invoke anyways, but more guards is better
        if (isResetting) return;
        Invoke(nameof(ResetScene), delay);
    }

    public void ResetScene()
    {
        if (isResetting) return;
        if (!ScreenWipe.current.WipeIn()) return;
        ScreenWipe.current.PostWipe += () => { SceneManager.LoadScene(currentLevelName); isResetting = false; };
        isResetting = true;
    }

    public void CompleteLevel()
    {
        if (SettingsManager.completedLevels < currentLevelNumber)
        {
            SettingsManager.completedLevels = currentLevelNumber;
            SettingsManager.SaveSettings();
        }

        if (AudioManager.instance.CheckChangeWorlds(nextLevelName))
        {
            if (SteamManager.Initialized)
            {
                float clearTime = 1;
                float timeLimit = 0;
                if (SpeedrunManager.instance != null)
                {
                    clearTime = SpeedrunManager.instance.StopTimer();
                    timeLimit = SpeedrunManager.instance.timeLimit;
                    Debug.Log("Clear time: " + clearTime);
                    Destroy(SpeedrunManager.instance);
                }

                // Unlock appropriate world clear achievements
                // TODO: Add the speedrun achievements as we finish building out entire worlds
                int.TryParse(currentLevelName["Level".Length..], out int level);
                switch (AudioManager.instance.GetWorld(level))
                {
                    case AudioManager.World.WORLD1:
                        Debug.Log("Achievement unlocked! CLEAR_W1");
                        JetEngine.SteamUtils.TryGetAchievement("CLEAR_W1");

                        if (clearTime <= timeLimit)
                        {
                            Debug.Log("Achievement unlocked! SPEEDRUN_W1");
                            JetEngine.SteamUtils.TryGetAchievement("SPEEDRUN_W1");
                        }
                        break;
                    case AudioManager.World.WORLD2:
                        Debug.Log("Achievement unlocked! CLEAR_W2");
                        JetEngine.SteamUtils.TryGetAchievement("CLEAR_W2");

                        if (clearTime <= timeLimit)
                        {
                            Debug.Log("Achievement unlocked! SPEEDRUN_W2");
                            JetEngine.SteamUtils.TryGetAchievement("SPEEDRUN_W2");
                        }
                        break;
                    case AudioManager.World.WORLD3:
                        Debug.Log("Achievement unlocked! CLEAR_W3");
                        JetEngine.SteamUtils.TryGetAchievement("CLEAR_W3");

                        if (clearTime <= timeLimit)
                        {
                            Debug.Log("Achievement unlocked! SPEEDRUN_W3");
                            JetEngine.SteamUtils.TryGetAchievement("SPEEDRUN_W3");
                        }
                        break;
                    case AudioManager.World.WORLD4:
                        Debug.Log("Achievement unlocked! CLEAR_W4");
                        JetEngine.SteamUtils.TryGetAchievement("CLEAR_W4");

                        if (clearTime <= timeLimit)
                        {
                            Debug.Log("Achievement unlocked! SPEEDRUN_W4");
                            JetEngine.SteamUtils.TryGetAchievement("SPEEDRUN_W4");
                        }
                        break;
                }
            }
        }
        
        StartCoroutine(GoToNextLevel(1.5f));
    }

    IEnumerator GoToNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        ScreenWipe.current.WipeIn(true);
        ScreenWipe.current.PostWipe += NextScene;
    }

    public void NextScene()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public bool ReturnToMainMenu()
    {
        if (!ScreenWipe.current.WipeIn()) return false;
        ScreenWipe.current.PostWipe += () => { SceneManager.LoadScene("MainMenu"); };
        return true;
    }

    private enum CELL_SEARCH_VALUES
    {
        CLOSED = -10,
        UNKNOWN = -5,
        //READY = 0,
        START = 0,
        LEFT = -1,
        RIGHT = 1,
        UP = 2,
        DOWN = -2,
        GOAL = 10
    }

    private bool IsValidCell(Vector2Int _targetCell, CELL_SEARCH_VALUES[,] _cells)
    {
        if (_targetCell.x < 0 || _targetCell.y < 0 || _targetCell.x >= levelGrid.width || _targetCell.y >= levelGrid.height)
        {
            return false;
        }

        if(_cells[_targetCell.x, _targetCell.y] != CELL_SEARCH_VALUES.UNKNOWN)
        {
            return false;
        }

        return true;
    }

    private bool VentSearch(Vector2Int _startCell, Vector2Int _endCell, ref List<int> _cellPath)
    {
        CELL_SEARCH_VALUES[,] _cells = new CELL_SEARCH_VALUES[levelGrid.width, levelGrid.height];
        for (int i = 0; i < levelGrid.width; i++)
        {
            for (int j = 0; j < levelGrid.height; j++)
            {
                if (levelGrid.GetCell(i, j) == "F")
                {
                    _cells[i, j] = CELL_SEARCH_VALUES.CLOSED;
                }
                else
                {
                    _cells[i, j] = CELL_SEARCH_VALUES.UNKNOWN;
                }
            }
        }

        _cells[_startCell.x, _startCell.y] = CELL_SEARCH_VALUES.START;
        Queue<Vector2Int> _cellsToCheck = new Queue<Vector2Int>();
        _cellsToCheck.Enqueue(_startCell);

        //Debug.Log($"V: {IsValidCell(_endCell, _cells)}");
        bool _reachedEndCell = false;
        while (_cellsToCheck.Count > 0)
        {
            Vector2Int _curCell = _cellsToCheck.Dequeue();
            //Debug.Log($"Checking: {_curCell}");
            if (_curCell == _endCell)
            {
                _reachedEndCell = true;
                break;
            }

            Vector2Int _rightCell = _curCell + new Vector2Int(1, 0);
            Vector2Int _leftCell = _curCell + new Vector2Int(-1, 0);
            Vector2Int _upCell = _curCell + new Vector2Int(0, -1);
            Vector2Int _downCell = _curCell + new Vector2Int(0, 1);

            if (IsValidCell(_rightCell, _cells))
            {
                _cells[_rightCell.x, _rightCell.y] = CELL_SEARCH_VALUES.LEFT;
                _cellsToCheck.Enqueue(_rightCell);
            }
            if (IsValidCell(_downCell, _cells))
            {
                _cells[_downCell.x, _downCell.y] = CELL_SEARCH_VALUES.UP;
                _cellsToCheck.Enqueue(_downCell);
            }
            if (IsValidCell(_leftCell, _cells))
            {
                _cells[_leftCell.x, _leftCell.y] = CELL_SEARCH_VALUES.RIGHT;
                _cellsToCheck.Enqueue(_leftCell);
            }
            if (IsValidCell(_upCell, _cells))
            {
                _cells[_upCell.x, _upCell.y] = CELL_SEARCH_VALUES.DOWN;
                _cellsToCheck.Enqueue(_upCell);
            }
            //Debug.Log($"DOWN: {_downCell} {IsValidCell(_downCell, _cells)}");
        }

        if (_reachedEndCell)
        {
            Vector2Int _curCell = _endCell;
            while (_curCell != _startCell)
            {
                int _dir = 0;
                Vector2Int _moveDir;
                switch (_cells[_curCell.x, _curCell.y])
                {
                    case CELL_SEARCH_VALUES.LEFT:
                        _dir = 1;
                        _moveDir = new Vector2Int(-1, 0);
                        break;
                    case CELL_SEARCH_VALUES.RIGHT:
                        _dir = -1;
                        _moveDir = new Vector2Int(1, 0);
                        break;
                    case CELL_SEARCH_VALUES.UP:
                        _dir = -2;
                        _moveDir = new Vector2Int(0, -1);
                        break;
                    case CELL_SEARCH_VALUES.DOWN:
                        _dir = 2;
                        _moveDir = new Vector2Int(0, 1);
                        break;
                    default:
                        Debug.LogError("How did this break???");
                        return false;
                }

                _cellPath.Add(_dir);
                _curCell += _moveDir;
            }

            _cellPath.Reverse();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetVentPath(Vent _vent, out List<int> _zonePath)
    {
        _zonePath = new List<int>();
        Vector2Int _startZone = new Vector2(MathUtils.GetClosestEvenDivisor(_vent.transform.position.x, LevelManager.instance.zoneSize.x) / LevelManager.instance.zoneSize.x, MathUtils.GetClosestEvenDivisor(_vent.transform.position.y, LevelManager.instance.zoneSize.y) / LevelManager.instance.zoneSize.y).ToVector2Int();
        Vector2Int _endZone = new Vector2(MathUtils.GetClosestEvenDivisor(_vent.counterpart.transform.position.x, LevelManager.instance.zoneSize.x) / LevelManager.instance.zoneSize.x, MathUtils.GetClosestEvenDivisor(_vent.counterpart.transform.position.y, LevelManager.instance.zoneSize.y) / LevelManager.instance.zoneSize.y).ToVector2Int();

        //Cell space is upside down from world space
        _startZone.y *= -1;
        _endZone.y *= -1;

        //Quick exit if vent is on same screen
        if (_startZone == _endZone)
        {
            return true;
        }

        Vector2Int _startCell = zeroRoomReferenceCell + _startZone;
        Vector2Int _endCell = zeroRoomReferenceCell + _endZone;

        
        if (_startCell.x < 0 || _startCell.y < 0 || _endCell.x < 0 || _endCell.y < 0 ||
            _startCell.x >= levelGrid.width || _startCell.y >= levelGrid.height || _endCell.x >= levelGrid.width || _endCell.y >= levelGrid.height)
        {
            Debug.LogError($"LEVEL GRID IS MESSED UP; FIX IMMEDIATELY; OUT OF BOUNDS: {_startCell} || {_endCell}");
            return false;
        }

        return VentSearch(_startCell, _endCell, ref _zonePath);
    }
}
