using UnityEngine;
using System;
using System.Collections.Generic;

namespace NeonSurvivors.Core
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance;
        public static SaveManager Instance => _instance;

        private const string SAVE_KEY = "NeonSurvivors_GameData";
        private const float AUTO_SAVE_INTERVAL = 30f;

        private float autoSaveTimer = 0f;

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

        void Update()
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
            {
                SaveGame();
                autoSaveTimer = 0f;
            }
        }

        public void SaveGame()
        {
            try
            {
                GameData data = new GameData
                {
                    // Currency
                    currentGold = GameManager.Instance.currentGold,
                    currentGems = GameManager.Instance.currentGems,

                    // Progression
                    prestigePoints = ProgressionManager.Instance != null ? ProgressionManager.Instance.prestigePoints : 0,
                    totalLifetimeGold = ProgressionManager.Instance != null ? ProgressionManager.Instance.totalLifetimeGold : 0,

                    // Ships
                    unlockedShipIDs = ShipManager.Instance != null ? ShipManager.Instance.GetUnlockedShipIDs() : new List<string> { "ship_starter" },
                    currentShipID = ShipManager.Instance != null ? ShipManager.Instance.GetCurrentShipID() : "ship_starter",

                    // Workshop
                    workshopUpgrades = WorkshopManager.Instance != null ? WorkshopManager.Instance.GetUpgradeLevels() : new Dictionary<string, int>(),

                    // Stats
                    totalKills = StatsManager.Instance != null ? StatsManager.Instance.totalKills : 0,
                    totalGamesPlayed = StatsManager.Instance != null ? StatsManager.Instance.totalGamesPlayed : 0,
                    highestWave = StatsManager.Instance != null ? StatsManager.Instance.highestWave : 0,

                    // Timestamp
                    lastSaveTime = DateTime.Now.ToString("o")
                };

                string json = JsonUtility.ToJson(data);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();

                Debug.Log("Game saved successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (!PlayerPrefs.HasKey(SAVE_KEY))
                {
                    Debug.Log("No save data found, starting new game");
                    InitializeNewGame();
                    return;
                }

                string json = PlayerPrefs.GetString(SAVE_KEY);
                GameData data = JsonUtility.FromJson<GameData>(json);

                // Apply loaded data
                GameManager.Instance.currentGold = data.currentGold;
                GameManager.Instance.currentGems = data.currentGems;

                if (ProgressionManager.Instance != null)
                {
                    ProgressionManager.Instance.prestigePoints = data.prestigePoints;
                    ProgressionManager.Instance.totalLifetimeGold = data.totalLifetimeGold;
                }

                if (ShipManager.Instance != null)
                {
                    ShipManager.Instance.LoadShipData(data.unlockedShipIDs, data.currentShipID);
                }

                if (WorkshopManager.Instance != null)
                {
                    WorkshopManager.Instance.LoadUpgradeLevels(data.workshopUpgrades);
                }

                if (StatsManager.Instance != null)
                {
                    StatsManager.Instance.LoadStats(data.totalKills, data.totalGamesPlayed, data.highestWave);
                }

                // Calculate offline earnings
                CalculateOfflineEarnings(data.lastSaveTime);

                Debug.Log("Game loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                InitializeNewGame();
            }
        }

        void InitializeNewGame()
        {
            GameManager.Instance.currentGold = 0;
            GameManager.Instance.currentGems = 0;

            if (ShipManager.Instance != null)
            {
                ShipManager.Instance.UnlockShip("ship_starter");
                ShipManager.Instance.SetCurrentShip("ship_starter");
            }

            Debug.Log("New game initialized");
        }

        void CalculateOfflineEarnings(string lastSaveTimeString)
        {
            if (IdleManager.Instance == null)
                return;

            try
            {
                DateTime lastSaveTime = DateTime.Parse(lastSaveTimeString);
                TimeSpan timeOffline = DateTime.Now - lastSaveTime;

                if (timeOffline.TotalMinutes < 1)
                    return; // Less than 1 minute, no offline earnings

                IdleManager.Instance.CalculateOfflineEarnings(timeOffline);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to calculate offline earnings: {e.Message}");
            }
        }

        public DateTime GetLastSaveTime()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
                return DateTime.Now;

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                GameData data = JsonUtility.FromJson<GameData>(json);
                return DateTime.Parse(data.lastSaveTime);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public void DeleteSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted");
        }
    }

    [System.Serializable]
    public class GameData
    {
        // Currency
        public long currentGold;
        public int currentGems;

        // Progression
        public int prestigePoints;
        public long totalLifetimeGold;

        // Ships
        public List<string> unlockedShipIDs;
        public string currentShipID;

        // Workshop
        public Dictionary<string, int> workshopUpgrades;

        // Stats
        public int totalKills;
        public int totalGamesPlayed;
        public int highestWave;

        // Timestamp
        public string lastSaveTime;
    }

    // Custom JSON helper for Dictionary serialization
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            for (int i = 0; i < keys.Count; i++)
            {
                result[keys[i]] = values[i];
            }
            return result;
        }
    }
}
