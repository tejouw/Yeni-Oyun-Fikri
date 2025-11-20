using UnityEngine;
using NeonSurvivors.Player;
using NeonSurvivors.Enemy;
using NeonSurvivors.Monetization;
using NeonSurvivors.Utilities;
using NeonSurvivors.Data;

namespace NeonSurvivors.Core
{
    /// <summary>
    /// Game Setup - Initializes the game on startup
    /// Attach this to a GameObject in the first scene
    /// </summary>
    public class GameSetup : MonoBehaviour
    {
        [Header("Create Managers")]
        public bool autoCreateManagers = true;

        [Header("Player Setup")]
        public GameObject playerPrefab;
        public Vector3 playerSpawnPosition = Vector3.zero;

        [Header("References")]
        private GameObject player;

        void Awake()
        {
            // Set target frame rate for mobile
            Application.targetFrameRate = 60;

            // Create core managers if they don't exist
            if (autoCreateManagers)
            {
                CreateManagers();
            }
        }

        void Start()
        {
            // Create player
            CreatePlayer();

            // Setup camera
            SetupCamera();

            // Create pool prefabs
            CreatePooledPrefabs();
        }

        void CreateManagers()
        {
            // GameManager
            if (GameManager.Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.AddComponent<GameManager>();
                Debug.Log("Created GameManager");
            }

            // PoolManager
            if (PoolManager.Instance == null)
            {
                GameObject pmObj = new GameObject("PoolManager");
                pmObj.AddComponent<PoolManager>();
                Debug.Log("Created PoolManager");
            }

            // SaveManager
            if (SaveManager.Instance == null)
            {
                GameObject smObj = new GameObject("SaveManager");
                smObj.AddComponent<SaveManager>();
                Debug.Log("Created SaveManager");
            }

            // StatsManager
            if (StatsManager.Instance == null)
            {
                GameObject statsObj = new GameObject("StatsManager");
                statsObj.AddComponent<StatsManager>();
                Debug.Log("Created StatsManager");
            }

            // LevelSystem
            if (LevelSystem.Instance == null)
            {
                GameObject lsObj = new GameObject("LevelSystem");
                lsObj.AddComponent<LevelSystem>();
                Debug.Log("Created LevelSystem");
            }

            // ProgressionManager
            if (ProgressionManager.Instance == null)
            {
                GameObject progObj = new GameObject("ProgressionManager");
                progObj.AddComponent<ProgressionManager>();
                Debug.Log("Created ProgressionManager");
            }

            // WorkshopManager
            if (WorkshopManager.Instance == null)
            {
                GameObject wsmObj = new GameObject("WorkshopManager");
                wsmObj.AddComponent<WorkshopManager>();
                Debug.Log("Created WorkshopManager");
            }

            // IdleManager
            if (IdleManager.Instance == null)
            {
                GameObject imObj = new GameObject("IdleManager");
                imObj.AddComponent<IdleManager>();
                Debug.Log("Created IdleManager");
            }

            // ShipManager
            if (ShipManager.Instance == null)
            {
                GameObject shipObj = new GameObject("ShipManager");
                shipObj.AddComponent<ShipManager>();
                Debug.Log("Created ShipManager");
            }

            // UIManager
            if (UIManager.Instance == null)
            {
                GameObject uiObj = new GameObject("UIManager");
                uiObj.AddComponent<UIManager>();
                Debug.Log("Created UIManager");
            }

            // AdManager
            if (AdManager.Instance == null)
            {
                GameObject adObj = new GameObject("AdManager");
                adObj.AddComponent<AdManager>();
                Debug.Log("Created AdManager");
            }

            // IAPManager
            if (IAPManager.Instance == null)
            {
                GameObject iapObj = new GameObject("IAPManager");
                iapObj.AddComponent<IAPManager>();
                Debug.Log("Created IAPManager");
            }
        }

        void CreatePlayer()
        {
            // Create player GameObject
            player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = playerSpawnPosition;
            player.layer = LayerMask.NameToLayer("Default");

            // Add components
            player.AddComponent<Rigidbody>();
            player.AddComponent<SphereCollider>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerHealth>();
            player.AddComponent<PlayerWeapon>();
            player.AddComponent<PlayerUpgrades>();

            // Get current ship data
            ShipData currentShip = ShipManager.Instance?.GetCurrentShip();
            if (currentShip == null)
            {
                Debug.LogWarning("No ship selected, creating default ship");
                currentShip = CreateDefaultShip();
            }

            // Set ship data
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.currentShip = currentShip;

            Debug.Log($"Player created with ship: {currentShip.shipName}");
        }

        ShipData CreateDefaultShip()
        {
            ShipData defaultShip = ScriptableObject.CreateInstance<ShipData>();
            defaultShip.shipID = "ship_starter";
            defaultShip.shipName = "Starter Ship";
            defaultShip.maxHealth = 100f;
            defaultShip.moveSpeed = 5f;
            defaultShip.baseDamage = 10f;
            defaultShip.fireRate = 1f;
            defaultShip.meshType = ShipMeshType.Triangle;
            defaultShip.primaryColor = Color.cyan;
            defaultShip.emissionColor = Color.cyan;
            defaultShip.meshScale = 1f;
            defaultShip.abilityType = ShipAbilityType.None;
            defaultShip.unlockType = UnlockType.Default;

            return defaultShip;
        }

        void SetupCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }

            // Top-down view
            mainCam.transform.position = new Vector3(0, 15, 0);
            mainCam.transform.rotation = Quaternion.Euler(90, 0, 0);
            mainCam.orthographic = true;
            mainCam.orthographicSize = 10f;
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.05f, 0f, 0.1f); // Dark purple

            Debug.Log("Camera setup complete");
        }

        void CreatePooledPrefabs()
        {
            // Create enemy spawner
            GameObject spawnerObj = new GameObject("EnemySpawner");
            EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();

            // Load enemy data
            EnemyData[] enemyTypes = Resources.LoadAll<EnemyData>("Data/Enemies");
            if (enemyTypes.Length > 0)
            {
                spawner.enemyTypes.AddRange(enemyTypes);
            }
            else
            {
                Debug.LogWarning("No enemy data found, creating defaults");
                CreateDefaultEnemyData(spawner);
            }

            // Create pools
            SetupObjectPools();

            Debug.Log("Pooled prefabs created");
        }

        void CreateDefaultEnemyData(EnemySpawner spawner)
        {
            // Basic enemy
            EnemyData basicEnemy = ScriptableObject.CreateInstance<EnemyData>();
            basicEnemy.enemyID = "enemy_basic";
            basicEnemy.enemyName = "Basic Enemy";
            basicEnemy.enemyType = EnemyType.Basic;
            basicEnemy.baseHealth = 10f;
            basicEnemy.moveSpeed = 3f;
            basicEnemy.damage = 10f;
            basicEnemy.goldValue = 10;
            basicEnemy.xpValue = 10;
            basicEnemy.meshType = EnemyMeshType.Cube;
            basicEnemy.enemyColor = Color.red;
            basicEnemy.meshScale = 0.5f;
            basicEnemy.behavior = EnemyBehavior.ChasePlayer;

            spawner.enemyTypes.Add(basicEnemy);
        }

        void SetupObjectPools()
        {
            PoolManager poolManager = PoolManager.Instance;
            if (poolManager == null)
                return;

            // Create prefabs and add to pool configuration
            poolManager.pools.Add(CreateEnemyPool("Enemy_Basic", Color.red, EnemyMeshType.Cube));
            poolManager.pools.Add(CreateEnemyPool("Enemy_Fast", Color.yellow, EnemyMeshType.Pyramid));
            poolManager.pools.Add(CreateEnemyPool("Enemy_Tank", Color.blue, EnemyMeshType.Sphere));

            poolManager.pools.Add(CreateProjectilePool("Projectile_Player", Color.cyan));

            Debug.Log($"Setup {poolManager.pools.Count} object pools");
        }

        PoolManager.Pool CreateEnemyPool(string tag, Color color, EnemyMeshType meshType)
        {
            GameObject enemyPrefab = new GameObject($"{tag}_Prefab");
            enemyPrefab.tag = "Enemy";
            enemyPrefab.AddComponent<Rigidbody>();
            enemyPrefab.AddComponent<SphereCollider>();
            enemyPrefab.AddComponent<EnemyBase>();

            // Don't activate yet
            enemyPrefab.SetActive(false);

            return new PoolManager.Pool
            {
                tag = tag,
                prefab = enemyPrefab,
                size = 100
            };
        }

        PoolManager.Pool CreateProjectilePool(string tag, Color color)
        {
            GameObject projectilePrefab = new GameObject($"{tag}_Prefab");
            projectilePrefab.tag = "Projectile";

            Rigidbody rb = projectilePrefab.AddComponent<Rigidbody>();
            rb.useGravity = false;

            SphereCollider col = projectilePrefab.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.2f;

            projectilePrefab.AddComponent<Weapons.Projectile>();

            // Add visual (small sphere)
            Mesh sphereMesh = ProceduralMeshGenerator.CreateRegularPolygon(8, 0.15f);
            Material material = ProceduralMeshGenerator.CreateNeonMaterial(color, 3f);
            ProceduralMeshGenerator.ApplyMeshToGameObject(projectilePrefab, sphereMesh, material);

            projectilePrefab.SetActive(false);

            return new PoolManager.Pool
            {
                tag = tag,
                prefab = projectilePrefab,
                size = 500
            };
        }

        public void StartGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }
    }
}
