using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NeonSurvivors.Data;
using NeonSurvivors.Core;

namespace NeonSurvivors.Player
{
    public class ShipManager : MonoBehaviour
    {
        private static ShipManager _instance;
        public static ShipManager Instance => _instance;

        [Header("Ships")]
        public List<ShipData> allShips = new List<ShipData>();
        private List<string> unlockedShipIDs = new List<string>();
        private string currentShipID = "ship_starter";

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

        void Start()
        {
            LoadShipData();

            // Unlock starter ship by default
            if (unlockedShipIDs.Count == 0)
            {
                UnlockShip("ship_starter");
            }
        }

        void LoadShipData()
        {
            // Load ships from GameDataInitializer
            if (GameDataInitializer.Instance != null)
            {
                allShips = GameDataInitializer.Instance.GetAllShips();
                Debug.Log($"Loaded {allShips.Count} ship types from GameDataInitializer");
            }
            else
            {
                Debug.LogWarning("GameDataInitializer not found! No ships loaded.");
            }
        }

        public void UnlockShip(string shipID)
        {
            if (unlockedShipIDs.Contains(shipID))
            {
                Debug.Log($"Ship {shipID} already unlocked");
                return;
            }

            unlockedShipIDs.Add(shipID);
            Debug.Log($"Unlocked ship: {shipID}");

            SaveManager.Instance.SaveGame();
        }

        public void SetCurrentShip(string shipID)
        {
            if (!unlockedShipIDs.Contains(shipID))
            {
                Debug.LogWarning($"Cannot set ship {shipID} - not unlocked");
                return;
            }

            currentShipID = shipID;
            Debug.Log($"Current ship set to: {shipID}");

            SaveManager.Instance.SaveGame();
        }

        public ShipData GetCurrentShip()
        {
            return allShips.Find(s => s.shipID == currentShipID);
        }

        public ShipData GetShip(string shipID)
        {
            return allShips.Find(s => s.shipID == shipID);
        }

        public bool IsShipUnlocked(string shipID)
        {
            return unlockedShipIDs.Contains(shipID);
        }

        public List<string> GetUnlockedShipIDs()
        {
            return new List<string>(unlockedShipIDs);
        }

        public string GetCurrentShipID()
        {
            return currentShipID;
        }

        public void LoadShipData(List<string> unlocked, string current)
        {
            unlockedShipIDs = unlocked ?? new List<string> { "ship_starter" };
            currentShipID = current ?? "ship_starter";

            Debug.Log($"Loaded ship data: {unlockedShipIDs.Count} unlocked, current: {currentShipID}");
        }

        public void CheckUnlockConditions()
        {
            foreach (var ship in allShips)
            {
                if (IsShipUnlocked(ship.shipID))
                    continue;

                if (ship.isPremium)
                    continue; // Premium ships only unlocked via IAP

                bool shouldUnlock = false;

                switch (ship.unlockType)
                {
                    case UnlockType.Default:
                        shouldUnlock = true;
                        break;

                    case UnlockType.ReachWave:
                        if (StatsManager.Instance != null && StatsManager.Instance.highestWave >= ship.unlockValue)
                            shouldUnlock = true;
                        break;

                    case UnlockType.KillEnemies:
                        if (StatsManager.Instance != null && StatsManager.Instance.totalKills >= ship.unlockValue)
                            shouldUnlock = true;
                        break;

                    case UnlockType.SurviveTime:
                        if (StatsManager.Instance != null && StatsManager.Instance.longestSurvivalTime >= ship.unlockValue)
                            shouldUnlock = true;
                        break;

                    case UnlockType.PrestigeCount:
                        if (ProgressionManager.Instance != null && ProgressionManager.Instance.prestigePoints >= ship.unlockValue)
                            shouldUnlock = true;
                        break;

                    case UnlockType.SpendGold:
                        if (ProgressionManager.Instance != null && ProgressionManager.Instance.totalLifetimeGold >= ship.unlockValue)
                            shouldUnlock = true;
                        break;
                }

                if (shouldUnlock)
                {
                    UnlockShip(ship.shipID);

                    // Show unlock notification
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowShipUnlockNotification(ship);
                    }
                }
            }
        }

        public int GetTotalShipCount()
        {
            return allShips.Count;
        }

        public int GetUnlockedShipCount()
        {
            return unlockedShipIDs.Count;
        }
    }
}
