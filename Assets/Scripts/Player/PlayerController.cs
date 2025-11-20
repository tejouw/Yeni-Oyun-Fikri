using UnityEngine;
using NeonSurvivors.Data;
using NeonSurvivors.Utilities;

namespace NeonSurvivors.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerWeapon))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public ShipData currentShip;
        private Rigidbody rb;
        private PlayerHealth health;
        private PlayerWeapon weapon;
        private PlayerUpgrades upgrades;

        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        private Vector3 moveDirection;

        [Header("Boundaries")]
        public float boundaryX = 10f;
        public float boundaryZ = 10f;

        [Header("Input")]
        private Vector3 joystickInput;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            health = GetComponent<PlayerHealth>();
            weapon = GetComponent<PlayerWeapon>();
            upgrades = GetComponent<PlayerUpgrades>();

            // Rigidbody settings
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        }

        void Start()
        {
            if (currentShip != null)
            {
                InitializeFromShipData();
            }
            else
            {
                Debug.LogWarning("No ship data assigned to PlayerController!");
            }
        }

        void Update()
        {
            HandleInput();
        }

        void FixedUpdate()
        {
            MovePlayer();
            RotatePlayer();
        }

        void HandleInput()
        {
            // Mobile touch input
            #if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Simple joystick: touch anywhere, drag to move
                Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));
                Vector3 direction = (touchWorldPos - transform.position);
                direction.y = 0;
                moveDirection = direction.normalized;
            }
            else
            {
                moveDirection = Vector3.zero;
            }
            #endif

            // Desktop input for testing
            #if UNITY_EDITOR || UNITY_STANDALONE
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(h, 0, v).normalized;
            #endif
        }

        void MovePlayer()
        {
            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 velocity = moveDirection * moveSpeed;
                rb.velocity = velocity;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }

            // Clamp position to boundaries
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            pos.z = Mathf.Clamp(pos.z, -boundaryZ, boundaryZ);
            pos.y = 0; // Keep at ground level
            transform.position = pos;
        }

        void RotatePlayer()
        {
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }

        public void InitializeFromShipData()
        {
            if (currentShip == null)
                return;

            // Set stats
            moveSpeed = currentShip.moveSpeed;
            health.SetMaxHealth(currentShip.maxHealth);
            weapon.SetBaseDamage(currentShip.baseDamage);
            weapon.SetFireRate(currentShip.fireRate);

            // Apply ship ability
            ApplyShipAbility();

            // Generate visual
            GenerateShipVisual();

            Debug.Log($"Player initialized with ship: {currentShip.shipName}");
        }

        void ApplyShipAbility()
        {
            switch (currentShip.abilityType)
            {
                case ShipAbilityType.GoldBonus:
                    // Handled in gold collection
                    break;
                case ShipAbilityType.SpeedBoost:
                    moveSpeed *= (1f + currentShip.abilityValue);
                    break;
                case ShipAbilityType.FireRateBonus:
                    weapon.SetFireRate(currentShip.fireRate * (1f + currentShip.abilityValue));
                    break;
                case ShipAbilityType.ExtraProjectiles:
                    weapon.AddProjectileCount((int)currentShip.abilityValue);
                    break;
                // Add other abilities as needed
            }
        }

        void GenerateShipVisual()
        {
            // Create mesh based on ship type
            Mesh mesh = null;
            switch (currentShip.meshType)
            {
                case ShipMeshType.Triangle:
                    mesh = ProceduralMeshGenerator.CreateTriangle(currentShip.meshScale);
                    break;
                case ShipMeshType.Pentagon:
                    mesh = ProceduralMeshGenerator.CreatePentagon(currentShip.meshScale);
                    break;
                case ShipMeshType.Arrow:
                    mesh = ProceduralMeshGenerator.CreateArrow(currentShip.meshScale);
                    break;
                case ShipMeshType.Diamond:
                    mesh = ProceduralMeshGenerator.CreateDiamond(currentShip.meshScale);
                    break;
                case ShipMeshType.Star:
                    mesh = ProceduralMeshGenerator.CreateStar(currentShip.meshScale);
                    break;
                case ShipMeshType.Hexagon:
                    mesh = ProceduralMeshGenerator.CreateHexagon(currentShip.meshScale);
                    break;
            }

            // Create neon material
            Material material = ProceduralMeshGenerator.CreateNeonMaterial(currentShip.emissionColor, 3f);

            // Apply to GameObject
            ProceduralMeshGenerator.ApplyMeshToGameObject(gameObject, mesh, material);
        }

        public float GetGoldBonusMultiplier()
        {
            if (currentShip != null && currentShip.abilityType == ShipAbilityType.GoldBonus)
            {
                return 1f + currentShip.abilityValue;
            }
            return 1f;
        }

        public void ApplyUpgradeModifiers()
        {
            if (upgrades == null)
                return;

            // Apply speed modifiers from upgrades
            float speedMult = upgrades.GetMoveSpeedMultiplier();
            moveSpeed = currentShip.moveSpeed * speedMult;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Damage is handled by enemy script
            }
        }
    }
}
