using UnityEngine;

namespace NeonSurvivors.Data
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "Neon Survivors/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Identity")]
        public string upgradeID;
        public string upgradeName;
        [TextArea(3, 5)]
        public string description;

        [Header("Category")]
        public UpgradeCategory category;
        public UpgradeRarity rarity;

        [Header("Stat Modifiers")]
        public float damageMultiplier = 1f;
        public float damageAdditive = 0f;
        public float fireRateMultiplier = 1f;
        public float fireRateAdditive = 0f;
        public int projectileCountBonus = 0;
        public float projectileSpeedMultiplier = 1f;
        public float moveSpeedMultiplier = 1f;
        public float maxHealthMultiplier = 1f;
        public float maxHealthAdditive = 0f;

        [Header("Special Modifiers")]
        public int pierceBonus = 0;
        public int bounceBonus = 0;
        public float explosionRadius = 0f;
        public float chainLightningChance = 0f;
        public int chainLightningTargets = 0;
        public float freezeChance = 0f;
        public float freezeDuration = 0f;
        public float burnChance = 0f;
        public float burnDamagePerSecond = 0f;
        public float criticalChance = 0f;
        public float criticalMultiplier = 2f;
        public float lifestealPercent = 0f;

        [Header("Visual")]
        public Color iconColor = Color.cyan;

        [Header("Stacking")]
        public bool canStack = true;
        public int maxStacks = 10;

        public string GetFormattedDescription()
        {
            string result = description;

            // Replace placeholders
            if (damageMultiplier != 1f)
                result = result.Replace("{damage}", $"+{(damageMultiplier - 1f) * 100f:F0}%");
            if (damageAdditive != 0f)
                result = result.Replace("{damage}", $"+{damageAdditive:F0}");
            if (fireRateMultiplier != 1f)
                result = result.Replace("{firerate}", $"+{(fireRateMultiplier - 1f) * 100f:F0}%");
            if (projectileCountBonus > 0)
                result = result.Replace("{projectiles}", $"+{projectileCountBonus}");
            if (pierceBonus > 0)
                result = result.Replace("{pierce}", $"+{pierceBonus}");
            if (bounceBonus > 0)
                result = result.Replace("{bounce}", $"+{bounceBonus}");

            return result;
        }
    }

    public enum UpgradeCategory
    {
        Damage,
        FireRate,
        Projectile,
        Mobility,
        Survival,
        Special
    }

    public enum UpgradeRarity
    {
        Common,     // 60%
        Uncommon,   // 25%
        Rare,       // 12%
        Epic        // 3%
    }
}
