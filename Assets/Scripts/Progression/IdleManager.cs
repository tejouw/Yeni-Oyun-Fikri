using UnityEngine;
using System;
using NeonSurvivors.Core;

namespace NeonSurvivors.Player
{
    public class IdleManager : MonoBehaviour
    {
        private static IdleManager _instance;
        public static IdleManager Instance => _instance;

        [Header("Idle Earnings")]
        public float baseGoldPerSecond = 10f;
        public float maxOfflineHours = 4f;

        [Header("Current Offline Earnings")]
        private int pendingOfflineGold = 0;
        private float hoursOffline = 0f;

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

        public void CalculateOfflineEarnings(TimeSpan timeOffline)
        {
            float hours = (float)timeOffline.TotalHours;
            hours = Mathf.Min(hours, maxOfflineHours); // Cap at 4 hours

            if (hours < 0.016f) // Less than 1 minute
            {
                Debug.Log("No offline earnings (less than 1 minute offline)");
                return;
            }

            // Calculate earnings based on highest wave reached
            float goldPerSecond = CalculateIdleGPS();

            // Prestige multiplier
            float prestigeMultiplier = 1f;
            if (ProgressionManager.Instance != null)
            {
                prestigeMultiplier = ProgressionManager.Instance.GetPrestigeGoldMultiplier();
            }

            int offlineGold = Mathf.RoundToInt(goldPerSecond * hours * 3600f * 0.5f * prestigeMultiplier);

            pendingOfflineGold = offlineGold;
            hoursOffline = hours;

            // Show offline earnings UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowOfflineEarnings(offlineGold, hours);
            }
            else
            {
                // Auto-claim if UI not available
                ClaimOfflineEarnings(false);
            }

            Debug.Log($"Offline for {hours:F1} hours. Earned {offlineGold} gold (base: {goldPerSecond}/s)");
        }

        float CalculateIdleGPS()
        {
            // Base on highest wave reached
            int highestWave = 0;
            if (StatsManager.Instance != null)
            {
                highestWave = StatsManager.Instance.highestWave;
            }

            float gps = baseGoldPerSecond * (1f + highestWave * 0.1f);
            return gps;
        }

        public void ClaimOfflineEarnings(bool watchedAd)
        {
            int finalAmount = watchedAd ? pendingOfflineGold * 3 : pendingOfflineGold;

            GameManager.Instance.AddGold(finalAmount);

            Debug.Log($"Claimed {finalAmount} offline gold (ad multiplier: {(watchedAd ? "3x" : "1x")})");

            pendingOfflineGold = 0;
            hoursOffline = 0f;
        }

        public int GetPendingOfflineGold()
        {
            return pendingOfflineGold;
        }

        public float GetHoursOffline()
        {
            return hoursOffline;
        }
    }
}
