using UnityEngine;
using NeonSurvivors.Data;
using NeonSurvivors.Core;
using NeonSurvivors.Utilities;
using NeonSurvivors.Player;

namespace NeonSurvivors.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class EnemyBase : MonoBehaviour
    {
        [Header("Data")]
        public EnemyData enemyData;

        [Header("Stats")]
        public float maxHealth = 10f;
        public float currentHealth = 10f;
        public float moveSpeed = 3f;
        public float damage = 10f;
        public int goldValue = 10;
        public int xpValue = 10;

        [Header("Status Effects")]
        public bool isFrozen = false;
        public float freezeTimer = 0f;
        public float originalMoveSpeed = 0f;
        public bool isBurning = false;
        public float burnTimer = 0f;
        public float burnDamage = 0f;

        [Header("References")]
        private Transform player;
        private Rigidbody rb;
        private Vector3 moveDirection;

        [Header("Behavior")]
        private float behaviorTimer = 0f;
        private bool isInitialized = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

            SphereCollider collider = GetComponent<SphereCollider>();
            collider.isTrigger = false;
            collider.radius = 0.5f;
        }

        void OnEnable()
        {
            // Find player when spawned
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        public void Initialize(EnemyData data, int waveNumber)
        {
            enemyData = data;

            // Scale stats with wave
            maxHealth = data.baseHealth * (1 + waveNumber * 0.15f);
            currentHealth = maxHealth;
            moveSpeed = data.moveSpeed * (1 + waveNumber * 0.05f);
            damage = data.damage * (1 + waveNumber * 0.1f);
            goldValue = data.goldValue * (1 + waveNumber / 5);
            xpValue = data.xpValue;

            // Reset status
            isFrozen = false;
            isBurning = false;
            freezeTimer = 0f;
            burnTimer = 0f;

            isInitialized = true;

            // Generate visual
            GenerateEnemyMesh();

            // Find player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        void Update()
        {
            if (!isInitialized || player == null)
                return;

            // Update status effects
            UpdateStatusEffects();

            // Update behavior
            if (!isFrozen)
            {
                UpdateBehavior();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }

            behaviorTimer += Time.deltaTime;
        }

        void UpdateStatusEffects()
        {
            // Freeze
            if (isFrozen)
            {
                freezeTimer -= Time.deltaTime;
                if (freezeTimer <= 0)
                {
                    isFrozen = false;
                    // Restore original move speed
                    moveSpeed = originalMoveSpeed;
                }
            }

            // Burn (DOT)
            if (isBurning)
            {
                burnTimer -= Time.deltaTime;
                TakeDamage(burnDamage * Time.deltaTime);

                if (burnTimer <= 0)
                {
                    isBurning = false;
                }
            }
        }

        void UpdateBehavior()
        {
            switch (enemyData.behavior)
            {
                case EnemyBehavior.ChasePlayer:
                    ChasePlayer();
                    break;
                case EnemyBehavior.ZigZag:
                    ZigZagMovement();
                    break;
                case EnemyBehavior.StraightLine:
                    StraightLineMovement();
                    break;
                case EnemyBehavior.CircleStrafe:
                    CircleStrafeMovement();
                    break;
                case EnemyBehavior.KeepDistance:
                    KeepDistanceMovement();
                    break;
            }
        }

        void ChasePlayer()
        {
            moveDirection = (player.position - transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;
            transform.LookAt(player);
        }

        void ZigZagMovement()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            float zigzag = Mathf.Sin(behaviorTimer * 3f) * 2f;
            Vector3 perpendicular = Vector3.Cross(toPlayer, Vector3.up);
            moveDirection = (toPlayer + perpendicular * zigzag).normalized;
            rb.velocity = moveDirection * moveSpeed;
        }

        void StraightLineMovement()
        {
            // Move in initial direction
            if (moveDirection == Vector3.zero)
            {
                moveDirection = (player.position - transform.position).normalized;
            }
            rb.velocity = moveDirection * moveSpeed;
        }

        void CircleStrafeMovement()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;
            Vector3 perpendicular = Vector3.Cross(toPlayer, Vector3.up);
            moveDirection = (toPlayer * 0.3f + perpendicular).normalized;
            rb.velocity = moveDirection * moveSpeed;
            transform.LookAt(player);
        }

        void KeepDistanceMovement()
        {
            float distance = Vector3.Distance(transform.position, player.position);
            float optimalDistance = 5f;

            if (distance < optimalDistance)
            {
                // Move away
                moveDirection = (transform.position - player.position).normalized;
            }
            else
            {
                // Move closer
                moveDirection = (player.position - transform.position).normalized;
            }

            rb.velocity = moveDirection * moveSpeed;
            transform.LookAt(player);
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;

            // Visual feedback (color flash could be added here)

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            // Drop rewards
            GameManager.Instance.AddGoldThisRun(goldValue);
            GameManager.Instance.AddKill();

            // Add XP to level system
            if (LevelSystem.Instance != null)
            {
                LevelSystem.Instance.AddXP(xpValue);
            }

            // Splitter logic
            if (enemyData.splitCount > 0)
            {
                SpawnSplitEnemies();
            }

            // Particle effect (would be pooled)
            // PoolManager.Instance.SpawnFromPool("Particle_Explosion", transform.position, Quaternion.identity);

            // Return to pool
            ReturnToPool();

            Debug.Log($"{enemyData.enemyName} died! Gold: {goldValue}, XP: {xpValue}");
        }

        void SpawnSplitEnemies()
        {
            for (int i = 0; i < enemyData.splitCount; i++)
            {
                float angle = i * (360f / enemyData.splitCount) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 0.5f;
                Vector3 spawnPos = transform.position + offset;

                // Spawn smaller version (would use a different enemy type)
                // For now, just placeholder
                Debug.Log($"Spawning split enemy at {spawnPos}");
            }
        }

        void GenerateEnemyMesh()
        {
            Mesh mesh = null;

            switch (enemyData.meshType)
            {
                case EnemyMeshType.Cube:
                    mesh = CreateCubeMesh(enemyData.meshScale);
                    break;
                case EnemyMeshType.Sphere:
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    mesh = sphere.GetComponent<MeshFilter>().mesh;
                    Destroy(sphere);
                    break;
                case EnemyMeshType.Pyramid:
                    mesh = ProceduralMeshGenerator.CreatePyramid(enemyData.meshScale);
                    break;
                case EnemyMeshType.Cylinder:
                    GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    mesh = cylinder.GetComponent<MeshFilter>().mesh;
                    Destroy(cylinder);
                    break;
                case EnemyMeshType.Octahedron:
                    mesh = ProceduralMeshGenerator.CreateOctahedron(enemyData.meshScale);
                    break;
                case EnemyMeshType.Icosahedron:
                    mesh = ProceduralMeshGenerator.CreateIcosahedron(enemyData.meshScale);
                    break;
            }

            Material material = ProceduralMeshGenerator.CreateNeonMaterial(enemyData.enemyColor, 2f);
            ProceduralMeshGenerator.ApplyMeshToGameObject(gameObject, mesh, material);
        }

        Mesh CreateCubeMesh(float size)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = cube.GetComponent<MeshFilter>().mesh;
            Destroy(cube);
            return mesh;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }

        void ReturnToPool()
        {
            isInitialized = false;
            PoolManager.Instance.ReturnToPool($"Enemy_{enemyData.enemyType}", gameObject);
        }

        // ==================== STATUS EFFECTS ====================

        public void ApplyFreeze(float duration)
        {
            if (!isFrozen)
            {
                // Save original speed before slowing
                originalMoveSpeed = moveSpeed;
            }

            isFrozen = true;
            freezeTimer = duration;
            moveSpeed = originalMoveSpeed * 0.5f; // Slow down to 50% when frozen
        }

        public void ApplyBurn(float damagePerSecond, float duration)
        {
            isBurning = true;
            burnDamage = damagePerSecond;
            burnTimer = duration;
        }
    }
}
