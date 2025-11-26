using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace JetEngine
{
    public static class SteamUtils
    {
        //Set via cmd line args
        private static bool noSteam = false;

        //Checks if steam is successfully loaded
        private static bool successfullyLoadedSteamApi = false;

        //Try to load Steam
        //  Returns false if Steam fails to load or if "-noSteam" is defined as a command line arg
        public static bool TryInitSteam()
        {
            noSteam = false;
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.HasKey("noSteam"))
            {
                noSteam = UnityEditor.EditorPrefs.GetBool("noSteam");
                Debug.Log($"EDITOR: Set noSteam to {noSteam}");
            }
#endif

            //If we're specifically circumventing steam, don't even bother opening it!
            string[] _cmdLineArgs = Environment.GetCommandLineArgs();
            if (_cmdLineArgs.Contains("-noSteam"))
            {
                noSteam = true;
                successfullyLoadedSteamApi = false;
                return false;
            }

            if (SteamManager.Initialized)
            {
                successfullyLoadedSteamApi = true;

                return true;
            }
            else
            {
                successfullyLoadedSteamApi = false;
                return false;
            }
        }

        public static bool IsSteamApiLoaded()
        {
            return !noSteam && successfullyLoadedSteamApi;
        }
        
        public static bool TryGetAchievement(string API_NAME)
        {
            if (!IsSteamApiLoaded())
            {
                //TODO: Save achievements locally?
                return false;
            }

            if (SteamManager.Initialized)
            {
                Steamworks.SteamUserStats.GetAchievement(API_NAME, out var completed);

                if (!completed)
                {
                    Steamworks.SteamUserStats.SetAchievement(API_NAME);
                    Steamworks.SteamUserStats.StoreStats();
                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool IsOnSteamDeck()
        {
            if (!IsSteamApiLoaded())
            {
                //TODO: Save achievements locally?
                return false;
            }

            return Steamworks.SteamUtils.IsSteamRunningOnSteamDeck();
        }

        /*
        public static void GetSteamController()
        {
            if (!IsSteamApiLoaded())
            {
                return;
            }

            SteamInput.Init(true);
            SteamInput.RunFrame();
            InputHandle_t[] _controllerHandles = new InputHandle_t[Steamworks.Constants.STEAM_INPUT_MAX_COUNT];
            int _numConnectedControllers = Steamworks.SteamInput.GetConnectedControllers(_controllerHandles);
            Debug.Log($"Controllers Found: {_numConnectedControllers}");
            for (int i = 0; i < _numConnectedControllers; i++)
            {
                ESteamInputType _inputType = Steamworks.SteamInput.GetInputTypeForHandle(_controllerHandles[i]);
                string _output = $"Controller [{i} | {(int)_inputType}]: ";

                switch (_inputType)
                {
                    case ESteamInputType.k_ESteamInputType_SteamController:
                    case ESteamInputType.k_ESteamInputType_SteamDeckController:
                        _output += "STEAM";
                        break;
                    case ESteamInputType.k_ESteamInputType_XBox360Controller:
                    case ESteamInputType.k_ESteamInputType_XBoxOneController:
                        _output += "XBOX";
                        break;
                    case ESteamInputType.k_ESteamInputType_PS3Controller:
                    case ESteamInputType.k_ESteamInputType_PS4Controller:
                    case ESteamInputType.k_ESteamInputType_PS5Controller:
                        _output += "PLAYSTATION";
                        break;
                    case ESteamInputType.k_ESteamInputType_SwitchJoyConPair:
                    case ESteamInputType.k_ESteamInputType_SwitchProController:
                        _output += "SWITCH";
                        break;
                    case ESteamInputType.k_ESteamInputType_GenericGamepad:
                        _output += "GENERIC";
                        break;
                    case ESteamInputType.k_ESteamInputType_Unknown:
                    default:
                        _output += "UNKNOWN";
                        break;
                }

                Debug.Log(_output);
            }
            SteamInput.Shutdown();
        }
        */
    }
}
