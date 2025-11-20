using UnityEngine;
using System.Collections.Generic;

namespace NeonSurvivors.Player
{
    public class WorkshopManager : MonoBehaviour
    {
        private static WorkshopManager _instance;
        public static WorkshopManager Instance => _instance;

        [System.Serializable]
        public class WorkshopUpgrade
        {
            public string upgradeID;
            public string upgradeName;
            public string description;
            public int currentLevel;
            public int maxLevel;
            public long baseCost;
            public float costMultiplier;
            public UpgradeStatType statType;
            public float valuePerLevel;
        }

        public enum UpgradeStatType
        {
            MaxHealth,
            Damage,
            MoveSpeed,
            PickupRange,
            GoldMultiplier
        }

        [Header("Upgrades")]
        public List<WorkshopUpgrade> workshopUpgrades = new List<WorkshopUpgrade>();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUpgrades();
        }

        void InitializeUpgrades()
        {
            workshopUpgrades = new List<WorkshopUpgrade>
            {
                new WorkshopUpgrade
                {
                    upgradeID = "max_health",
                    upgradeName = "Max Health",
                    description = "+50 Max HP",
                    currentLevel = 0,
                    maxLevel = 10,
                    baseCost = 100,
                    costMultiplier = 1.5f,
                    statType = UpgradeStatType.MaxHealth,
                    valuePerLevel = 50f
                },
                new WorkshopUpgrade
                {
                    upgradeID = "damage",
                    upgradeName = "Damage",
                    description = "+20% Damage",
                    currentLevel = 0,
                    maxLevel = 10,
                    baseCost = 150,
                    costMultiplier = 1.5f,
                    statType = UpgradeStatType.Damage,
                    valuePerLevel = 0.2f
                },
                new WorkshopUpgrade
                {
                    upgradeID = "move_speed",
                    upgradeName = "Move Speed",
                    description = "+10% Speed",
                    currentLevel = 0,
                    maxLevel = 8,
                    baseCost = 120,
                    costMultiplier = 1.5f,
                    statType = UpgradeStatType.MoveSpeed,
                    valuePerLevel = 0.1f
                },
                new WorkshopUpgrade
                {
                    upgradeID = "pickup_range",
                    upgradeName = "Pickup Range",
                    description = "+15% Range",
                    currentLevel = 0,
                    maxLevel = 5,
                    baseCost = 100,
                    costMultiplier = 1.5f,
                    statType = UpgradeStatType.PickupRange,
                    valuePerLevel = 0.15f
                },
                new WorkshopUpgrade
                {
                    upgradeID = "gold_multiplier",
                    upgradeName = "Gold Bonus",
                    description = "+25% Gold",
                    currentLevel = 0,
                    maxLevel = 15,
                    baseCost = 200,
                    costMultiplier = 1.5f,
                    statType = UpgradeStatType.GoldMultiplier,
                    valuePerLevel = 0.25f
                }
            };
        }

        public bool PurchaseUpgrade(string upgradeID)
        {
            WorkshopUpgrade upgrade = workshopUpgrades.Find(u => u.upgradeID == upgradeID);

            if (upgrade == null)
            {
                Debug.LogWarning($"Upgrade {upgradeID} not found");
                return false;
            }

            if (upgrade.currentLevel >= upgrade.maxLevel)
            {
                Debug.Log($"Upgrade {upgrade.upgradeName} is already at max level");
                return false;
            }

            long cost = GetUpgradeCost(upgrade);

            if (Core.GameManager.Instance.SpendGold(cost))
            {
                upgrade.currentLevel++;
                Debug.Log($"Purchased {upgrade.upgradeName} level {upgrade.currentLevel}");
                SaveManager.Instance.SaveGame();
                return true;
            }
            else
            {
                Debug.Log($"Not enough gold to purchase {upgrade.upgradeName}. Cost: {cost}");
                return false;
            }
        }

        public long GetUpgradeCost(WorkshopUpgrade upgrade)
        {
            return (long)(upgrade.baseCost * Mathf.Pow(upgrade.costMultiplier, upgrade.currentLevel));
        }

        public float GetTotalStatBonus(UpgradeStatType statType)
        {
            float total = 0f;

            foreach (var upgrade in workshopUpgrades)
            {
                if (upgrade.statType == statType)
                {
                    total += upgrade.valuePerLevel * upgrade.currentLevel;
                }
            }

            return total;
        }

        public Dictionary<string, int> GetUpgradeLevels()
        {
            Dictionary<string, int> levels = new Dictionary<string, int>();

            foreach (var upgrade in workshopUpgrades)
            {
                levels[upgrade.upgradeID] = upgrade.currentLevel;
            }

            return levels;
        }

        public void LoadUpgradeLevels(Dictionary<string, int> levels)
        {
            if (levels == null)
                return;

            foreach (var kvp in levels)
            {
                WorkshopUpgrade upgrade = workshopUpgrades.Find(u => u.upgradeID == kvp.Key);
                if (upgrade != null)
                {
                    upgrade.currentLevel = kvp.Value;
                }
            }

            Debug.Log("Workshop upgrades loaded");
        }

        public WorkshopUpgrade GetUpgrade(string upgradeID)
        {
            return workshopUpgrades.Find(u => u.upgradeID == upgradeID);
        }
    }
}
