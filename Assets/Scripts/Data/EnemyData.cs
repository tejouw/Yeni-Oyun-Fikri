using UnityEngine;

namespace NeonSurvivors.Data
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Neon Survivors/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string enemyID;
        public string enemyName;
        public EnemyType enemyType;

        [Header("Stats")]
        public float baseHealth = 10f;
        public float moveSpeed = 3f;
        public float damage = 10f;
        public int goldValue = 10;
        public int xpValue = 10;

        [Header("Visual")]
        public EnemyMeshType meshType;
        public Color enemyColor = Color.red;
        public float meshScale = 0.5f;

        [Header("Behavior")]
        public EnemyBehavior behavior;
        public float attackRange = 1f;
        public float detectionRange = 20f;
        public float attackCooldown = 2f;

        [Header("Special")]
        public bool isBoss = false;
        public int splitCount = 0; // For splitter enemies
        public bool canShoot = false;
        public float shootCooldown = 3f;
        public float projectileSpeed = 10f;
        public float projectileDamage = 5f;
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

    public enum EnemyMeshType
    {
        Cube,
        Sphere,
        Pyramid,
        Cylinder,
        Octahedron,
        Icosahedron
    }

    public enum EnemyBehavior
    {
        ChasePlayer,
        ZigZag,
        StraightLine,
        CircleStrafe,
        KeepDistance
    }
}
