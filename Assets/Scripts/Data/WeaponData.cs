using UnityEngine;

namespace NeonSurvivors.Data
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Neon Survivors/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponID;
        public string weaponName;
        [TextArea(2, 4)]
        public string description;

        [Header("Stats")]
        public WeaponType weaponType;
        public float baseDamage = 10f;
        public float fireRate = 1f;
        public float projectileSpeed = 15f;
        public int projectileCount = 1;
        public float projectileLifetime = 3f;
        public float range = 20f;

        [Header("Visual")]
        public ProjectileShape projectileShape;
        public Color projectileColor = Color.cyan;
        public float projectileScale = 0.2f;
        public bool hasTrail = true;

        [Header("Special")]
        public bool isUnlocked = true;
    }

    public enum WeaponType
    {
        Blaster,        // Standard projectiles
        Laser,          // Continuous beam
        Missiles,       // Homing projectiles
        Shotgun,        // Multi-directional spread
        Orbit           // Rotating shields
    }

    public enum ProjectileShape
    {
        Sphere,
        Cube,
        Cylinder,
        Capsule
    }
}
