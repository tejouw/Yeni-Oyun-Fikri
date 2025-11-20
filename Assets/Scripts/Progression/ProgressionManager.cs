using UnityEngine;
using NeonSurvivors.Core;

namespace NeonSurvivors.Player
{
    public class ProgressionManager : MonoBehaviour
    {
        private static ProgressionManager _instance;
        public static ProgressionManager Instance => _instance;

        [Header("Prestige")]
        public int prestigePoints = 0;
        public long totalLifetimeGold = 0;

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

        public void Prestige()
        {
            // Calculate prestige points earned
            int ppEarned = CalculatePrestigePoints();

            if (ppEarned == 0)
            {
                Debug.Log("Not enough lifetime gold to earn prestige points");
                return;
            }

            prestigePoints += ppEarned;

            // Update lifetime gold
            totalLifetimeGold += GameManager.Instance.currentGold;

            // Reset run progress
            ResetRunProgress();

            // Show prestige rewards UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPrestigeRewards(ppEarned);
            }

            SaveManager.Instance.SaveGame();

            Debug.Log($"Prestiged! Earned {ppEarned} prestige points. Total: {prestigePoints}");
        }

        int CalculatePrestigePoints()
        {
            // Formula: PP = 150 × √(totalGold / 10^6)
            long totalGold = totalLifetimeGold + GameManager.Instance.currentGold;
            float pp = 150f * Mathf.Sqrt(totalGold / 1000000f);
            return Mathf.FloorToInt(pp) - prestigePoints; // Only new PP earned
        }

        void ResetRunProgress()
        {
            // Reset level system
            if (LevelSystem.Instance != null)
            {
                LevelSystem.Instance.Reset();
            }

            // Reset player upgrades
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerUpgrades upgrades = player.GetComponent<PlayerUpgrades>();
                if (upgrades != null)
                {
                    upgrades.ResetUpgrades();
                }
            }

            // Keep persistent: prestigePoints, workshop upgrades, ship unlocks
            Debug.Log("Run progress reset");
        }

        public float GetPrestigeDamageMultiplier()
        {
            return 1f + (prestigePoints * 0.05f); // +5% per PP
        }

        public float GetPrestigeGoldMultiplier()
        {
            return 1f + (prestigePoints * 0.03f); // +3% per PP
        }

        public float GetPrestigeSpeedMultiplier()
        {
            return 1f + (prestigePoints * 0.02f); // +2% per PP
        }

        public bool CanPrestige()
        {
            return CalculatePrestigePoints() > 0;
        }

        public int GetPotentialPrestigePoints()
        {
            return CalculatePrestigePoints();
        }
    }
}
