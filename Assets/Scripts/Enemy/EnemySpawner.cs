using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Data;
using NeonSurvivors.Core;

namespace NeonSurvivors.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Configuration")]
        public List<EnemyData> enemyTypes = new List<EnemyData>();
        public float baseSpawnRate = 2f; // enemies per second
        public float spawnRadius = 12f;

        [Header("Spawn Weights")]
        public float basicWeight = 50f;
        public float fastWeight = 25f;
        public float tankWeight = 15f;
        public float shooterWeight = 10f;

        [Header("Boss Spawning")]
        public bool spawnBosses = true;
        public float bossSpawnInterval = 300f; // Every 5 minutes
        private float bossTimer = 0f;

        [Header("Runtime")]
        private float spawnTimer = 0f;
        private int currentWave = 0;
        private Transform player;

        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Player not found for enemy spawner!");
            }

            // Subscribe to game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnWaveChanged += OnWaveChanged;
                GameManager.Instance.OnGameStart += OnGameStart;
                GameManager.Instance.OnGameEnd += OnGameEnd;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isGameRunning)
                return;

            // Update wave
            currentWave = GameManager.Instance.currentWave;

            // Spawn enemies
            spawnTimer += Time.deltaTime;
            float adjustedSpawnRate = CalculateSpawnRate();

            if (spawnTimer >= 1f / adjustedSpawnRate)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }

            // Boss spawning
            if (spawnBosses)
            {
                bossTimer += Time.deltaTime;
                if (bossTimer >= bossSpawnInterval)
                {
                    SpawnBoss();
                    bossTimer = 0f;
                }
            }
        }

        float CalculateSpawnRate()
        {
            // Increase spawn rate with wave
            return baseSpawnRate * (1f + currentWave * 0.1f);
        }

        void SpawnEnemy()
        {
            if (player == null)
                return;

            // Select enemy type based on weights
            EnemyData enemyData = SelectRandomEnemy();

            if (enemyData == null)
            {
                Debug.LogWarning("No enemy data selected!");
                return;
            }

            // Random position on circle edge around player
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * spawnRadius;
            Vector3 spawnPos = player.position + spawnOffset;

            // Spawn from pool
            GameObject enemyObj = PoolManager.Instance.SpawnFromPool($"Enemy_{enemyData.enemyType}", spawnPos, Quaternion.identity);

            if (enemyObj != null)
            {
                EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.Initialize(enemyData, currentWave);
                }
            }
        }

        EnemyData SelectRandomEnemy()
        {
            if (enemyTypes.Count == 0)
                return null;

            // Weight-based selection
            float totalWeight = basicWeight + fastWeight + tankWeight + shooterWeight;
            float randomValue = Random.Range(0f, totalWeight);

            float cumulativeWeight = 0f;

            // Basic
            cumulativeWeight += basicWeight;
            if (randomValue < cumulativeWeight && enemyTypes.Count > 0)
                return enemyTypes[0];

            // Fast
            cumulativeWeight += fastWeight;
            if (randomValue < cumulativeWeight && enemyTypes.Count > 1)
                return enemyTypes[1];

            // Tank
            cumulativeWeight += tankWeight;
            if (randomValue < cumulativeWeight && enemyTypes.Count > 2)
                return enemyTypes[2];

            // Shooter
            if (enemyTypes.Count > 3)
                return enemyTypes[3];

            // Fallback
            return enemyTypes[0];
        }

        void SpawnBoss()
        {
            if (player == null)
                return;

            // Find boss enemy data
            EnemyData bossData = enemyTypes.Find(e => e.isBoss);

            if (bossData == null)
            {
                Debug.LogWarning("No boss enemy data found!");
                return;
            }

            // Spawn in front of player
            Vector3 spawnPos = player.position + player.forward * spawnRadius;

            GameObject bossObj = PoolManager.Instance.SpawnFromPool($"Enemy_{bossData.enemyType}", spawnPos, Quaternion.identity);

            if (bossObj != null)
            {
                EnemyBase boss = bossObj.GetComponent<EnemyBase>();
                if (boss != null)
                {
                    boss.Initialize(bossData, currentWave);
                }
            }

            Debug.Log("Boss spawned!");
        }

        void OnWaveChanged(int wave)
        {
            Debug.Log($"Wave {wave} - Spawn rate increased");
        }

        void OnGameStart()
        {
            spawnTimer = 0f;
            bossTimer = 0f;
            currentWave = 0;
        }

        void OnGameEnd()
        {
            // Stop spawning
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnWaveChanged -= OnWaveChanged;
                GameManager.Instance.OnGameStart -= OnGameStart;
                GameManager.Instance.OnGameEnd -= OnGameEnd;
            }
        }
    }
}
