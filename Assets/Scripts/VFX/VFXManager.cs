using UnityEngine;
using NeonSurvivors.Core;

namespace NeonSurvivors.VFX
{
    /// <summary>
    /// VFXManager - Visual Effects Manager
    /// Handles all visual feedback in the game (explosions, hit effects, level-up particles, etc.)
    /// This is a placeholder implementation for future VFX integration
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        private static VFXManager _instance;
        public static VFXManager Instance => _instance;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ==================== PLACEHOLDER VFX METHODS ====================

        /// <summary>
        /// Spawns an explosion effect at the specified position
        /// TODO: Implement particle system or VFX Graph
        /// </summary>
        public void PlayExplosion(Vector3 position, float radius, Color color)
        {
            SimpleParticleSystem.CreateExplosion(position, color, radius);
        }

        /// <summary>
        /// Plays a hit flash effect on the specified GameObject
        /// TODO: Implement material flash or sprite flash
        /// </summary>
        public void PlayHitFlash(GameObject target, Color flashColor, float duration = 0.1f)
        {
            SimpleParticleSystem.CreateHitFlash(target, flashColor, duration);
        }

        /// <summary>
        /// Spawns a damage number popup
        /// TODO: Implement floating text system
        /// </summary>
        public void ShowDamageNumber(Vector3 position, float damage, bool isCritical = false)
        {
            SimpleParticleSystem.CreateDamageNumber(position, damage, isCritical);
        }

        /// <summary>
        /// Plays projectile trail effect
        /// TODO: Implement trail renderer or particle trail
        /// </summary>
        public void PlayProjectileTrail(GameObject projectile, Color trailColor)
        {
            SimpleParticleSystem.CreateProjectileTrail(projectile, trailColor);
        }

        /// <summary>
        /// Plays level-up effect on player
        /// TODO: Implement particle burst
        /// </summary>
        public void PlayLevelUpEffect(Vector3 playerPosition)
        {
            SimpleParticleSystem.CreateExplosion(playerPosition, Color.cyan, 3f, 30);
        }

        /// <summary>
        /// Plays chain lightning effect between targets
        /// TODO: Implement line renderer chain effect
        /// </summary>
        public void PlayChainLightning(Vector3 start, Vector3 end, Color lightningColor)
        {
            Debug.Log($"VFX: Chain lightning from {start} to {end}");
            // TODO: Use LineRenderer or VFX Graph for lightning
            // Could use pooled LineRenderer objects
        }

        /// <summary>
        /// Plays freeze effect on enemy
        /// TODO: Implement shader effect or particle overlay
        /// </summary>
        public void PlayFreezeEffect(GameObject enemy, float duration)
        {
            Debug.Log($"VFX: Freeze effect on {enemy.name} for {duration}s");
            // TODO: Apply freeze shader or spawn ice particles
        }

        /// <summary>
        /// Plays burn effect on enemy
        /// TODO: Implement particle flame effect
        /// </summary>
        public void PlayBurnEffect(GameObject enemy, float duration)
        {
            Debug.Log($"VFX: Burn effect on {enemy.name} for {duration}s");
            // TODO: Spawn flame particles attached to enemy
        }

        /// <summary>
        /// Plays gold pickup effect
        /// TODO: Implement particle collection animation
        /// </summary>
        public void PlayGoldPickupEffect(Vector3 goldPosition, Vector3 playerPosition)
        {
            Debug.Log($"VFX: Gold pickup from {goldPosition} to {playerPosition}");
            // TODO: Animate gold sprite/particle flying to player
        }

        /// <summary>
        /// Plays screen shake effect
        /// TODO: Implement camera shake
        /// </summary>
        public void PlayScreenShake(float intensity, float duration)
        {
            Debug.Log($"VFX: Screen shake intensity {intensity} for {duration}s");
            // TODO: Apply camera shake to main camera
            // StartCoroutine(CameraShake(Camera.main, intensity, duration));
        }

        // ==================== FUTURE IMPLEMENTATION NOTES ====================

        /*
         * RECOMMENDED IMPLEMENTATION APPROACH:
         *
         * 1. Create VFX Prefabs:
         *    - Use Unity Particle System for basic effects
         *    - Use VFX Graph (if using URP/HDRP) for advanced effects
         *    - Create simple geometric particle sprites
         *
         * 2. Object Pooling:
         *    - Add VFX prefabs to PoolManager
         *    - Pool sizes: Explosion=50, HitFlash=100, DamageNumber=200
         *
         * 3. Performance Considerations:
         *    - Limit max particles: 1000-2000 total
         *    - Use simple shaders (unlit/additive)
         *    - Auto-despawn particles after lifetime
         *
         * 4. Neon Aesthetic:
         *    - Use bloom-friendly colors (cyan, magenta, yellow)
         *    - Additive blend mode for glow effect
         *    - High emission intensity
         *
         * 5. Integration Points:
         *    - Projectile.cs: ApplyExplosion(), ApplyChainLightning()
         *    - EnemyBase.cs: TakeDamage(), ApplyStatusEffects()
         *    - PlayerHealth.cs: TakeDamage()
         *    - LevelSystem.cs: LevelUp()
         */
    }
}
