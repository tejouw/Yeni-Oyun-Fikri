using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Data;
using NeonSurvivors.Weapons;

namespace NeonSurvivors.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("Current Weapon")]
        public WeaponData currentWeaponData;

        [Header("Base Stats")]
        public float baseDamage = 10f;
        public float fireRate = 1f;
        public int projectileCount = 1;
        public float projectileSpeed = 15f;

        [Header("Special Modifiers")]
        public int pierceCount = 0;
        public int bounceCount = 0;
        public float explosionRadius = 0f;
        public float criticalChance = 0f;
        public float criticalMultiplier = 2f;

        [Header("Status Effects")]
        public float freezeChance = 0f;
        public float freezeDuration = 0f;
        public float burnChance = 0f;
        public float burnDamage = 0f;
        public float chainLightningChance = 0f;
        public int chainLightningTargets = 0;

        [Header("Auto-Target")]
        public float targetRange = 20f;
        public LayerMask enemyLayer;
        private Transform currentTarget;

        [Header("Firing")]
        private float fireTimer = 0f;

        void Update()
        {
            // Null check for GameManager
            if (GameManager.Instance == null || !GameManager.Instance.isGameRunning)
                return;

            fireTimer += Time.deltaTime;

            if (fireTimer >= 1f / fireRate)
            {
                FindNearestTarget();
                if (currentTarget != null)
                {
                    FireWeapon();
                    fireTimer = 0f;
                }
            }
        }

        void FindNearestTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length == 0)
            {
                currentTarget = null;
                return;
            }

            float minDistance = Mathf.Infinity;
            Transform nearest = null;

            foreach (GameObject enemy in enemies)
            {
                if (!enemy.activeInHierarchy)
                    continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < targetRange && distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy.transform;
                }
            }

            currentTarget = nearest;
        }

        void FireWeapon()
        {
            if (currentTarget == null)
                return;

            Vector3 direction = (currentTarget.position - transform.position).normalized;

            // Fire multiple projectiles with spread
            for (int i = 0; i < projectileCount; i++)
            {
                float spreadAngle = 0f;

                if (projectileCount > 1)
                {
                    spreadAngle = ((i - (projectileCount - 1) / 2f) * 10f);
                }

                Vector3 spreadDirection = Quaternion.Euler(0, spreadAngle, 0) * direction;

                FireProjectile(spreadDirection);
            }
        }

        void FireProjectile(Vector3 direction)
        {
            // Null check for PoolManager
            if (PoolManager.Instance == null)
            {
                Debug.LogWarning("PoolManager not found!");
                return;
            }

            // Spawn projectile from pool
            GameObject projectileObj = PoolManager.Instance.SpawnFromPool("Projectile_Player", transform.position, Quaternion.identity);

            if (projectileObj == null)
            {
                Debug.LogWarning("Failed to spawn player projectile from pool");
                return;
            }

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if (projectile != null)
            {
                // Calculate final damage
                float finalDamage = baseDamage;

                // Apply critical hit
                if (Random.value < criticalChance)
                {
                    finalDamage *= criticalMultiplier;
                }

                // Initialize projectile
                projectile.Initialize(
                    finalDamage,
                    direction,
                    projectileSpeed,
                    pierceCount,
                    bounceCount,
                    explosionRadius,
                    freezeChance,
                    freezeDuration,
                    burnChance,
                    burnDamage,
                    chainLightningChance,
                    chainLightningTargets
                );
            }
        }

        // ==================== SETTERS ====================

        public void SetBaseDamage(float damage)
        {
            baseDamage = damage;
        }

        public void SetFireRate(float rate)
        {
            fireRate = rate;
        }

        public void AddProjectileCount(int count)
        {
            projectileCount += count;
        }

        public void SetProjectileSpeed(float speed)
        {
            projectileSpeed = speed;
        }

        // ==================== MODIFIERS ====================

        public void MultiplyDamage(float multiplier)
        {
            baseDamage *= multiplier;
        }

        public void AddDamage(float additionalDamage)
        {
            baseDamage += additionalDamage;
        }

        public void MultiplyFireRate(float multiplier)
        {
            fireRate *= multiplier;
        }

        public void AddFireRate(float additionalRate)
        {
            fireRate += additionalRate;
        }

        public void AddPierce(int pierce)
        {
            pierceCount += pierce;
        }

        public void AddBounce(int bounce)
        {
            bounceCount += bounce;
        }

        public void SetExplosionRadius(float radius)
        {
            explosionRadius = radius;
        }

        public void AddCriticalChance(float chance)
        {
            criticalChance = Mathf.Min(1f, criticalChance + chance);
        }

        public void SetCriticalMultiplier(float multiplier)
        {
            criticalMultiplier = multiplier;
        }

        public void AddFreezeChance(float chance)
        {
            freezeChance = Mathf.Min(1f, freezeChance + chance);
        }

        public void AddBurnChance(float chance)
        {
            burnChance = Mathf.Min(1f, burnChance + chance);
        }

        public void AddChainLightning(float chance, int targets)
        {
            chainLightningChance = Mathf.Min(1f, chainLightningChance + chance);
            chainLightningTargets = Mathf.Max(chainLightningTargets, targets);
        }
    }
}
