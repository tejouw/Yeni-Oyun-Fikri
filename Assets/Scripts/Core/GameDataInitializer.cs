using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Data;

namespace NeonSurvivors.Core
{
    /// <summary>
    /// Runtime'da tüm oyun datalarını oluşturur
    /// ScriptableObject yerine kod ile data generation
    /// </summary>
    public class GameDataInitializer : MonoBehaviour
    {
        private static GameDataInitializer _instance;
        public static GameDataInitializer Instance => _instance;

        [Header("Generated Data")]
        public List<ShipData> ships = new List<ShipData>();
        public List<EnemyData> enemies = new List<EnemyData>();
        public List<UpgradeData> upgrades = new List<UpgradeData>();
        public List<WeaponData> weapons = new List<WeaponData>();

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAllData();
        }

        void InitializeAllData()
        {
            CreateShips();
            CreateEnemies();
            CreateUpgrades();
            CreateWeapons();

            Debug.Log($"Game Data Initialized: {ships.Count} ships, {enemies.Count} enemies, {upgrades.Count} upgrades");
        }

        // ==================== SHIPS ====================

        void CreateShips()
        {
            // 1. Starter Ship (Triangle)
            ships.Add(CreateShip(
                "ship_starter", "Starter Ship",
                "Balanced starter ship with no special abilities",
                100f, 5f, 10f, 1f, 2f,
                ShipMeshType.Triangle, Color.cyan, Color.cyan, 1f,
                ShipAbilityType.None, 0f, "No special ability",
                UnlockType.Default, 0, "Available from start"
            ));

            // 2. Tank Ship (Pentagon)
            ships.Add(CreateShip(
                "ship_tank", "Tank",
                "High HP, slow movement, damage reduction",
                200f, 4f, 10f, 0.9f, 2f,
                ShipMeshType.Pentagon, new Color(0.3f, 0.5f, 1f), new Color(0.3f, 0.5f, 1f), 1.2f,
                ShipAbilityType.DamageReduction, 0.25f, "Take 25% less damage",
                UnlockType.ReachWave, 5, "Reach wave 5"
            ));

            // 3. Speed Ship (Arrow)
            ships.Add(CreateShip(
                "ship_speed", "Speed Demon",
                "Fast movement, low HP, dodge chance",
                80f, 7f, 10f, 1.1f, 2.5f,
                ShipMeshType.Arrow, Color.yellow, Color.yellow, 0.9f,
                ShipAbilityType.DodgeChance, 0.15f, "15% chance to dodge attacks",
                UnlockType.SurviveTime, 300, "Survive 5 minutes in one run"
            ));

            // 4. Laser Ship (Diamond)
            ships.Add(CreateShip(
                "ship_laser", "Laser Focus",
                "Pierce bonus, long range attacks",
                90f, 5f, 12f, 1f, 2.2f,
                ShipMeshType.Diamond, new Color(1f, 0.3f, 0.3f), new Color(1f, 0.3f, 0.3f), 1f,
                ShipAbilityType.PierceBonus, 2f, "Projectiles pierce +2 enemies",
                UnlockType.KillEnemies, 1000, "Kill 1000 enemies total"
            ));

            // 5. Swarm Ship (Star)
            ships.Add(CreateShip(
                "ship_swarm", "Swarm Master",
                "Extra projectiles, lower damage per shot",
                100f, 5f, 8f, 1.2f, 2f,
                ShipMeshType.Star, new Color(0.8f, 0.3f, 1f), new Color(0.8f, 0.3f, 1f), 1.1f,
                ShipAbilityType.ExtraProjectiles, 2f, "+2 projectiles",
                UnlockType.PrestigeCount, 1, "Prestige once"
            ));

            // 6. Gold Farmer (Hexagon)
            ships.Add(CreateShip(
                "ship_gold", "Gold Rush",
                "Bonus gold earning, standard stats",
                100f, 5f, 10f, 1f, 2f,
                ShipMeshType.Hexagon, new Color(1f, 0.84f, 0f), new Color(1f, 0.84f, 0f), 1f,
                ShipAbilityType.GoldBonus, 0.5f, "+50% gold earning",
                UnlockType.SpendGold, 50000, "Spend 50,000 gold total"
            ));

            // 7-15: Additional ships (Premium and progression-locked)
            CreateAdditionalShips();
        }

        void CreateAdditionalShips()
        {
            // 7. Fortress (Premium)
            ships.Add(CreateShip(
                "ship_fortress", "Fortress", "Massive HP, slow but unstoppable",
                300f, 3.5f, 10f, 0.8f, 2f,
                ShipMeshType.Pentagon, new Color(0.5f, 0.5f, 0.5f), new Color(0.5f, 0.5f, 0.5f), 1.5f,
                ShipAbilityType.HealthRegen, 2f, "Regenerate 2 HP/second",
                UnlockType.Premium, 0, "Premium Ship - $0.99", true, 0.99f
            ));

            // 8. Ghost (High dodge)
            ships.Add(CreateShip(
                "ship_ghost", "Ghost", "Ultra-fast with high dodge chance",
                70f, 8f, 10f, 1.1f, 2.5f,
                ShipMeshType.Star, new Color(0.5f, 0.5f, 1f, 0.7f), new Color(0.5f, 0.5f, 1f), 0.8f,
                ShipAbilityType.DodgeChance, 0.3f, "30% dodge chance",
                UnlockType.ReachWave, 15, "Reach wave 15"
            ));

            // 9. Berserker (High damage)
            ships.Add(CreateShip(
                "ship_berserker", "Berserker", "Glass cannon - high damage, low HP",
                70f, 5f, 15f, 1.3f, 2f,
                ShipMeshType.Arrow, new Color(1f, 0f, 0f), new Color(1f, 0f, 0f), 1f,
                ShipAbilityType.None, 0f, "50% more base damage",
                UnlockType.KillEnemies, 5000, "Kill 5000 enemies total"
            ));

            // 10. XP Farmer
            ships.Add(CreateShip(
                "ship_scholar", "Scholar", "Bonus XP gain for faster leveling",
                100f, 5f, 10f, 1f, 2f,
                ShipMeshType.Diamond, new Color(0f, 1f, 0.5f), new Color(0f, 1f, 0.5f), 1f,
                ShipAbilityType.ExpBonus, 0.25f, "+25% XP gain",
                UnlockType.PrestigeCount, 3, "Prestige 3 times"
            ));

            // 11-15 placeholder for future content
            for (int i = 11; i <= 15; i++)
            {
                ships.Add(CreateShip(
                    $"ship_future_{i}", $"Future Ship {i}", "Coming soon!",
                    100f, 5f, 10f, 1f, 2f,
                    ShipMeshType.Triangle, Color.gray, Color.gray, 1f,
                    ShipAbilityType.None, 0f, "Coming in future update",
                    UnlockType.Premium, 0, "Future Content", true, 0.99f
                ));
            }
        }

        ShipData CreateShip(string id, string name, string desc, float hp, float speed, float dmg, float fireRate, float pickupRange,
                           ShipMeshType mesh, Color color, Color emission, float scale,
                           ShipAbilityType ability, float abilityVal, string abilityDesc,
                           UnlockType unlock, int unlockVal, string unlockDesc, bool premium = false, float price = 0.99f)
        {
            ShipData ship = ScriptableObject.CreateInstance<ShipData>();
            ship.shipID = id;
            ship.shipName = name;
            ship.description = desc;
            ship.maxHealth = hp;
            ship.moveSpeed = speed;
            ship.baseDamage = dmg;
            ship.fireRate = fireRate;
            ship.pickupRange = pickupRange;
            ship.meshType = mesh;
            ship.primaryColor = color;
            ship.emissionColor = emission;
            ship.meshScale = scale;
            ship.abilityType = ability;
            ship.abilityValue = abilityVal;
            ship.abilityDescription = abilityDesc;
            ship.unlockType = unlock;
            ship.unlockValue = unlockVal;
            ship.unlockDescription = unlockDesc;
            ship.isPremium = premium;
            ship.priceUSD = price;
            return ship;
        }

        // ==================== ENEMIES ====================

        void CreateEnemies()
        {
            // 1. Basic Enemy (Cube)
            enemies.Add(CreateEnemy(
                "enemy_basic", "Basic Drone", EnemyType.Basic,
                10f, 3f, 10f, 10, 10,
                EnemyMeshType.Cube, new Color(1f, 0.2f, 0.2f), 0.5f,
                EnemyBehavior.ChasePlayer, 1f, 20f, 0f
            ));

            // 2. Fast Enemy (Pyramid)
            enemies.Add(CreateEnemy(
                "enemy_fast", "Fast Striker", EnemyType.Fast,
                5f, 5f, 8f, 15, 15,
                EnemyMeshType.Pyramid, new Color(1f, 1f, 0.2f), 0.4f,
                EnemyBehavior.ZigZag, 1f, 20f, 0f
            ));

            // 3. Tank Enemy (Sphere)
            enemies.Add(CreateEnemy(
                "enemy_tank", "Heavy Tank", EnemyType.Tank,
                50f, 2f, 15f, 50, 30,
                EnemyMeshType.Sphere, new Color(0.3f, 0.3f, 1f), 0.7f,
                EnemyBehavior.StraightLine, 1f, 20f, 0f
            ));

            // 4. Shooter Enemy (Cylinder)
            enemies.Add(CreateEnemy(
                "enemy_shooter", "Ranged Attacker", EnemyType.Shooter,
                15f, 2.5f, 5f, 20, 20,
                EnemyMeshType.Cylinder, new Color(1f, 0.5f, 0f), 0.5f,
                EnemyBehavior.KeepDistance, 5f, 20f, 3f,
                true, 3f, 10f, 5f
            ));

            // 5. Splitter Enemy (Octahedron)
            enemies.Add(CreateEnemy(
                "enemy_splitter", "Splitter", EnemyType.Splitter,
                20f, 3f, 10f, 30, 25,
                EnemyMeshType.Octahedron, new Color(0.8f, 0.2f, 0.8f), 0.6f,
                EnemyBehavior.CircleStrafe, 1f, 20f, 0f,
                false, 0f, 0f, 0f, 3
            ));

            // 6. Boss Enemy (Icosahedron)
            enemies.Add(CreateEnemy(
                "enemy_boss", "Boss", EnemyType.Boss,
                500f, 2f, 20f, 500, 100,
                EnemyMeshType.Icosahedron, new Color(1f, 0f, 0f), 2f,
                EnemyBehavior.CircleStrafe, 1.5f, 30f, 5f,
                true, 2f, 15f, 10f, 0, true
            ));
        }

        EnemyData CreateEnemy(string id, string name, EnemyType type,
                             float hp, float speed, float dmg, int gold, int xp,
                             EnemyMeshType mesh, Color color, float scale,
                             EnemyBehavior behavior, float atkRange, float detectRange, float atkCooldown,
                             bool canShoot = false, float shootCooldown = 0f, float projSpeed = 0f, float projDmg = 0f,
                             int splitCount = 0, bool isBoss = false)
        {
            EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
            enemy.enemyID = id;
            enemy.enemyName = name;
            enemy.enemyType = type;
            enemy.baseHealth = hp;
            enemy.moveSpeed = speed;
            enemy.damage = dmg;
            enemy.goldValue = gold;
            enemy.xpValue = xp;
            enemy.meshType = mesh;
            enemy.enemyColor = color;
            enemy.meshScale = scale;
            enemy.behavior = behavior;
            enemy.attackRange = atkRange;
            enemy.detectionRange = detectRange;
            enemy.attackCooldown = atkCooldown;
            enemy.canShoot = canShoot;
            enemy.shootCooldown = shootCooldown;
            enemy.projectileSpeed = projSpeed;
            enemy.projectileDamage = projDmg;
            enemy.splitCount = splitCount;
            enemy.isBoss = isBoss;
            return enemy;
        }

        // ==================== UPGRADES ====================

        void CreateUpgrades()
        {
            // COMMON UPGRADES (60% drop rate)

            // Damage upgrades
            upgrades.Add(CreateUpgrade("upgrade_dmg_1", "Damage Boost", "Increase damage by 20%",
                UpgradeCategory.Damage, UpgradeRarity.Common, 1.2f));

            upgrades.Add(CreateUpgrade("upgrade_dmg_2", "Power Surge", "Increase damage by 15%",
                UpgradeCategory.Damage, UpgradeRarity.Common, 1.15f));

            // Fire rate upgrades
            upgrades.Add(CreateUpgrade("upgrade_fire_1", "Rapid Fire", "Increase fire rate by 15%",
                UpgradeCategory.FireRate, UpgradeRarity.Common, fireRateMult: 1.15f));

            upgrades.Add(CreateUpgrade("upgrade_fire_2", "Quick Shooter", "Increase fire rate by 10%",
                UpgradeCategory.FireRate, UpgradeRarity.Common, fireRateMult: 1.1f));

            // Speed upgrades
            upgrades.Add(CreateUpgrade("upgrade_speed_1", "Swift Moves", "Increase movement speed by 10%",
                UpgradeCategory.Mobility, UpgradeRarity.Common, moveSpeedMult: 1.1f));

            // Health upgrades
            upgrades.Add(CreateUpgrade("upgrade_hp_1", "Health Boost", "Increase max HP by 20",
                UpgradeCategory.Survival, UpgradeRarity.Common, maxHpAdd: 20f));

            // UNCOMMON UPGRADES (25% drop rate)

            // Projectile count
            upgrades.Add(CreateUpgrade("upgrade_proj_1", "Multi-Shot", "+1 projectile",
                UpgradeCategory.Projectile, UpgradeRarity.Uncommon, projCount: 1));

            // Pierce
            upgrades.Add(CreateUpgrade("upgrade_pierce_1", "Piercing Shot", "Projectiles pierce +1 enemy",
                UpgradeCategory.Projectile, UpgradeRarity.Uncommon, pierce: 1));

            // Bigger damage
            upgrades.Add(CreateUpgrade("upgrade_dmg_3", "Heavy Hitter", "Increase damage by 30%",
                UpgradeCategory.Damage, UpgradeRarity.Uncommon, 1.3f));

            // Speed boost
            upgrades.Add(CreateUpgrade("upgrade_speed_2", "Speed Demon", "Increase movement speed by 20%",
                UpgradeCategory.Mobility, UpgradeRarity.Uncommon, moveSpeedMult: 1.2f));

            // RARE UPGRADES (12% drop rate)

            // Bounce
            upgrades.Add(CreateUpgrade("upgrade_bounce_1", "Ricochet", "Projectiles bounce once",
                UpgradeCategory.Projectile, UpgradeRarity.Rare, bounce: 1));

            // Critical hit
            upgrades.Add(CreateUpgrade("upgrade_crit_1", "Critical Strike", "+10% critical hit chance (2x damage)",
                UpgradeCategory.Special, UpgradeRarity.Rare, critChance: 0.1f, critMult: 2f));

            // Explosion
            upgrades.Add(CreateUpgrade("upgrade_explosion_1", "Explosive Rounds", "Projectiles explode on hit (radius 2)",
                UpgradeCategory.Special, UpgradeRarity.Rare, explosionRadius: 2f));

            // Freeze
            upgrades.Add(CreateUpgrade("upgrade_freeze_1", "Frost Touch", "20% chance to freeze enemies for 2 seconds",
                UpgradeCategory.Special, UpgradeRarity.Rare, freezeChance: 0.2f, freezeDur: 2f));

            // EPIC UPGRADES (3% drop rate)

            // Chain Lightning
            upgrades.Add(CreateUpgrade("upgrade_chain_1", "Chain Lightning", "30% chance to chain to 3 nearby enemies",
                UpgradeCategory.Special, UpgradeRarity.Epic, chainChance: 0.3f, chainTargets: 3));

            // Burn
            upgrades.Add(CreateUpgrade("upgrade_burn_1", "Inferno", "30% chance to burn enemies (10 DPS for 3s)",
                UpgradeCategory.Special, UpgradeRarity.Epic, burnChance: 0.3f, burnDPS: 10f));

            // Lifesteal
            upgrades.Add(CreateUpgrade("upgrade_lifesteal_1", "Vampiric", "Heal for 5% of damage dealt",
                UpgradeCategory.Special, UpgradeRarity.Epic, lifesteal: 0.05f));

            // Multiple projectiles
            upgrades.Add(CreateUpgrade("upgrade_proj_2", "Projectile Storm", "+2 projectiles",
                UpgradeCategory.Projectile, UpgradeRarity.Epic, projCount: 2));

            // Mega damage
            upgrades.Add(CreateUpgrade("upgrade_dmg_epic", "Devastation", "Increase damage by 50%",
                UpgradeCategory.Damage, UpgradeRarity.Epic, 1.5f));

            // MORE UPGRADES FOR VARIETY (20+ total)
            CreateAdditionalUpgrades();
        }

        void CreateAdditionalUpgrades()
        {
            // More common upgrades
            upgrades.Add(CreateUpgrade("upgrade_dmg_common_3", "Damage Up", "+10% damage",
                UpgradeCategory.Damage, UpgradeRarity.Common, 1.1f));

            upgrades.Add(CreateUpgrade("upgrade_fire_common_3", "Faster Shots", "+8% fire rate",
                UpgradeCategory.FireRate, UpgradeRarity.Common, fireRateMult: 1.08f));

            // More uncommon
            upgrades.Add(CreateUpgrade("upgrade_hp_uncommon", "Fortify", "+50 max HP",
                UpgradeCategory.Survival, UpgradeRarity.Uncommon, maxHpAdd: 50f));

            upgrades.Add(CreateUpgrade("upgrade_proj_speed", "Fast Projectiles", "+20% projectile speed",
                UpgradeCategory.Projectile, UpgradeRarity.Uncommon, projSpeedMult: 1.2f));

            // More rare
            upgrades.Add(CreateUpgrade("upgrade_pierce_2", "Deep Pierce", "Pierce +2 enemies",
                UpgradeCategory.Projectile, UpgradeRarity.Rare, pierce: 2));

            upgrades.Add(CreateUpgrade("upgrade_bounce_2", "Multi-Bounce", "Bounce 2 times",
                UpgradeCategory.Projectile, UpgradeRarity.Rare, bounce: 2));

            upgrades.Add(CreateUpgrade("upgrade_crit_2", "Master Critical", "+15% crit chance, 2.5x damage",
                UpgradeCategory.Special, UpgradeRarity.Rare, critChance: 0.15f, critMult: 2.5f));

            // More epic
            upgrades.Add(CreateUpgrade("upgrade_combo", "Ultimate Combo", "+1 projectile, +20% damage, +10% speed",
                UpgradeCategory.Special, UpgradeRarity.Epic, 1.2f, projCount: 1, moveSpeedMult: 1.1f));

            upgrades.Add(CreateUpgrade("upgrade_massive_dmg", "Annihilation", "+100% damage",
                UpgradeCategory.Damage, UpgradeRarity.Epic, 2f));
        }

        UpgradeData CreateUpgrade(string id, string name, string desc,
                                 UpgradeCategory cat, UpgradeRarity rarity,
                                 float dmgMult = 1f, float dmgAdd = 0f, float fireRateMult = 1f, float fireRateAdd = 0f,
                                 int projCount = 0, float projSpeedMult = 1f, float moveSpeedMult = 1f,
                                 float maxHpMult = 1f, float maxHpAdd = 0f, int pierce = 0, int bounce = 0,
                                 float explosionRadius = 0f, float chainChance = 0f, int chainTargets = 0,
                                 float freezeChance = 0f, float freezeDur = 0f, float burnChance = 0f, float burnDPS = 0f,
                                 float critChance = 0f, float critMult = 2f, float lifesteal = 0f)
        {
            UpgradeData upgrade = ScriptableObject.CreateInstance<UpgradeData>();
            upgrade.upgradeID = id;
            upgrade.upgradeName = name;
            upgrade.description = desc;
            upgrade.category = cat;
            upgrade.rarity = rarity;
            upgrade.damageMultiplier = dmgMult;
            upgrade.damageAdditive = dmgAdd;
            upgrade.fireRateMultiplier = fireRateMult;
            upgrade.fireRateAdditive = fireRateAdd;
            upgrade.projectileCountBonus = projCount;
            upgrade.projectileSpeedMultiplier = projSpeedMult;
            upgrade.moveSpeedMultiplier = moveSpeedMult;
            upgrade.maxHealthMultiplier = maxHpMult;
            upgrade.maxHealthAdditive = maxHpAdd;
            upgrade.pierceBonus = pierce;
            upgrade.bounceBonus = bounce;
            upgrade.explosionRadius = explosionRadius;
            upgrade.chainLightningChance = chainChance;
            upgrade.chainLightningTargets = chainTargets;
            upgrade.freezeChance = freezeChance;
            upgrade.freezeDuration = freezeDur;
            upgrade.burnChance = burnChance;
            upgrade.burnDamagePerSecond = burnDPS;
            upgrade.criticalChance = critChance;
            upgrade.criticalMultiplier = critMult;
            upgrade.lifestealPercent = lifesteal;

            // Icon color based on rarity
            upgrade.iconColor = rarity switch
            {
                UpgradeRarity.Common => Color.gray,
                UpgradeRarity.Uncommon => Color.green,
                UpgradeRarity.Rare => Color.blue,
                UpgradeRarity.Epic => new Color(0.8f, 0.2f, 0.8f),
                _ => Color.white
            };

            upgrade.canStack = true;
            upgrade.maxStacks = 10;

            return upgrade;
        }

        // ==================== WEAPONS ====================

        void CreateWeapons()
        {
            // 1. Blaster (default)
            weapons.Add(CreateWeapon(
                "weapon_blaster", "Blaster", "Standard rapid-fire weapon",
                WeaponType.Blaster, 10f, 1f, 15f, 1, 3f, 20f,
                ProjectileShape.Sphere, Color.cyan, 0.2f
            ));

            // 2. Laser
            weapons.Add(CreateWeapon(
                "weapon_laser", "Laser Beam", "Continuous beam weapon",
                WeaponType.Laser, 5f, 10f, 20f, 1, 5f, 30f,
                ProjectileShape.Capsule, Color.red, 0.15f
            ));

            // 3. Missiles
            weapons.Add(CreateWeapon(
                "weapon_missiles", "Homing Missiles", "Lock-on missiles",
                WeaponType.Missiles, 20f, 0.5f, 10f, 1, 4f, 25f,
                ProjectileShape.Cylinder, Color.yellow, 0.25f
            ));

            // 4. Shotgun
            weapons.Add(CreateWeapon(
                "weapon_shotgun", "Shotgun", "Wide spread, close range",
                WeaponType.Shotgun, 8f, 0.8f, 12f, 5, 2f, 10f,
                ProjectileShape.Cube, new Color(1f, 0.5f, 0f), 0.15f
            ));

            // 5. Orbit
            weapons.Add(CreateWeapon(
                "weapon_orbit", "Orbit Shield", "Rotating protective shield",
                WeaponType.Orbit, 15f, 5f, 5f, 3, 999f, 3f,
                ProjectileShape.Sphere, new Color(0.5f, 1f, 0.5f), 0.3f
            ));
        }

        WeaponData CreateWeapon(string id, string name, string desc,
                               WeaponType type, float dmg, float fireRate, float projSpeed,
                               int projCount, float lifetime, float range,
                               ProjectileShape shape, Color color, float scale)
        {
            WeaponData weapon = ScriptableObject.CreateInstance<WeaponData>();
            weapon.weaponID = id;
            weapon.weaponName = name;
            weapon.description = desc;
            weapon.weaponType = type;
            weapon.baseDamage = dmg;
            weapon.fireRate = fireRate;
            weapon.projectileSpeed = projSpeed;
            weapon.projectileCount = projCount;
            weapon.projectileLifetime = lifetime;
            weapon.range = range;
            weapon.projectileShape = shape;
            weapon.projectileColor = color;
            weapon.projectileScale = scale;
            weapon.hasTrail = true;
            weapon.isUnlocked = true;
            return weapon;
        }

        // ==================== PUBLIC GETTERS ====================

        public ShipData GetShip(string shipID)
        {
            return ships.Find(s => s.shipID == shipID);
        }

        public EnemyData GetEnemy(string enemyID)
        {
            return enemies.Find(e => e.enemyID == enemyID);
        }

        public UpgradeData GetUpgrade(string upgradeID)
        {
            return upgrades.Find(u => u.upgradeID == upgradeID);
        }

        public WeaponData GetWeapon(string weaponID)
        {
            return weapons.Find(w => w.weaponID == weaponID);
        }

        public List<ShipData> GetAllShips() => new List<ShipData>(ships);
        public List<EnemyData> GetAllEnemies() => new List<EnemyData>(enemies);
        public List<UpgradeData> GetAllUpgrades() => new List<UpgradeData>(upgrades);
        public List<WeaponData> GetAllWeapons() => new List<WeaponData>(weapons);
    }
}
