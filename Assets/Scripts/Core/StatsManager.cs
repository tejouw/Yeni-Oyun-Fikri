using UnityEngine;

namespace NeonSurvivors.Core
{
    public class StatsManager : MonoBehaviour
    {
        private static StatsManager _instance;
        public static StatsManager Instance => _instance;

        [Header("Lifetime Stats")]
        public int totalKills = 0;
        public int totalGamesPlayed = 0;
        public int highestWave = 0;
        public float longestSurvivalTime = 0f;
        public long totalGoldEarned = 0;

        [Header("Session Stats")]
        public int sessionsToday = 0;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RecordGameEnd(int wave, int kills, float survivalTime, int goldEarned)
        {
            totalGamesPlayed++;
            totalKills += kills;
            totalGoldEarned += goldEarned;

            if (wave > highestWave)
            {
                highestWave = wave;
                Debug.Log($"New highest wave record: {highestWave}");
            }

            if (survivalTime > longestSurvivalTime)
            {
                longestSurvivalTime = survivalTime;
                Debug.Log($"New survival time record: {longestSurvivalTime:F1}s");
            }
        }

        public void LoadStats(int kills, int gamesPlayed, int wave)
        {
            totalKills = kills;
            totalGamesPlayed = gamesPlayed;
            highestWave = wave;
        }

        public void IncrementSessionCount()
        {
            sessionsToday++;
        }
    }
}
