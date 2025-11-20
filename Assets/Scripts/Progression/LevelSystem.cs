using UnityEngine;
using System.Collections.Generic;
using System;
using NeonSurvivors.Data;
using NeonSurvivors.Core;

namespace NeonSurvivors.Player
{
    public class LevelSystem : MonoBehaviour
    {
        private static LevelSystem _instance;
        public static LevelSystem Instance => _instance;

        [Header("Level Progress")]
        public int currentLevel = 1;
        public int currentXP = 0;
        public int xpToNextLevel = 1000;

        [Header("Upgrade Pool")]
        public List<UpgradeData> allUpgrades = new List<UpgradeData>();

        [Header("Events")]
        public event Action<int> OnLevelUp;
        public event Action<int, int> OnXPGained; // current, required

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        void Start()
        {
            LoadUpgradeData();
        }

        void LoadUpgradeData()
        {
            // Load all upgrade ScriptableObjects from Resources
            UpgradeData[] upgrades = Resources.LoadAll<UpgradeData>("Data/Upgrades");
            if (upgrades.Length > 0)
            {
                allUpgrades.AddRange(upgrades);
                Debug.Log($"Loaded {upgrades.Length} upgrade types");
            }
            else
            {
                Debug.LogWarning("No upgrade data found in Resources/Data/Upgrades");
                CreateDefaultUpgrades();
            }
        }

        void CreateDefaultUpgrades()
        {
            // Create some default upgrades programmatically if none exist
            // This is a fallback
            Debug.Log("Creating default upgrades");
        }

        public void AddXP(int amount)
        {
            currentXP += amount;
            OnXPGained?.Invoke(currentXP, xpToNextLevel);

            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }

        void LevelUp()
        {
            currentLevel++;
            currentXP -= xpToNextLevel;
            xpToNextLevel = Mathf.RoundToInt(1000 * Mathf.Pow(1.1f, currentLevel - 1));

            OnLevelUp?.Invoke(currentLevel);

            // Show upgrade selection UI
            ShowUpgradeSelection();

            Debug.Log($"Level Up! Now level {currentLevel}. Next level requires {xpToNextLevel} XP");
        }

        void ShowUpgradeSelection()
        {
            if (allUpgrades.Count == 0)
            {
                Debug.LogWarning("No upgrades available to show");
                return;
            }

            List<UpgradeData> selectedUpgrades = SelectThreeRandomUpgrades();

            // Pause game
            Time.timeScale = 0f;

            // Show UI (will be implemented in UI system)
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowLevelUpScreen(selectedUpgrades);
            }
            else
            {
                Debug.LogWarning("UIManager not found, cannot show level up screen");
                Time.timeScale = 1f; // Resume if UI not available
            }
        }

        List<UpgradeData> SelectThreeRandomUpgrades()
        {
            List<UpgradeData> selected = new List<UpgradeData>();
            List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

            for (int i = 0; i < 3 && pool.Count > 0; i++)
            {
                UpgradeData upgrade = SelectWeightedRandomUpgrade(pool);
                selected.Add(upgrade);
                pool.Remove(upgrade);
            }

            return selected;
        }

        UpgradeData SelectWeightedRandomUpgrade(List<UpgradeData> pool)
        {
            float roll = Random.Range(0f, 1f);

            // Rarity weights: Common 60%, Uncommon 25%, Rare 12%, Epic 3%
            UpgradeRarity targetRarity;
            if (roll < 0.60f)
                targetRarity = UpgradeRarity.Common;
            else if (roll < 0.85f)
                targetRarity = UpgradeRarity.Uncommon;
            else if (roll < 0.97f)
                targetRarity = UpgradeRarity.Rare;
            else
                targetRarity = UpgradeRarity.Epic;

            // Find upgrades of target rarity
            List<UpgradeData> rarityPool = pool.FindAll(u => u.rarity == targetRarity);

            // Fallback to any upgrade if none of target rarity
            if (rarityPool.Count == 0)
                return pool[Random.Range(0, pool.Count)];

            return rarityPool[Random.Range(0, rarityPool.Count)];
        }

        public void OnUpgradeSelected(UpgradeData upgrade)
        {
            // Apply upgrade to player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerUpgrades playerUpgrades = player.GetComponent<PlayerUpgrades>();
                if (playerUpgrades != null)
                {
                    playerUpgrades.ApplyUpgrade(upgrade);
                }
            }

            // Resume game
            Time.timeScale = 1f;

            Debug.Log($"Selected upgrade: {upgrade.upgradeName}");
        }

        public void Reset()
        {
            currentLevel = 1;
            currentXP = 0;
            xpToNextLevel = 1000;
        }
    }
}
