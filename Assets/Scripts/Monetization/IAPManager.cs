using UnityEngine;
using System;
using NeonSurvivors.Core;
using NeonSurvivors.Player;

namespace NeonSurvivors.Monetization
{
    /// <summary>
    /// IAP Manager - Placeholder for In-App Purchase integration (Unity IAP)
    /// Replace with actual Unity IAP implementation
    /// </summary>
    public class IAPManager : MonoBehaviour
    {
        private static IAPManager _instance;
        public static IAPManager Instance => _instance;

        [Header("Configuration")]
        public bool iapEnabled = true;
        public bool testMode = true;

        [Header("Product IDs")]
        private const string GOLD_SMALL = "gold_10k";
        private const string GOLD_MEDIUM = "gold_50k";
        private const string GOLD_LARGE = "gold_200k";
        private const string GOLD_MEGA = "gold_1m";
        private const string SHIP_PREMIUM_1 = "ship_tank";
        private const string SHIP_PREMIUM_2 = "ship_laser";
        private const string SHIP_PREMIUM_3 = "ship_fortress";
        private const string AD_REMOVAL = "remove_ads";
        private const string BATTLE_PASS = "battle_pass";
        private const string STARTER_PACK = "starter_pack";

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeIAP();
        }

        void InitializeIAP()
        {
            if (!iapEnabled)
            {
                Debug.Log("IAP disabled");
                return;
            }

            // TODO: Initialize Unity IAP
            // Example:
            // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            // builder.AddProduct(GOLD_SMALL, ProductType.Consumable);
            // builder.AddProduct(AD_REMOVAL, ProductType.NonConsumable);
            // UnityPurchasing.Initialize(this, builder);

            Debug.Log("IAPManager initialized (Placeholder)");
        }

        // ==================== PURCHASE METHODS ====================

        public void PurchaseGold(GoldPackSize size)
        {
            string productID = "";
            int goldAmount = 0;

            switch (size)
            {
                case GoldPackSize.Small:
                    productID = GOLD_SMALL;
                    goldAmount = 10000;
                    break;
                case GoldPackSize.Medium:
                    productID = GOLD_MEDIUM;
                    goldAmount = 50000;
                    break;
                case GoldPackSize.Large:
                    productID = GOLD_LARGE;
                    goldAmount = 200000;
                    break;
                case GoldPackSize.Mega:
                    productID = GOLD_MEGA;
                    goldAmount = 1000000;
                    break;
            }

            if (testMode)
            {
                Debug.Log($"[TEST MODE] Purchased {productID} - {goldAmount} gold");
                OnPurchaseSuccess(productID, goldAmount);
                return;
            }

            // TODO: Initiate actual purchase
            // storeController.InitiatePurchase(productID);

            Debug.Log($"Initiating purchase: {productID}");
        }

        public void PurchaseShip(string shipID)
        {
            if (testMode)
            {
                Debug.Log($"[TEST MODE] Purchased ship: {shipID}");
                OnShipPurchaseSuccess(shipID);
                return;
            }

            // TODO: Initiate actual purchase
            // storeController.InitiatePurchase(shipID);

            Debug.Log($"Initiating ship purchase: {shipID}");
        }

        public void PurchaseAdRemoval()
        {
            if (testMode)
            {
                Debug.Log("[TEST MODE] Purchased ad removal");
                OnAdRemovalPurchaseSuccess();
                return;
            }

            // TODO: Initiate actual purchase
            // storeController.InitiatePurchase(AD_REMOVAL);

            Debug.Log("Initiating ad removal purchase");
        }

        public void PurchaseBattlePass()
        {
            if (testMode)
            {
                Debug.Log("[TEST MODE] Purchased battle pass");
                OnBattlePassPurchaseSuccess();
                return;
            }

            // TODO: Initiate actual purchase
            // storeController.InitiatePurchase(BATTLE_PASS);

            Debug.Log("Initiating battle pass purchase");
        }

        public void PurchaseStarterPack()
        {
            if (testMode)
            {
                Debug.Log("[TEST MODE] Purchased starter pack");
                OnStarterPackPurchaseSuccess();
                return;
            }

            // TODO: Initiate actual purchase
            // storeController.InitiatePurchase(STARTER_PACK);

            Debug.Log("Initiating starter pack purchase");
        }

        // ==================== PURCHASE SUCCESS HANDLERS ====================

        void OnPurchaseSuccess(string productID, int goldAmount)
        {
            // Grant gold
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddGold(goldAmount);
            }

            Debug.Log($"Purchase successful: {productID} - {goldAmount} gold granted");

            // Save
            SaveManager.Instance.SaveGame();

            // Show success message
            ShowPurchaseSuccessMessage($"You received {goldAmount} gold!");
        }

        void OnShipPurchaseSuccess(string shipID)
        {
            // Unlock ship
            if (ShipManager.Instance != null)
            {
                ShipManager.Instance.UnlockShip(shipID);
            }

            Debug.Log($"Ship purchase successful: {shipID}");

            // Save
            SaveManager.Instance.SaveGame();

            // Show success message
            ShowPurchaseSuccessMessage($"Ship unlocked: {shipID}!");
        }

        void OnAdRemovalPurchaseSuccess()
        {
            // Disable ads
            if (AdManager.Instance != null)
            {
                AdManager.Instance.DisableAds();
            }

            Debug.Log("Ad removal purchase successful");

            // Save
            SaveManager.Instance.SaveGame();

            // Show success message
            ShowPurchaseSuccessMessage("Ads removed forever!");
        }

        void OnBattlePassPurchaseSuccess()
        {
            // Grant battle pass
            // TODO: Implement battle pass system

            Debug.Log("Battle pass purchase successful");

            // Save
            SaveManager.Instance.SaveGame();

            // Show success message
            ShowPurchaseSuccessMessage("Battle Pass activated!");
        }

        void OnStarterPackPurchaseSuccess()
        {
            // Grant starter pack contents
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddGold(5000);
            }

            // Unlock 3 premium ships (example)
            if (ShipManager.Instance != null)
            {
                ShipManager.Instance.UnlockShip(SHIP_PREMIUM_1);
                ShipManager.Instance.UnlockShip(SHIP_PREMIUM_2);
                ShipManager.Instance.UnlockShip(SHIP_PREMIUM_3);
            }

            Debug.Log("Starter pack purchase successful");

            // Save
            SaveManager.Instance.SaveGame();

            // Show success message
            ShowPurchaseSuccessMessage("Starter Pack received!");
        }

        // ==================== UI METHODS ====================

        void ShowPurchaseSuccessMessage(string message)
        {
            Debug.Log($"Purchase Success: {message}");
            // TODO: Show UI notification
        }

        void ShowPurchaseFailedMessage(string message)
        {
            Debug.LogWarning($"Purchase Failed: {message}");
            // TODO: Show UI notification
        }

        // ==================== RESTORE PURCHASES ====================

        public void RestorePurchases()
        {
            if (testMode)
            {
                Debug.Log("[TEST MODE] Restore purchases");
                return;
            }

            // TODO: Restore non-consumable purchases
            // Example for iOS:
            // if (Application.platform == RuntimePlatform.IPhonePlayer)
            // {
            //     var apple = extensions.GetExtension<IAppleExtensions>();
            //     apple.RestoreTransactions((result) => {
            //         Debug.Log("Restore completed: " + result);
            //     });
            // }

            Debug.Log("Restoring purchases...");
        }

        // ==================== PRICE QUERIES ====================

        public string GetProductPrice(string productID)
        {
            if (testMode)
            {
                // Return fake prices for testing
                switch (productID)
                {
                    case GOLD_SMALL: return "$0.99";
                    case GOLD_MEDIUM: return "$4.99";
                    case GOLD_LARGE: return "$9.99";
                    case GOLD_MEGA: return "$49.99";
                    case AD_REMOVAL: return "$2.99";
                    case BATTLE_PASS: return "$4.99";
                    case STARTER_PACK: return "$4.99";
                    default: return "$0.99";
                }
            }

            // TODO: Get actual price from store
            // return storeController.products.WithID(productID).metadata.localizedPriceString;

            return "$0.99"; // Placeholder
        }
    }

    public enum GoldPackSize
    {
        Small,  // 10,000 gold
        Medium, // 50,000 gold
        Large,  // 200,000 gold
        Mega    // 1,000,000 gold
    }
}
