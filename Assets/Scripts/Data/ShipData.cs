using UnityEngine;

namespace NeonSurvivors.Data
{
    [CreateAssetMenu(fileName = "New Ship", menuName = "Neon Survivors/Ship Data")]
    public class ShipData : ScriptableObject
    {
        [Header("Identity")]
        public string shipID;
        public string shipName;
        [TextArea(3, 5)]
        public string description;

        [Header("Base Stats")]
        public float maxHealth = 100f;
        public float moveSpeed = 5f;
        public float baseDamage = 10f;
        public float fireRate = 1f;
        public float pickupRange = 2f;

        [Header("Visual")]
        public ShipMeshType meshType;
        public Color primaryColor = Color.cyan;
        public Color emissionColor = Color.cyan;
        public float meshScale = 1f;

        [Header("Ability")]
        public ShipAbilityType abilityType;
        public float abilityValue;
        [TextArea(2, 3)]
        public string abilityDescription;

        [Header("Unlock Condition")]
        public UnlockType unlockType;
        public int unlockValue;
        public string unlockDescription;

        [Header("Monetization")]
        public bool isPremium = false;
        public float priceUSD = 0.99f;
    }

    public enum ShipMeshType
    {
        Triangle,
        Pentagon,
        Arrow,
        Diamond,
        Star,
        Hexagon
    }

    public enum ShipAbilityType
    {
        None,
        GoldBonus,          // +X% gold earning
        DamageReduction,    // X% damage reduction
        DodgeChance,        // X% chance to dodge
        PierceBonus,        // +X pierce count
        ExtraProjectiles,   // +X projectiles
        HealthRegen,        // X HP per second
        SpeedBoost,         // +X% movement speed
        FireRateBonus,      // +X% fire rate
        ExpBonus            // +X% XP gain
    }

    public enum UnlockType
    {
        Default,            // Starter ship
        ReachWave,          // Reach wave X
        KillEnemies,        // Kill X enemies total
        SurviveTime,        // Survive X seconds in one run
        PrestigeCount,      // Prestige X times
        SpendGold,          // Spend X gold total
        Premium             // IAP only
    }
}
