using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Data;

namespace NeonSurvivors.Player
{
    public class PlayerUpgrades : MonoBehaviour
    {
        [Header("Applied Upgrades")]
        public List<UpgradeData> appliedUpgrades = new List<UpgradeData>();
        private Dictionary<string, int> upgradeStacks = new Dictionary<string, int>();

        [Header("References")]
        private PlayerController controller;
        private PlayerHealth health;
        private PlayerWeapon weapon;

        void Awake()
        {
            controller = GetComponent<PlayerController>();
            health = GetComponent<PlayerHealth>();
            weapon = GetComponent<PlayerWeapon>();
        }

        public void ApplyUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null)
                return;

            // Check stacking
            if (upgradeStacks.ContainsKey(upgrade.upgradeID))
            {
                if (!upgrade.canStack || upgradeStacks[upgrade.upgradeID] >= upgrade.maxStacks)
                {
                    Debug.Log($"Upgrade {upgrade.upgradeName} cannot stack further");
                    return;
                }
                upgradeStacks[upgrade.upgradeID]++;
            }
            else
            {
                upgradeStacks[upgrade.upgradeID] = 1;
            }

            appliedUpgrades.Add(upgrade);

            // Apply stat modifiers
            ApplyStatModifiers(upgrade);

            Debug.Log($"Applied upgrade: {upgrade.upgradeName} (Stack: {upgradeStacks[upgrade.upgradeID]})");
        }

        void ApplyStatModifiers(UpgradeData upgrade)
        {
            // Damage modifiers
            if (upgrade.damageMultiplier != 1f)
            {
                weapon.MultiplyDamage(upgrade.damageMultiplier);
            }
            if (upgrade.damageAdditive != 0f)
            {
                weapon.AddDamage(upgrade.damageAdditive);
            }

            // Fire rate modifiers
            if (upgrade.fireRateMultiplier != 1f)
            {
                weapon.MultiplyFireRate(upgrade.fireRateMultiplier);
            }
            if (upgrade.fireRateAdditive != 0f)
            {
                weapon.AddFireRate(upgrade.fireRateAdditive);
            }

            // Projectile modifiers
            if (upgrade.projectileCountBonus > 0)
            {
                weapon.AddProjectileCount(upgrade.projectileCountBonus);
            }

            if (upgrade.projectileSpeedMultiplier != 1f)
            {
                weapon.SetProjectileSpeed(weapon.projectileSpeed * upgrade.projectileSpeedMultiplier);
            }

            // Health modifiers
            if (upgrade.maxHealthMultiplier != 1f)
            {
                health.MultiplyMaxHealth(upgrade.maxHealthMultiplier);
            }
            if (upgrade.maxHealthAdditive != 0f)
            {
                health.AddMaxHealth(upgrade.maxHealthAdditive);
            }

            // Special modifiers
            if (upgrade.pierceBonus > 0)
            {
                weapon.AddPierce(upgrade.pierceBonus);
            }

            if (upgrade.bounceBonus > 0)
            {
                weapon.AddBounce(upgrade.bounceBonus);
            }

            if (upgrade.explosionRadius > 0f)
            {
                weapon.SetExplosionRadius(upgrade.explosionRadius);
            }

            if (upgrade.criticalChance > 0f)
            {
                weapon.AddCriticalChance(upgrade.criticalChance);
            }

            if (upgrade.criticalMultiplier != 2f)
            {
                weapon.SetCriticalMultiplier(upgrade.criticalMultiplier);
            }

            if (upgrade.freezeChance > 0f)
            {
                weapon.AddFreezeChance(upgrade.freezeChance);
            }

            if (upgrade.burnChance > 0f)
            {
                weapon.AddBurnChance(upgrade.burnChance);
            }

            if (upgrade.chainLightningChance > 0f)
            {
                weapon.AddChainLightning(upgrade.chainLightningChance, upgrade.chainLightningTargets);
            }

            if (upgrade.lifestealPercent > 0f)
            {
                // Lifesteal would be handled in projectile hit logic
            }

            // Movement speed
            if (upgrade.moveSpeedMultiplier != 1f)
            {
                controller.ApplyUpgradeModifiers();
            }
        }

        public float GetMoveSpeedMultiplier()
        {
            float multiplier = 1f;

            foreach (var upgrade in appliedUpgrades)
            {
                multiplier *= upgrade.moveSpeedMultiplier;
            }

            return multiplier;
        }

        public float GetDamageMultiplier()
        {
            float multiplier = 1f;

            foreach (var upgrade in appliedUpgrades)
            {
                multiplier *= upgrade.damageMultiplier;
            }

            return multiplier;
        }

        public void ResetUpgrades()
        {
            appliedUpgrades.Clear();
            upgradeStacks.Clear();

            // Reset stats to base values
            if (controller != null && controller.currentShip != null)
            {
                controller.InitializeFromShipData();
            }

            Debug.Log("All upgrades reset");
        }

        public int GetUpgradeStackCount(string upgradeID)
        {
            if (upgradeStacks.ContainsKey(upgradeID))
                return upgradeStacks[upgradeID];
            return 0;
        }

        public bool HasUpgrade(string upgradeID)
        {
            return upgradeStacks.ContainsKey(upgradeID);
        }
    }
}
