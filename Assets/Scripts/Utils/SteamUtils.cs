using UnityEngine;
using Steamworks;

namespace JetEngine
{
    public static class SteamUtils
    {
        public static bool TryGetAchievement(string API_NAME)
        {
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
    }
}
