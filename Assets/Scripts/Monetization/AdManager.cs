using UnityEngine;
using System;

namespace NeonSurvivors.Monetization
{
    /// <summary>
    /// Ad Manager - Placeholder for ad integration (AdMob, Unity Ads, IronSource, etc.)
    /// Replace with actual SDK implementation
    /// </summary>
    public class AdManager : MonoBehaviour
    {
        private static AdManager _instance;
        public static AdManager Instance => _instance;

        [Header("Ad Configuration")]
        public bool adsEnabled = true;
        public float interstitialCooldown = 180f; // 3 minutes between interstitials
        private float lastInterstitialTime = 0f;

        [Header("Test Mode")]
        public bool testMode = true;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAds();
        }

        void InitializeAds()
        {
            if (!adsEnabled)
            {
                Debug.Log("Ads disabled");
                return;
            }

            // TODO: Initialize actual ad SDK (AdMob, Unity Ads, etc.)
            // Example:
            // MobileAds.Initialize(initStatus => {
            //     Debug.Log("Ads initialized");
            // });

            Debug.Log("AdManager initialized (Placeholder)");
        }

        /// <summary>
        /// Shows a rewarded video ad
        /// </summary>
        public void ShowRewardedAd(string placement, Action<bool> onComplete)
        {
            if (!adsEnabled)
            {
                Debug.Log("Ads disabled, giving reward anyway");
                onComplete?.Invoke(true);
                return;
            }

            if (testMode)
            {
                Debug.Log($"[TEST MODE] Showing rewarded ad for: {placement}");
                // Simulate watching ad
                onComplete?.Invoke(true);
                return;
            }

            // TODO: Show actual rewarded video
            // Example:
            // rewardedAd.Show((success) => {
            //     onComplete?.Invoke(success);
            // });

            Debug.Log($"Showing rewarded ad for: {placement}");
            onComplete?.Invoke(true);
        }

        /// <summary>
        /// Shows an interstitial ad (with cooldown)
        /// </summary>
        public void ShowInterstitial(string placement)
        {
            if (!adsEnabled)
            {
                Debug.Log("Ads disabled");
                return;
            }

            // Check cooldown
            if (Time.time - lastInterstitialTime < interstitialCooldown)
            {
                Debug.Log($"Interstitial on cooldown ({interstitialCooldown - (Time.time - lastInterstitialTime):F0}s remaining)");
                return;
            }

            if (testMode)
            {
                Debug.Log($"[TEST MODE] Showing interstitial ad for: {placement}");
                lastInterstitialTime = Time.time;
                return;
            }

            // TODO: Show actual interstitial
            // Example:
            // interstitialAd.Show();

            Debug.Log($"Showing interstitial ad for: {placement}");
            lastInterstitialTime = Time.time;
        }

        /// <summary>
        /// Shows banner ad
        /// </summary>
        public void ShowBanner(BannerPosition position = BannerPosition.Bottom)
        {
            if (!adsEnabled)
            {
                Debug.Log("Ads disabled");
                return;
            }

            if (testMode)
            {
                Debug.Log($"[TEST MODE] Showing banner ad at: {position}");
                return;
            }

            // TODO: Show actual banner
            // Example:
            // bannerAd.Show(position);

            Debug.Log($"Showing banner ad at: {position}");
        }

        /// <summary>
        /// Hides banner ad
        /// </summary>
        public void HideBanner()
        {
            if (testMode)
            {
                Debug.Log("[TEST MODE] Hiding banner ad");
                return;
            }

            // TODO: Hide actual banner
            // bannerAd.Hide();

            Debug.Log("Hiding banner ad");
        }

        /// <summary>
        /// Check if rewarded ad is ready
        /// </summary>
        public bool IsRewardedAdReady()
        {
            if (testMode)
                return true;

            // TODO: Check if actual ad is loaded
            // return rewardedAd.IsLoaded();

            return true; // Placeholder
        }

        /// <summary>
        /// Disable ads (IAP purchase)
        /// </summary>
        public void DisableAds()
        {
            adsEnabled = false;
            HideBanner();
            Debug.Log("Ads permanently disabled");

            // Save this state
            PlayerPrefs.SetInt("AdsRemoved", 1);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Check if ads have been removed
        /// </summary>
        public bool AreAdsRemoved()
        {
            return PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
        }
    }

    public enum BannerPosition
    {
        Top,
        Bottom
    }
}
