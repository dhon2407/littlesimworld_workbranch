using UnityEngine;

namespace PlayerStats
{
    public class PlayerStatsManager : MonoBehaviour
    {
        private static PlayerStatsManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Stats.Initialize(this);
        }
    }

    public static class Stats
    {
        private static PlayerStatsManager manager = null;

        public static bool Ready => (manager != null);

        public static void Initialize(PlayerStatsManager statsManager)
        {
            manager = statsManager;
        }
    }
}
