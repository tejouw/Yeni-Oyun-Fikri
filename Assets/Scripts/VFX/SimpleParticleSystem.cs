using UnityEngine;
using System.Collections;
using NeonSurvivors.Core;

namespace NeonSurvivors.VFX
{
    /// <summary>
    /// Simple procedural particle system using Unity primitives
    /// Creates basic visual feedback without requiring imported particle assets
    /// </summary>
    public class SimpleParticleSystem : MonoBehaviour
    {
        /// <summary>
        /// Creates an explosion effect at the specified position
        /// </summary>
        public static void CreateExplosion(Vector3 position, Color color, float radius = 2f, int particleCount = 20)
        {
            GameObject explosionRoot = new GameObject("Explosion_Effect");
            explosionRoot.transform.position = position;

            for (int i = 0; i < particleCount; i++)
            {
                // Random direction
                Vector3 direction = Random.insideUnitSphere;
                float speed = Random.Range(3f, 8f);

                // Create particle
                GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(particle.GetComponent<Collider>()); // Remove collider
                particle.transform.position = position;
                particle.transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);

                // Material with emission
                Material mat = new Material(Shader.Find("Standard"));
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 2f);
                mat.color = color;
                particle.GetComponent<Renderer>().material = mat;

                particle.transform.SetParent(explosionRoot.transform);

                // Add particle behavior
                ParticleBehavior behavior = particle.AddComponent<ParticleBehavior>();
                behavior.Initialize(direction * speed, 0.5f);
            }

            // Destroy root after duration
            Destroy(explosionRoot, 1f);
        }

        /// <summary>
        /// Creates damage number text at position
        /// </summary>
        public static void CreateDamageNumber(Vector3 position, float damage, bool isCritical = false)
        {
            // Simple 3D text implementation
            GameObject textObj = new GameObject("DamageNumber");
            textObj.transform.position = position + Vector3.up * 0.5f;

            // Create text mesh
            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = Mathf.RoundToInt(damage).ToString();
            textMesh.fontSize = isCritical ? 48 : 32;
            textMesh.color = isCritical ? Color.yellow : Color.white;
            textMesh.alignment = TextAlignment.Center;
            textMesh.anchor = TextAnchor.MiddleCenter;

            // Add behavior
            DamageNumberBehavior behavior = textObj.AddComponent<DamageNumberBehavior>();
            behavior.Initialize(isCritical);

            // Destroy after duration
            Destroy(textObj, 1f);
        }

        /// <summary>
        /// Creates hit flash effect on GameObject
        /// </summary>
        public static void CreateHitFlash(GameObject target, Color flashColor, float duration = 0.1f)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                FlashBehavior flash = target.GetComponent<FlashBehavior>();
                if (flash == null)
                {
                    flash = target.AddComponent<FlashBehavior>();
                }
                flash.Flash(flashColor, duration);
            }
        }

        /// <summary>
        /// Creates trail effect for projectiles
        /// </summary>
        public static TrailRenderer CreateProjectileTrail(GameObject projectile, Color trailColor)
        {
            TrailRenderer trail = projectile.GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = projectile.AddComponent<TrailRenderer>();
            }

            trail.time = 0.3f;
            trail.startWidth = 0.2f;
            trail.endWidth = 0.05f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = trailColor;
            trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);

            return trail;
        }
    }

    /// <summary>
    /// Particle behavior for explosion particles
    /// </summary>
    public class ParticleBehavior : MonoBehaviour
    {
        private Vector3 velocity;
        private float lifetime;
        private float age = 0f;
        private Vector3 startScale;

        public void Initialize(Vector3 vel, float life)
        {
            velocity = vel;
            lifetime = life;
            startScale = transform.localScale;
        }

        void Update()
        {
            age += Time.deltaTime;

            // Move particle
            transform.position += velocity * Time.deltaTime;

            // Apply gravity
            velocity += Vector3.down * 9.8f * Time.deltaTime;

            // Fade out and shrink
            float progress = age / lifetime;
            transform.localScale = startScale * (1f - progress);

            // Fade material
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = 1f - progress;
                renderer.material.color = color;
            }

            // Destroy when lifetime ends
            if (age >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Damage number floating behavior
    /// </summary>
    public class DamageNumberBehavior : MonoBehaviour
    {
        private float speed = 2f;
        private float lifetime = 1f;
        private float age = 0f;

        public void Initialize(bool isCritical)
        {
            if (isCritical)
            {
                speed = 3f;
                transform.localScale = Vector3.one * 1.5f;
            }
        }

        void Update()
        {
            age += Time.deltaTime;

            // Float upward
            transform.position += Vector3.up * speed * Time.deltaTime;

            // Fade out
            float progress = age / lifetime;
            TextMesh textMesh = GetComponent<TextMesh>();
            if (textMesh != null)
            {
                Color color = textMesh.color;
                color.a = 1f - progress;
                textMesh.color = color;
            }

            // Face camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                                 mainCam.transform.rotation * Vector3.up);
            }
        }
    }

    /// <summary>
    /// Flash effect for hit feedback
    /// </summary>
    public class FlashBehavior : MonoBehaviour
    {
        private Material originalMaterial;
        private Color originalColor;
        private Color originalEmission;
        private Coroutine flashCoroutine;

        public void Flash(Color flashColor, float duration)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashCoroutine(flashColor, duration));
        }

        IEnumerator FlashCoroutine(Color flashColor, float duration)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer == null)
                yield break;

            // Store original
            if (originalMaterial == null)
            {
                originalMaterial = renderer.material;
                originalColor = originalMaterial.color;
                if (originalMaterial.HasProperty("_EmissionColor"))
                {
                    originalEmission = originalMaterial.GetColor("_EmissionColor");
                }
            }

            // Apply flash
            renderer.material.color = flashColor;
            if (renderer.material.HasProperty("_EmissionColor"))
            {
                renderer.material.SetColor("_EmissionColor", flashColor * 2f);
            }

            // Wait
            yield return new WaitForSeconds(duration);

            // Restore original
            renderer.material.color = originalColor;
            if (renderer.material.HasProperty("_EmissionColor"))
            {
                renderer.material.SetColor("_EmissionColor", originalEmission);
            }
        }
    }
}
