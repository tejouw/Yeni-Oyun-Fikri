using UnityEngine;
using System;
using NeonSurvivors.Core;

namespace NeonSurvivors.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        [Header("Regeneration")]
        public float healthRegenPerSecond = 0f;

        [Header("Invincibility")]
        public float invincibilityDuration = 1f;
        private float invincibilityTimer = 0f;
        private bool isInvincible = false;

        [Header("Events")]
        public event Action<float, float> OnHealthChanged; // current, max
        public event Action OnDeath;

        void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Update UI health bar
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        void Update()
        {
            // Health regeneration
            if (healthRegenPerSecond > 0 && currentHealth < maxHealth)
            {
                Heal(healthRegenPerSecond * Time.deltaTime);
            }

            // Invincibility timer
            if (isInvincible)
            {
                invincibilityTimer -= Time.deltaTime;
                if (invincibilityTimer <= 0)
                {
                    isInvincible = false;
                }
            }
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            maxHealth = newMaxHealth;
            currentHealth = maxHealth * healthPercentage;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Update UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        public void TakeDamage(float damage)
        {
            if (isInvincible)
                return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Update UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            }

            // Trigger invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;

            // Visual feedback (flash effect could be added here)

            if (currentHealth <= 0)
            {
                Die();
            }

            Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }

        public void Heal(float amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Update UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        public void FullHeal()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        void Die()
        {
            OnDeath?.Invoke();

            // End game
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame();
            }

            Debug.Log("Player died!");

            // Disable player
            gameObject.SetActive(false);
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }

        public void AddHealthRegen(float regenAmount)
        {
            healthRegenPerSecond += regenAmount;
        }

        public void AddMaxHealth(float additionalHealth)
        {
            float percentage = currentHealth / maxHealth;
            maxHealth += additionalHealth;
            currentHealth = maxHealth * percentage;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void MultiplyMaxHealth(float multiplier)
        {
            float percentage = currentHealth / maxHealth;
            maxHealth *= multiplier;
            currentHealth = maxHealth * percentage;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
