# NEON SURVIVORS - Teknik İmplementasyon Detayları

## 🏗️ ARCHITECTURE OVERVIEW

### Design Patterns
- **Singleton:** GameManager, PoolManager, SaveManager
- **Object Pool:** Enemies, projectiles, particles
- **Observer:** Event system for decoupling
- **Strategy:** Different enemy behaviors
- **ScriptableObject:** Data-driven design

---

## 📦 CORE SYSTEMS

### 1. OBJECT POOLING SYSTEM

**PoolManager.cs**
```csharp
public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance => _instance;

    // Pool dictionaries
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> prefabDictionary;

    // Pool configurations
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    void Awake()
    {
        _instance = this;

        // Initialize pools
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            prefabDictionary.Add(pool.tag, pool.prefab);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
```

**Pool Sizes:**
- Enemy_Basic: 300
- Enemy_Fast: 200
- Enemy_Tank: 100
- Enemy_Shooter: 150
- Projectile_Player: 1000
- Projectile_Enemy: 500
- Particle_Explosion: 200
- Particle_Trail: 300

---

### 2. PROCEDURAL MESH GENERATION

**ProceduralMeshGenerator.cs**
```csharp
public static class ProceduralMeshGenerator
{
    public static Mesh CreateCube(float size)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[24];
        // Front face
        vertices[0] = new Vector3(-size, -size, size);
        vertices[1] = new Vector3(size, -size, size);
        vertices[2] = new Vector3(size, size, size);
        vertices[3] = new Vector3(-size, size, size);
        // ... (back, top, bottom, left, right faces)

        int[] triangles = new int[36];
        // Define triangle indices

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public static Mesh CreatePyramid(float size)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[5];
        vertices[0] = new Vector3(0, size, 0); // top
        vertices[1] = new Vector3(-size, -size, size); // base corners
        vertices[2] = new Vector3(size, -size, size);
        vertices[3] = new Vector3(size, -size, -size);
        vertices[4] = new Vector3(-size, -size, -size);

        int[] triangles = new int[18];
        // Define faces

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh CreateOctahedron(float size)
    {
        // Double pyramid mesh
    }

    public static Mesh CreateIcosahedron(float size)
    {
        // 20-sided polyhedron for bosses
    }

    // Helper: Apply neon material
    public static Material CreateNeonMaterial(Color emissionColor)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissionColor * 2f);
        mat.SetColor("_BaseColor", emissionColor);
        return mat;
    }
}
```

**Shapes to Generate:**
- Cube (basic enemy)
- Pyramid (fast enemy)
- Octahedron (splitter enemy)
- Cylinder (shooter enemy)
- Sphere (tank enemy) - Unity primitive
- Icosahedron (boss enemy)
- Player ships (variations)

---

### 3. SCRIPTABLEOBJECT DATA ARCHITECTURE

**ShipData.cs**
```csharp
[CreateAssetMenu(fileName = "New Ship", menuName = "Game/Ship Data")]
public class ShipData : ScriptableObject
{
    public string shipName;
    public string shipID;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float baseDamage = 10f;
    public float fireRate = 1f;

    [Header("Visual")]
    public MeshType meshType;
    public Color primaryColor;
    public Color emissionColor;

    [Header("Ability")]
    public ShipAbilityType abilityType;
    public float abilityValue;
    public string abilityDescription;

    [Header("Unlock")]
    public UnlockConditionType unlockType;
    public int unlockValue;
    public string unlockDescription;

    [Header("Monetization")]
    public bool isPremium = false;
    public float priceUSD = 0.99f;
}

public enum MeshType
{
    Triangle,
    Pentagon,
    Arrow,
    Diamond,
    MultiTriangle
}

public enum ShipAbilityType
{
    GoldBonus,
    DamageReduction,
    DodgeChance,
    PierceBonus,
    ExtraProjectiles
}

public enum UnlockConditionType
{
    Default, // starter ships
    ReachWave,
    KillEnemies,
    SurviveTime,
    PrestigeCount,
    SpendGold,
    Premium // IAP
}
```

**WeaponData.cs**
```csharp
[CreateAssetMenu(fileName = "New Weapon", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType type;

    public float baseDamage;
    public float fireRate;
    public float projectileSpeed;
    public int projectileCount;

    public ProjectileShape projectileShape;
    public Color projectileColor;

    public AudioClip fireSound;
}

public enum WeaponType
{
    Blaster,
    Laser,
    Missiles,
    Shotgun,
    Orbit
}
```

**UpgradeData.cs**
```csharp
[CreateAssetMenu(fileName = "New Upgrade", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public UpgradeCategory category;

    public Sprite icon; // runtime generated

    // Stat modifiers
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public int projectileCountBonus = 0;
    public int pierceBonus = 0;
    public int bounceBonus = 0;
    public float explosionRadius = 0f;

    // Special effects
    public bool hasChainLightning = false;
    public bool hasFreeze = false;
    public bool hasBurn = false;

    // Rarity
    public UpgradeRarity rarity;
}

public enum UpgradeCategory
{
    Damage,
    FireRate,
    Projectiles,
    Special
}

public enum UpgradeRarity
{
    Common,    // 60%
    Uncommon,  // 25%
    Rare,      // 12%
    Epic       // 3%
}
```

**EnemyData.cs**
```csharp
[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public EnemyType type;

    public float baseHealth;
    public float moveSpeed;
    public float damage;
    public int goldValue;
    public int xpValue;

    public MeshType meshType;
    public Color enemyColor;

    public EnemyBehavior behavior;
}

public enum EnemyType
{
    Basic,
    Fast,
    Tank,
    Shooter,
    Splitter,
    Boss
}

public enum EnemyBehavior
{
    ChasePlayer,
    ZigZag,
    StraightLine,
    RangedAttack,
    CircleStrafe
}
```

---

### 4. PLAYER SYSTEM

**PlayerController.cs**
```csharp
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    private PlayerHealth health;
    private PlayerWeapon weapon;
    private PlayerUpgrades upgrades;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Current Ship")]
    public ShipData currentShip;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
        weapon = GetComponent<PlayerWeapon>();
        upgrades = GetComponent<PlayerUpgrades>();

        InitializeFromShipData();
    }

    void Update()
    {
        HandleInput();
        RotateTowardsMovement();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleInput()
    {
        // Touch/joystick input
        #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            moveDirection = (touchPos - transform.position).normalized;
        }
        #else
        // Desktop testing
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(h, 0, v).normalized;
        #endif
    }

    void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        rb.velocity = velocity;

        // Clamp to screen bounds
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -10f, 10f);
        pos.z = Mathf.Clamp(pos.z, -10f, 10f);
        transform.position = pos;
    }

    void RotateTowardsMovement()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void InitializeFromShipData()
    {
        health.maxHealth = currentShip.maxHealth;
        health.currentHealth = currentShip.maxHealth;
        moveSpeed = currentShip.moveSpeed;
        weapon.baseDamage = currentShip.baseDamage;
        weapon.fireRate = currentShip.fireRate;

        // Generate visual mesh
        GenerateShipMesh();
    }

    void GenerateShipMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        switch (currentShip.meshType)
        {
            case MeshType.Triangle:
                mf.mesh = ProceduralMeshGenerator.CreatePyramid(0.5f);
                break;
            case MeshType.Pentagon:
                mf.mesh = ProceduralMeshGenerator.CreatePentagon(0.5f);
                break;
            // ... other types
        }

        mr.material = ProceduralMeshGenerator.CreateNeonMaterial(currentShip.emissionColor);
    }
}
```

**PlayerWeapon.cs**
```csharp
public class PlayerWeapon : MonoBehaviour
{
    public WeaponData currentWeapon;

    public float baseDamage = 10f;
    public float fireRate = 1f;
    public int projectileCount = 1;
    public int pierceCount = 0;
    public int bounceCount = 0;

    private float fireTimer = 0f;
    private Transform nearestEnemy;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= 1f / fireRate)
        {
            FindNearestEnemy();
            if (nearestEnemy != null)
            {
                FireWeapon();
                fireTimer = 0f;
            }
        }
    }

    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
    }

    void FireWeapon()
    {
        Vector3 direction = (nearestEnemy.position - transform.position).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (i - projectileCount / 2f) * 10f;
            Vector3 spreadDirection = Quaternion.Euler(0, angle, 0) * direction;

            GameObject projectile = PoolManager.Instance.SpawnFromPool("Projectile_Player", transform.position, Quaternion.identity);

            Projectile proj = projectile.GetComponent<Projectile>();
            proj.Initialize(baseDamage, spreadDirection, pierceCount, bounceCount);
        }
    }
}
```

---

### 5. ENEMY SYSTEM

**EnemySpawner.cs**
```csharp
public class EnemySpawner : MonoBehaviour
{
    public List<EnemyData> enemyTypes;

    public float spawnRate = 2f; // enemies per second
    public float spawnRadius = 12f;

    private float spawnTimer = 0f;
    private int currentWave = 0;
    private float waveTimer = 0f;

    void Update()
    {
        waveTimer += Time.deltaTime;
        currentWave = Mathf.FloorToInt(waveTimer / 60f); // wave every minute

        spawnTimer += Time.deltaTime;
        float adjustedSpawnRate = spawnRate * (1 + currentWave * 0.1f);

        if (spawnTimer >= 1f / adjustedSpawnRate)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // Random position on circle edge
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = new Vector3(randomCircle.x, 0, randomCircle.y);

        // Select enemy type (weighted random)
        EnemyData enemyData = SelectRandomEnemy();

        GameObject enemy = PoolManager.Instance.SpawnFromPool($"Enemy_{enemyData.type}", spawnPos, Quaternion.identity);

        EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
        enemyScript.Initialize(enemyData, currentWave);
    }

    EnemyData SelectRandomEnemy()
    {
        // Weighted selection based on wave
        float roll = Random.Range(0f, 1f);

        if (roll < 0.5f) return enemyTypes[0]; // Basic (50%)
        else if (roll < 0.75f) return enemyTypes[1]; // Fast (25%)
        else if (roll < 0.90f) return enemyTypes[2]; // Tank (15%)
        else return enemyTypes[3]; // Shooter (10%)
    }
}
```

**EnemyBase.cs**
```csharp
public class EnemyBase : MonoBehaviour
{
    public EnemyData data;

    public float currentHealth;
    public float maxHealth;
    public float moveSpeed;
    public int goldValue;
    public int xpValue;

    private Transform player;
    private Rigidbody rb;

    public void Initialize(EnemyData enemyData, int waveNumber)
    {
        data = enemyData;

        // Scale stats with wave
        maxHealth = enemyData.baseHealth * (1 + waveNumber * 0.15f);
        currentHealth = maxHealth;
        moveSpeed = enemyData.moveSpeed * (1 + waveNumber * 0.05f);
        goldValue = enemyData.goldValue * (1 + waveNumber);
        xpValue = enemyData.xpValue;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        GenerateEnemyMesh();
    }

    void Update()
    {
        MoveBehavior();
    }

    void MoveBehavior()
    {
        switch (data.behavior)
        {
            case EnemyBehavior.ChasePlayer:
                ChasePlayer();
                break;
            case EnemyBehavior.ZigZag:
                ZigZagMovement();
                break;
            // ... other behaviors
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        transform.LookAt(player);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Drop gold/xp
        GameManager.Instance.AddGold(goldValue);
        GameManager.Instance.AddXP(xpValue);

        // Particle effect
        PoolManager.Instance.SpawnFromPool("Particle_Explosion", transform.position, Quaternion.identity);

        // Return to pool
        gameObject.SetActive(false);
    }

    void GenerateEnemyMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        switch (data.meshType)
        {
            case MeshType.Cube:
                mf.mesh = ProceduralMeshGenerator.CreateCube(0.5f);
                break;
            case MeshType.Pyramid:
                mf.mesh = ProceduralMeshGenerator.CreatePyramid(0.5f);
                break;
            // ... others
        }

        mr.material = ProceduralMeshGenerator.CreateNeonMaterial(data.enemyColor);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(data.damage);
        }
    }
}
```

---

### 6. PROGRESSION SYSTEM

**LevelSystem.cs**
```csharp
public class LevelSystem : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 1000;

    public List<UpgradeData> allUpgrades;

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        currentLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(1000 * Mathf.Pow(1.1f, currentLevel));

        // Show level-up screen
        ShowUpgradeChoices();
    }

    void ShowUpgradeChoices()
    {
        List<UpgradeData> choices = SelectThreeRandomUpgrades();
        UIManager.Instance.ShowLevelUpScreen(choices);
        Time.timeScale = 0f; // pause game
    }

    List<UpgradeData> SelectThreeRandomUpgrades()
    {
        List<UpgradeData> selected = new List<UpgradeData>();
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);

        for (int i = 0; i < 3; i++)
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
        if (roll < 0.60f) targetRarity = UpgradeRarity.Common;
        else if (roll < 0.85f) targetRarity = UpgradeRarity.Uncommon;
        else if (roll < 0.97f) targetRarity = UpgradeRarity.Rare;
        else targetRarity = UpgradeRarity.Epic;

        List<UpgradeData> rarityPool = pool.FindAll(u => u.rarity == targetRarity);

        if (rarityPool.Count == 0)
            return pool[Random.Range(0, pool.Count)];

        return rarityPool[Random.Range(0, rarityPool.Count)];
    }
}
```

**PrestigeManager.cs**
```csharp
public class PrestigeManager : MonoBehaviour
{
    public int prestigePoints = 0;
    public long totalLifetimeGold = 0;

    public void Prestige()
    {
        // Calculate prestige points earned
        int ppEarned = CalculatePrestigePoints();
        prestigePoints += ppEarned;

        // Reset run progress
        ResetRunProgress();

        // Show prestige rewards
        UIManager.Instance.ShowPrestigeRewards(ppEarned);

        SaveManager.Instance.SaveGame();
    }

    int CalculatePrestigePoints()
    {
        // Formula: PP = 150 × √(totalGold / 10^6)
        float pp = 150f * Mathf.Sqrt(totalLifetimeGold / 1000000f);
        return Mathf.FloorToInt(pp);
    }

    void ResetRunProgress()
    {
        // Reset in-run stats
        LevelSystem levelSys = GetComponent<LevelSystem>();
        levelSys.currentLevel = 1;
        levelSys.currentXP = 0;

        PlayerUpgrades upgrades = FindObjectOfType<PlayerUpgrades>();
        upgrades.ResetUpgrades();

        // Keep persistent: prestigePoints, totalLifetimeGold, workshop upgrades, ship unlocks
    }

    public float GetPrestigeDamageMultiplier()
    {
        return 1f + (prestigePoints * 0.05f); // +5% per PP
    }

    public float GetPrestigeGoldMultiplier()
    {
        return 1f + (prestigePoints * 0.03f); // +3% per PP
    }
}
```

**IdleManager.cs**
```csharp
public class IdleManager : MonoBehaviour
{
    private float baseGoldPerSecond = 10f;
    private const float MAX_OFFLINE_HOURS = 4f;

    public void CalculateOfflineEarnings()
    {
        DateTime lastSaveTime = SaveManager.Instance.GetLastSaveTime();
        TimeSpan timeOffline = DateTime.Now - lastSaveTime;

        float hoursOffline = (float)timeOffline.TotalHours;
        hoursOffline = Mathf.Min(hoursOffline, MAX_OFFLINE_HOURS); // cap at 4 hours

        float prestigeMultiplier = GetComponent<PrestigeManager>().GetPrestigeGoldMultiplier();
        int offlineGold = Mathf.RoundToInt(baseGoldPerSecond * prestigeMultiplier * hoursOffline * 3600f * 0.5f);

        if (offlineGold > 0)
        {
            UIManager.Instance.ShowOfflineEarnings(offlineGold, hoursOffline);
        }
    }

    public void ClaimOfflineEarnings(int baseAmount, bool watchedAd)
    {
        int finalAmount = watchedAd ? baseAmount * 3 : baseAmount;
        GameManager.Instance.AddGold(finalAmount);
    }
}
```

---

### 7. SAVE SYSTEM

**SaveManager.cs**
```csharp
using ES3;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;
    public static SaveManager Instance => _instance;

    private const string SAVE_KEY = "GameData";
    private const float AUTO_SAVE_INTERVAL = 30f;
    private float autoSaveTimer = 0f;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
        {
            SaveGame();
            autoSaveTimer = 0f;
        }
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        // Collect data from managers
        data.prestigePoints = FindObjectOfType<PrestigeManager>().prestigePoints;
        data.totalLifetimeGold = FindObjectOfType<PrestigeManager>().totalLifetimeGold;
        data.currentGold = GameManager.Instance.currentGold;
        data.unlockedShips = ShipManager.Instance.GetUnlockedShipIDs();
        data.workshopLevels = WorkshopManager.Instance.GetUpgradeLevels();
        data.lastSaveTime = DateTime.Now.ToString();

        ES3.Save(SAVE_KEY, data);
    }

    public void LoadGame()
    {
        if (!ES3.KeyExists(SAVE_KEY))
        {
            // First time player
            InitializeNewGame();
            return;
        }

        GameData data = ES3.Load<GameData>(SAVE_KEY);

        // Apply loaded data
        FindObjectOfType<PrestigeManager>().prestigePoints = data.prestigePoints;
        FindObjectOfType<PrestigeManager>().totalLifetimeGold = data.totalLifetimeGold;
        GameManager.Instance.currentGold = data.currentGold;
        ShipManager.Instance.UnlockShips(data.unlockedShips);
        WorkshopManager.Instance.SetUpgradeLevels(data.workshopLevels);

        // Calculate offline earnings
        FindObjectOfType<IdleManager>().CalculateOfflineEarnings();
    }

    public DateTime GetLastSaveTime()
    {
        if (!ES3.KeyExists(SAVE_KEY))
            return DateTime.Now;

        GameData data = ES3.Load<GameData>(SAVE_KEY);
        return DateTime.Parse(data.lastSaveTime);
    }

    void InitializeNewGame()
    {
        // Set default values for new players
        GameManager.Instance.currentGold = 0;
        ShipManager.Instance.UnlockShip("ship_starter");
    }
}

[System.Serializable]
public class GameData
{
    public int prestigePoints;
    public long totalLifetimeGold;
    public int currentGold;
    public List<string> unlockedShips;
    public Dictionary<string, int> workshopLevels;
    public string lastSaveTime;
}
```

---

### 8. UI SYSTEM (UI Toolkit)

**UIManager.cs**
```csharp
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    private UIDocument document;
    private VisualElement root;

    // Screen references
    private VisualElement hudScreen;
    private VisualElement levelUpScreen;
    private VisualElement deathScreen;
    private VisualElement menuScreen;

    void Awake()
    {
        _instance = this;
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        InitializeScreens();
    }

    void InitializeScreens()
    {
        // Create HUD
        hudScreen = CreateHUD();
        root.Add(hudScreen);

        // Create level-up screen (hidden by default)
        levelUpScreen = CreateLevelUpScreen();
        levelUpScreen.style.display = DisplayStyle.None;
        root.Add(levelUpScreen);

        // Other screens...
    }

    VisualElement CreateHUD()
    {
        VisualElement hud = new VisualElement();
        hud.style.width = Length.Percent(100);
        hud.style.height = Length.Percent(100);

        // HP Bar
        VisualElement hpBar = new VisualElement();
        hpBar.style.position = Position.Absolute;
        hpBar.style.top = 20;
        hpBar.style.left = 20;
        hpBar.style.width = 200;
        hpBar.style.height = 20;
        hpBar.style.backgroundColor = new Color(1, 0, 0, 0.5f);
        hud.Add(hpBar);

        // XP Bar
        VisualElement xpBar = new VisualElement();
        xpBar.style.position = Position.Absolute;
        xpBar.style.bottom = 20;
        xpBar.style.left = Length.Percent(10);
        xpBar.style.width = Length.Percent(80);
        xpBar.style.height = 15;
        xpBar.style.backgroundColor = new Color(0, 1, 1, 0.5f);
        hud.Add(xpBar);

        // Gold counter
        Label goldLabel = new Label("Gold: 0");
        goldLabel.style.position = Position.Absolute;
        goldLabel.style.top = 20;
        goldLabel.style.right = 20;
        goldLabel.style.fontSize = 24;
        goldLabel.style.color = Color.yellow;
        hud.Add(goldLabel);

        return hud;
    }

    VisualElement CreateLevelUpScreen()
    {
        VisualElement screen = new VisualElement();
        screen.style.width = Length.Percent(100);
        screen.style.height = Length.Percent(100);
        screen.style.backgroundColor = new Color(0, 0, 0, 0.8f);
        screen.style.alignItems = Align.Center;
        screen.style.justifyContent = Justify.Center;

        // Title
        Label title = new Label("LEVEL UP!");
        title.style.fontSize = 48;
        title.style.color = Color.cyan;
        screen.Add(title);

        // Upgrade cards container
        VisualElement cardsContainer = new VisualElement();
        cardsContainer.style.flexDirection = FlexDirection.Row;
        cardsContainer.style.marginTop = 40;
        screen.Add(cardsContainer);

        return screen;
    }

    public void ShowLevelUpScreen(List<UpgradeData> upgrades)
    {
        levelUpScreen.style.display = DisplayStyle.Flex;

        VisualElement cardsContainer = levelUpScreen.Q<VisualElement>("CardsContainer");
        cardsContainer.Clear();

        foreach (UpgradeData upgrade in upgrades)
        {
            VisualElement card = CreateUpgradeCard(upgrade);
            cardsContainer.Add(card);
        }
    }

    VisualElement CreateUpgradeCard(UpgradeData upgrade)
    {
        Button card = new Button();
        card.style.width = 250;
        card.style.height = 350;
        card.style.marginLeft = 20;
        card.style.marginRight = 20;
        card.style.backgroundColor = new Color(0.1f, 0.1f, 0.3f);

        // Icon
        VisualElement icon = new VisualElement();
        icon.style.width = 100;
        icon.style.height = 100;
        icon.style.backgroundColor = upgrade.rarity == UpgradeRarity.Epic ? Color.magenta : Color.cyan;
        card.Add(icon);

        // Name
        Label name = new Label(upgrade.upgradeName);
        name.style.fontSize = 24;
        name.style.color = Color.white;
        card.Add(name);

        // Description
        Label desc = new Label(upgrade.description);
        desc.style.fontSize = 16;
        desc.style.color = Color.gray;
        desc.style.whiteSpace = WhiteSpace.Normal;
        card.Add(desc);

        // Click handler
        card.clicked += () => OnUpgradeSelected(upgrade);

        return card;
    }

    void OnUpgradeSelected(UpgradeData upgrade)
    {
        FindObjectOfType<PlayerUpgrades>().ApplyUpgrade(upgrade);
        levelUpScreen.style.display = DisplayStyle.None;
        Time.timeScale = 1f; // resume game
    }

    public void UpdateHPBar(float current, float max)
    {
        // Update HP bar width
    }

    public void UpdateXPBar(int current, int max)
    {
        // Update XP bar width
    }

    public void UpdateGoldCounter(int amount)
    {
        Label goldLabel = hudScreen.Q<Label>("GoldLabel");
        goldLabel.text = $"Gold: {amount}";
    }
}
```

---

### 9. MONETIZATION SYSTEM

**AdManager.cs**
```csharp
// Integration with Mobile Monetization Pro
public class AdManager : MonoBehaviour
{
    private static AdManager _instance;
    public static AdManager Instance => _instance;

    private bool isInitialized = false;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAds();
    }

    void InitializeAds()
    {
        // Mobile Monetization Pro initialization
        // AdMob, IronSource mediation setup
        isInitialized = true;
    }

    public void ShowRewardedAd(System.Action<bool> callback, string placement)
    {
        if (!isInitialized)
        {
            callback?.Invoke(false);
            return;
        }

        // Track placement for analytics
        AnalyticsManager.Instance.TrackAdRequest(placement);

        // Show rewarded video
        // MobileMonetizationPro.ShowRewardedAd(...)

        // On completion
        callback?.Invoke(true);
        AnalyticsManager.Instance.TrackAdCompleted(placement);
    }

    public void ShowInterstitial(string placement)
    {
        if (!isInitialized) return;

        // Interstitial logic (max 1 per 3 minutes)
        // MobileMonetizationPro.ShowInterstitial(...)
    }
}
```

**IAPManager.cs**
```csharp
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IAPManager _instance;
    public static IAPManager Instance => _instance;

    private IStoreController storeController;

    // Product IDs
    private const string SHIP_PREMIUM_1 = "ship_tank";
    private const string SHIP_PREMIUM_2 = "ship_laser";
    private const string GOLD_SMALL = "gold_10k";
    private const string AD_REMOVAL = "remove_ads";
    private const string BATTLE_PASS = "battle_pass";

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePurchasing();
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add products
        builder.AddProduct(SHIP_PREMIUM_1, ProductType.NonConsumable);
        builder.AddProduct(SHIP_PREMIUM_2, ProductType.NonConsumable);
        builder.AddProduct(GOLD_SMALL, ProductType.Consumable);
        builder.AddProduct(AD_REMOVAL, ProductType.NonConsumable);
        builder.AddProduct(BATTLE_PASS, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void PurchaseShip(string shipID)
    {
        if (storeController != null)
        {
            storeController.InitiatePurchase(shipID);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // Handle successful purchase
        string productID = args.purchasedProduct.definition.id;

        if (productID == SHIP_PREMIUM_1)
        {
            ShipManager.Instance.UnlockShip(productID);
        }
        else if (productID == GOLD_SMALL)
        {
            GameManager.Instance.AddGold(10000);
        }
        // ... other products

        AnalyticsManager.Instance.TrackPurchase(productID, args.purchasedProduct.metadata.localizedPrice);

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP initialization failed: {error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, reason: {failureReason}");
    }
}
```

---

## 📊 ANALYTICS & METRICS

**AnalyticsManager.cs**
```csharp
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager _instance;
    public static AnalyticsManager Instance => _instance;

    void Awake()
    {
        _instance = this;
    }

    public void TrackGameStart()
    {
        AnalyticsService.Instance.CustomData("game_start", new Dictionary<string, object>
        {
            { "timestamp", DateTime.Now.ToString() }
        });
    }

    public void TrackGameEnd(int wave, int kills, int gold, float survivalTime)
    {
        AnalyticsService.Instance.CustomData("game_end", new Dictionary<string, object>
        {
            { "wave_reached", wave },
            { "total_kills", kills },
            { "gold_earned", gold },
            { "survival_time", survivalTime }
        });
    }

    public void TrackLevelUp(int level)
    {
        AnalyticsService.Instance.CustomData("level_up", new Dictionary<string, object>
        {
            { "level", level }
        });
    }

    public void TrackAdRequest(string placement)
    {
        AnalyticsService.Instance.CustomData("ad_request", new Dictionary<string, object>
        {
            { "placement", placement }
        });
    }

    public void TrackAdCompleted(string placement)
    {
        AnalyticsService.Instance.CustomData("ad_completed", new Dictionary<string, object>
        {
            { "placement", placement }
        });
    }

    public void TrackPurchase(string productID, decimal price)
    {
        AnalyticsService.Instance.CustomData("iap_purchase", new Dictionary<string, object>
        {
            { "product_id", productID },
            { "price", price }
        });
    }
}
```

---

## 🎮 PERFORMANCE OPTIMIZATION

### Object Pooling Best Practices
- Pre-warm pools on game start (3-5 second loading)
- Never destroy pooled objects
- Disable instead of destroy
- Reuse particles (reset parameters)

### Mobile Optimization
- Target 60 FPS on mid-range devices
- Use URP (Universal Render Pipeline)
- Batch draw calls (shared materials)
- LOD not needed (simple geometry)
- Texture compression (ASTC for mobile)
- Resolution scaling (720p-1080p dynamic)

### Memory Management
- Use structs for small data
- Avoid LINQ in Update loops
- Cache GetComponent calls
- Use object pooling (zero GC)
- Profile with Unity Profiler

---

## 🚀 DEPLOYMENT

### Build Settings
- Target: Android 11+ (API Level 30)
- iOS 13+
- Architecture: ARM64
- Compression: LZ4 (fast)
- Split APKs by ABI

### Required Permissions
- Android: INTERNET, ACCESS_NETWORK_STATE
- iOS: AppTrackingTransparency (ATT)

### GDPR/Privacy
- GDPR consent dialog (Mobile Monetization Pro handles)
- Privacy policy URL in settings
- Data deletion request support

---

## ✅ CHECKLIST

### Core Systems
- [ ] Player movement (joystick)
- [ ] Auto-shooting (nearest enemy targeting)
- [ ] Enemy spawning (wave-based scaling)
- [ ] Object pooling (enemies, projectiles)
- [ ] Procedural meshes (all geometric shapes)
- [ ] Damage/health system
- [ ] Level-up system (XP, upgrades)
- [ ] Upgrade synergies
- [ ] Death/restart flow

### Meta-Progression
- [ ] Gold earning (in-run, offline)
- [ ] Workshop (permanent upgrades)
- [ ] Prestige system (reset, PP calculation)
- [ ] Ship collection (15 ships)
- [ ] Ship unlocking system
- [ ] Idle earnings (offline 4-hour cap)
- [ ] Save/load (Easy Save 3)

### UI
- [ ] HUD (HP, XP, gold, wave)
- [ ] Level-up screen (3 upgrade choices)
- [ ] Death screen (stats recap)
- [ ] Main menu
- [ ] Workshop screen (scrollable upgrades)
- [ ] Ships screen (grid, unlock status)
- [ ] Settings screen

### Monetization
- [ ] Rewarded ads (revive, double gold, offline multiplier)
- [ ] Interstitial ads (rate limiting)
- [ ] IAP (ships, gold, ad removal, battle pass)
- [ ] Daily bonus system
- [ ] Battle Pass structure

### Polish
- [ ] Particle effects (explosions, trails)
- [ ] Post-processing (bloom, color grading)
- [ ] Sound effects (shooting, explosions, level-up)
- [ ] Background music (looping neon synthwave)
- [ ] Screen shake (damage feedback)
- [ ] Haptic feedback (iOS/Android)

### Testing
- [ ] Balance testing (difficulty curve)
- [ ] Retention hooks (daily login, offline)
- [ ] Monetization placement testing
- [ ] Performance profiling (60 FPS)
- [ ] Analytics integration
- [ ] Crash reporting (Unity Cloud Diagnostics)

---

**Bu doküman oyunun teknik implementasyonunun blueprint'i. Her sistem detaylı kod örnekleriyle açıklandı.**

**Geliştirme başlasın! 🚀**
