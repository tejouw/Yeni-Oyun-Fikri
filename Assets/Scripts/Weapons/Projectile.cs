using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Core;
using NeonSurvivors.Enemy;

namespace NeonSurvivors.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {
        [Header("Stats")]
        public float damage = 10f;
        public float speed = 15f;
        public float lifetime = 3f;

        [Header("Special")]
        public int pierceRemaining = 0;
        public int bounceRemaining = 0;
        public float explosionRadius = 0f;

        [Header("Status Effects")]
        public float freezeChance = 0f;
        public float freezeDuration = 0f;
        public float burnChance = 0f;
        public float burnDamage = 0f;
        public float chainLightningChance = 0f;
        public int chainLightningTargets = 0;

        [Header("Runtime")]
        private Vector3 direction;
        private float lifeTimer = 0f;
        private Rigidbody rb;
        private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        // Performance: Reusable array for Physics.OverlapSphereNonAlloc
        private Collider[] overlapResults = new Collider[50];

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

            SphereCollider collider = GetComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.2f;
        }

        void OnEnable()
        {
            lifeTimer = 0f;
            hitEnemies.Clear();
        }

        void OnDisable()
        {
            // Clean up hit enemies on disable to prevent memory bloat
            hitEnemies.Clear();
        }

        public void Initialize(float dmg, Vector3 dir, float spd, int pierce, int bounce,
                               float explosion, float freezeCh, float freezeDur,
                               float burnCh, float burnDmg, float chainCh, int chainTargets)
        {
            damage = dmg;
            direction = dir.normalized;
            speed = spd;
            pierceRemaining = pierce;
            bounceRemaining = bounce;
            explosionRadius = explosion;
            freezeChance = freezeCh;
            freezeDuration = freezeDur;
            burnChance = burnCh;
            burnDamage = burnDmg;
            chainLightningChance = chainCh;
            chainLightningTargets = chainTargets;

            // Point towards direction
            transform.rotation = Quaternion.LookRotation(direction);

            lifeTimer = 0f;
            hitEnemies.Clear();
        }

        void Update()
        {
            // Move projectile
            transform.position += direction * speed * Time.deltaTime;

            // Lifetime
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifetime)
            {
                ReturnToPool();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                // Check if already hit this enemy (for pierce)
                if (hitEnemies.Contains(other.gameObject))
                    return;

                hitEnemies.Add(other.gameObject);

                // Deal damage
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                    // Apply status effects
                    ApplyStatusEffects(enemy);

                    // Explosion
                    if (explosionRadius > 0f)
                    {
                        ApplyExplosion(other.transform.position);
                    }

                    // Chain lightning
                    if (Random.value < chainLightningChance && chainLightningTargets > 0)
                    {
                        ApplyChainLightning(other.transform.position, other.gameObject);
                    }
                }

                // Pierce logic
                if (pierceRemaining > 0)
                {
                    pierceRemaining--;
                    return; // Continue flying
                }

                // Bounce logic (simplified - just reflect)
                if (bounceRemaining > 0)
                {
                    bounceRemaining--;
                    direction = Vector3.Reflect(direction, Vector3.up);
                    return;
                }

                // No pierce/bounce remaining, destroy projectile
                ReturnToPool();
            }
            else if (other.CompareTag("Wall") || other.CompareTag("Boundary"))
            {
                // Bounce off walls
                if (bounceRemaining > 0)
                {
                    bounceRemaining--;
                    // Use contact normal if available, otherwise fallback to generic reflection
                    Vector3 normal = (transform.position - other.transform.position).normalized;
                    direction = Vector3.Reflect(direction, normal);
                }
                else
                {
                    ReturnToPool();
                }
            }
        }

        void ApplyStatusEffects(EnemyBase enemy)
        {
            // Freeze
            if (Random.value < freezeChance)
            {
                enemy.ApplyFreeze(freezeDuration);
            }

            // Burn
            if (Random.value < burnChance)
            {
                enemy.ApplyBurn(burnDamage, 3f);
            }
        }

        void ApplyExplosion(Vector3 center)
        {
            // Use NonAlloc version to avoid GC allocation
            int hitCount = Physics.OverlapSphereNonAlloc(center, explosionRadius, overlapResults);

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = overlapResults[i];
                if (hit != null && hit.CompareTag("Enemy") && !hitEnemies.Contains(hit.gameObject))
                {
                    EnemyBase enemy = hit.GetComponent<EnemyBase>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage * 0.5f); // Explosion does 50% damage
                    }
                }
            }

            // Visual explosion effect (particle system would go here)
            Debug.Log($"Explosion at {center} with radius {explosionRadius}");
        }

        void ApplyChainLightning(Vector3 origin, GameObject sourceEnemy)
        {
            List<GameObject> chainedEnemies = new List<GameObject> { sourceEnemy };
            Vector3 currentPosition = origin;

            for (int i = 0; i < chainLightningTargets; i++)
            {
                GameObject nextTarget = FindNearestEnemy(currentPosition, chainedEnemies, 5f);

                if (nextTarget == null)
                    break;

                chainedEnemies.Add(nextTarget);

                EnemyBase enemy = nextTarget.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage * 0.3f); // Chain does 30% damage
                }

                currentPosition = nextTarget.transform.position;

                // Visual lightning effect would go here
                Debug.Log($"Chain lightning jumped to {nextTarget.name}");
            }
        }

        GameObject FindNearestEnemy(Vector3 position, List<GameObject> exclude, float maxRange)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float minDistance = Mathf.Infinity;
            GameObject nearest = null;

            foreach (GameObject enemy in enemies)
            {
                if (!enemy.activeInHierarchy || exclude.Contains(enemy))
                    continue;

                float distance = Vector3.Distance(position, enemy.transform.position);

                if (distance < maxRange && distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        void ReturnToPool()
        {
            hitEnemies.Clear();

            // Null check for PoolManager
            if (PoolManager.Instance != null)
            {
                PoolManager.Instance.ReturnToPool("Projectile_Player", gameObject);
            }
            else
            {
                // Fallback: Destroy if pool manager not available
                Destroy(gameObject);
            }
        }
    }
}
