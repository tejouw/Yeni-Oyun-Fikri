using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using NeonSurvivors.Data;
using NeonSurvivors.Core;
using NeonSurvivors.Player;

namespace NeonSurvivors
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        [Header("UI Document")]
        public UIDocument uiDocument;
        private VisualElement root;

        [Header("Screens")]
        private VisualElement hudScreen;
        private VisualElement levelUpScreen;
        private VisualElement deathScreen;
        private VisualElement offlineEarningsScreen;

        [Header("HUD Elements")]
        private Label goldLabel;
        private Label waveLabel;
        private Label timeLabel;
        private VisualElement healthBar;
        private VisualElement xpBar;

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

        void Start()
        {
            InitializeUI();
            SubscribeToEvents();
        }

        void InitializeUI()
        {
            // Get or create UIDocument
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }

            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }

            root = uiDocument.rootVisualElement;
            root.style.width = Length.Percent(100);
            root.style.height = Length.Percent(100);

            // Create screens
            CreateHUD();
            CreateLevelUpScreen();
            CreateDeathScreen();
            CreateOfflineEarningsScreen();

            // Show HUD by default
            ShowHUD();
        }

        void CreateHUD()
        {
            hudScreen = new VisualElement();
            hudScreen.name = "HUDScreen";
            hudScreen.style.width = Length.Percent(100);
            hudScreen.style.height = Length.Percent(100);
            hudScreen.style.display = DisplayStyle.None;

            // Top bar container
            VisualElement topBar = new VisualElement();
            topBar.style.flexDirection = FlexDirection.Row;
            topBar.style.justifyContent = Justify.SpaceBetween;
            topBar.style.paddingTop = 20;
            topBar.style.paddingLeft = 20;
            topBar.style.paddingRight = 20;

            // Wave label
            waveLabel = new Label("Wave: 0");
            waveLabel.style.fontSize = 24;
            waveLabel.style.color = Color.cyan;
            waveLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            topBar.Add(waveLabel);

            // Time label
            timeLabel = new Label("0:00");
            timeLabel.style.fontSize = 24;
            timeLabel.style.color = Color.white;
            topBar.Add(timeLabel);

            // Gold label
            goldLabel = new Label("Gold: 0");
            goldLabel.style.fontSize = 24;
            goldLabel.style.color = Color.yellow;
            goldLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            topBar.Add(goldLabel);

            hudScreen.Add(topBar);

            // Health bar (top left)
            VisualElement healthContainer = new VisualElement();
            healthContainer.style.position = Position.Absolute;
            healthContainer.style.top = 60;
            healthContainer.style.left = 20;
            healthContainer.style.width = 200;
            healthContainer.style.height = 20;
            healthContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            healthBar = new VisualElement();
            healthBar.style.width = Length.Percent(100);
            healthBar.style.height = Length.Percent(100);
            healthBar.style.backgroundColor = new Color(1f, 0.2f, 0.2f);
            healthContainer.Add(healthBar);

            hudScreen.Add(healthContainer);

            // XP bar (bottom, full width)
            VisualElement xpContainer = new VisualElement();
            xpContainer.style.position = Position.Absolute;
            xpContainer.style.bottom = 20;
            xpContainer.style.left = Length.Percent(10);
            xpContainer.style.width = Length.Percent(80);
            xpContainer.style.height = 15;
            xpContainer.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            xpBar = new VisualElement();
            xpBar.style.width = Length.Percent(0);
            xpBar.style.height = Length.Percent(100);
            xpBar.style.backgroundColor = Color.cyan;
            xpContainer.Add(xpBar);

            hudScreen.Add(xpContainer);

            root.Add(hudScreen);
        }

        void CreateLevelUpScreen()
        {
            levelUpScreen = new VisualElement();
            levelUpScreen.name = "LevelUpScreen";
            levelUpScreen.style.width = Length.Percent(100);
            levelUpScreen.style.height = Length.Percent(100);
            levelUpScreen.style.backgroundColor = new Color(0, 0, 0, 0.9f);
            levelUpScreen.style.display = DisplayStyle.None;
            levelUpScreen.style.alignItems = Align.Center;
            levelUpScreen.style.justifyContent = Justify.Center;

            // Title
            Label title = new Label("LEVEL UP!");
            title.style.fontSize = 48;
            title.style.color = Color.cyan;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 40;
            levelUpScreen.Add(title);

            // Cards container
            VisualElement cardsContainer = new VisualElement();
            cardsContainer.name = "CardsContainer";
            cardsContainer.style.flexDirection = FlexDirection.Row;
            levelUpScreen.Add(cardsContainer);

            root.Add(levelUpScreen);
        }

        void CreateDeathScreen()
        {
            deathScreen = new VisualElement();
            deathScreen.name = "DeathScreen";
            deathScreen.style.width = Length.Percent(100);
            deathScreen.style.height = Length.Percent(100);
            deathScreen.style.backgroundColor = new Color(0.1f, 0, 0, 0.9f);
            deathScreen.style.display = DisplayStyle.None;
            deathScreen.style.alignItems = Align.Center;
            deathScreen.style.justifyContent = Justify.Center;

            // Title
            Label title = new Label("GAME OVER");
            title.style.fontSize = 60;
            title.style.color = Color.red;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 40;
            deathScreen.Add(title);

            // Stats container
            VisualElement statsContainer = new VisualElement();
            statsContainer.name = "StatsContainer";
            deathScreen.Add(statsContainer);

            // Restart button
            Button restartButton = new Button(() => RestartGame());
            restartButton.text = "RESTART";
            restartButton.style.width = 200;
            restartButton.style.height = 50;
            restartButton.style.marginTop = 40;
            restartButton.style.fontSize = 24;
            deathScreen.Add(restartButton);

            root.Add(deathScreen);
        }

        void CreateOfflineEarningsScreen()
        {
            offlineEarningsScreen = new VisualElement();
            offlineEarningsScreen.name = "OfflineEarningsScreen";
            offlineEarningsScreen.style.width = Length.Percent(100);
            offlineEarningsScreen.style.height = Length.Percent(100);
            offlineEarningsScreen.style.backgroundColor = new Color(0, 0, 0, 0.95f);
            offlineEarningsScreen.style.display = DisplayStyle.None;
            offlineEarningsScreen.style.alignItems = Align.Center;
            offlineEarningsScreen.style.justifyContent = Justify.Center;

            // Title
            Label title = new Label("WELCOME BACK!");
            title.style.fontSize = 48;
            title.style.color = Color.cyan;
            title.style.marginBottom = 20;
            offlineEarningsScreen.Add(title);

            // Earnings label
            Label earningsLabel = new Label();
            earningsLabel.name = "EarningsLabel";
            earningsLabel.style.fontSize = 36;
            earningsLabel.style.color = Color.yellow;
            offlineEarningsScreen.Add(earningsLabel);

            // Buttons container
            VisualElement buttonsContainer = new VisualElement();
            buttonsContainer.style.flexDirection = FlexDirection.Row;
            buttonsContainer.style.marginTop = 40;

            Button claimButton = new Button(() => ClaimOfflineEarnings(false));
            claimButton.text = "CLAIM";
            claimButton.style.width = 150;
            claimButton.style.height = 50;
            claimButton.style.marginRight = 20;
            buttonsContainer.Add(claimButton);

            Button claimAdButton = new Button(() => ClaimOfflineEarnings(true));
            claimAdButton.text = "CLAIM 3X (AD)";
            claimAdButton.style.width = 150;
            claimAdButton.style.height = 50;
            buttonsContainer.Add(claimAdButton);

            offlineEarningsScreen.Add(buttonsContainer);

            root.Add(offlineEarningsScreen);
        }

        void SubscribeToEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGoldChanged += UpdateGoldDisplay;
                GameManager.Instance.OnWaveChanged += UpdateWaveDisplay;
                GameManager.Instance.OnGameEnd += ShowDeathScreen;
            }
        }

        void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameRunning)
            {
                UpdateTimeDisplay();
            }
        }

        // ==================== HUD UPDATES ====================

        void UpdateGoldDisplay(long gold)
        {
            if (goldLabel != null)
            {
                goldLabel.text = $"Gold: {gold}";
            }
        }

        void UpdateWaveDisplay(int wave)
        {
            if (waveLabel != null)
            {
                waveLabel.text = $"Wave: {wave}";
            }
        }

        void UpdateTimeDisplay()
        {
            if (timeLabel != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.survivalTime;
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeLabel.text = $"{minutes}:{seconds:00}";
            }
        }

        public void UpdateHealthBar(float current, float max)
        {
            if (healthBar != null)
            {
                float percentage = (current / max) * 100f;
                healthBar.style.width = Length.Percent(percentage);
            }
        }

        public void UpdateXPBar(int current, int required)
        {
            if (xpBar != null)
            {
                float percentage = ((float)current / required) * 100f;
                xpBar.style.width = Length.Percent(percentage);
            }
        }

        // ==================== SCREEN MANAGEMENT ====================

        public void ShowHUD()
        {
            if (hudScreen != null)
                hudScreen.style.display = DisplayStyle.Flex;

            HideOtherScreens(hudScreen);
        }

        public void ShowLevelUpScreen(List<UpgradeData> upgrades)
        {
            if (levelUpScreen == null)
                return;

            levelUpScreen.style.display = DisplayStyle.Flex;

            // Clear previous cards
            VisualElement cardsContainer = levelUpScreen.Q<VisualElement>("CardsContainer");
            cardsContainer.Clear();

            // Create upgrade cards
            foreach (var upgrade in upgrades)
            {
                Button card = CreateUpgradeCard(upgrade);
                cardsContainer.Add(card);
            }

            HideOtherScreens(levelUpScreen);
        }

        Button CreateUpgradeCard(UpgradeData upgrade)
        {
            Button card = new Button(() => OnUpgradeCardClicked(upgrade));
            card.style.width = 250;
            card.style.height = 350;
            card.style.marginLeft = 20;
            card.style.marginRight = 20;
            card.style.backgroundColor = GetRarityColor(upgrade.rarity);

            // Name
            Label nameLabel = new Label(upgrade.upgradeName);
            nameLabel.style.fontSize = 24;
            nameLabel.style.color = Color.white;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.marginTop = 20;
            card.Add(nameLabel);

            // Description
            Label descLabel = new Label(upgrade.GetFormattedDescription());
            descLabel.style.fontSize = 16;
            descLabel.style.color = Color.white;
            descLabel.style.marginTop = 20;
            descLabel.style.whiteSpace = WhiteSpace.Normal;
            descLabel.style.paddingLeft = 10;
            descLabel.style.paddingRight = 10;
            card.Add(descLabel);

            return card;
        }

        Color GetRarityColor(UpgradeRarity rarity)
        {
            switch (rarity)
            {
                case UpgradeRarity.Common: return new Color(0.5f, 0.5f, 0.5f, 0.8f);
                case UpgradeRarity.Uncommon: return new Color(0.2f, 0.8f, 0.2f, 0.8f);
                case UpgradeRarity.Rare: return new Color(0.2f, 0.4f, 1f, 0.8f);
                case UpgradeRarity.Epic: return new Color(0.8f, 0.2f, 0.8f, 0.8f);
                default: return Color.gray;
            }
        }

        void OnUpgradeCardClicked(UpgradeData upgrade)
        {
            if (LevelSystem.Instance != null)
            {
                LevelSystem.Instance.OnUpgradeSelected(upgrade);
            }

            levelUpScreen.style.display = DisplayStyle.None;
            ShowHUD();
        }

        public void ShowDeathScreen()
        {
            if (deathScreen == null)
                return;

            deathScreen.style.display = DisplayStyle.Flex;

            // Update stats
            VisualElement statsContainer = deathScreen.Q<VisualElement>("StatsContainer");
            statsContainer.Clear();

            if (GameManager.Instance != null)
            {
                AddStatLabel(statsContainer, $"Survived: {GameManager.Instance.survivalTime:F1}s");
                AddStatLabel(statsContainer, $"Wave: {GameManager.Instance.currentWave}");
                AddStatLabel(statsContainer, $"Kills: {GameManager.Instance.totalKills}");
                AddStatLabel(statsContainer, $"Gold Earned: {GameManager.Instance.goldEarnedThisRun}");
            }

            HideOtherScreens(deathScreen);
        }

        void AddStatLabel(VisualElement container, string text)
        {
            Label label = new Label(text);
            label.style.fontSize = 24;
            label.style.color = Color.white;
            label.style.marginTop = 10;
            container.Add(label);
        }

        public void ShowOfflineEarnings(int goldEarned, float hoursOffline)
        {
            if (offlineEarningsScreen == null)
                return;

            offlineEarningsScreen.style.display = DisplayStyle.Flex;

            Label earningsLabel = offlineEarningsScreen.Q<Label>("EarningsLabel");
            if (earningsLabel != null)
            {
                earningsLabel.text = $"You earned {goldEarned} gold\nwhile offline for {hoursOffline:F1} hours!";
            }

            HideOtherScreens(offlineEarningsScreen);
        }

        void HideOtherScreens(VisualElement activeScreen)
        {
            if (hudScreen != activeScreen)
                hudScreen.style.display = DisplayStyle.None;
            if (levelUpScreen != activeScreen)
                levelUpScreen.style.display = DisplayStyle.None;
            if (deathScreen != activeScreen)
                deathScreen.style.display = DisplayStyle.None;
            if (offlineEarningsScreen != activeScreen)
                offlineEarningsScreen.style.display = DisplayStyle.None;
        }

        // ==================== ACTIONS ====================

        void RestartGame()
        {
            deathScreen.style.display = DisplayStyle.None;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
            ShowHUD();
        }

        void ClaimOfflineEarnings(bool watchedAd)
        {
            if (IdleManager.Instance != null)
            {
                IdleManager.Instance.ClaimOfflineEarnings(watchedAd);
            }

            offlineEarningsScreen.style.display = DisplayStyle.None;
            ShowHUD();
        }

        // ==================== PLACEHOLDER METHODS ====================

        public void ShowPrestigeRewards(int ppEarned)
        {
            Debug.Log($"Prestige rewards UI: Earned {ppEarned} PP");
            // TODO: Create prestige rewards UI
        }

        public void ShowShipUnlockNotification(ShipData ship)
        {
            Debug.Log($"Ship unlocked: {ship.shipName}");
            // TODO: Create ship unlock notification
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
                GameManager.Instance.OnWaveChanged -= UpdateWaveDisplay;
                GameManager.Instance.OnGameEnd -= ShowDeathScreen;
            }
        }
    }
}
