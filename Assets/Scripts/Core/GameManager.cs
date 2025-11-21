using UnityEngine;
using System;

namespace NeonSurvivors.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [Header("Game State")]
        public GameState currentState = GameState.MainMenu;
        public bool isGameRunning = false;

        [Header("Currency")]
        public long currentGold = 0;
        public int currentGems = 0;

        [Header("Run Stats")]
        public int currentWave = 0;
        public int totalKills = 0;
        public int goldEarnedThisRun = 0;
        public float survivalTime = 0f;

        [Header("Events")]
        public event Action<long> OnGoldChanged;
        public event Action<int> OnGemsChanged;
        public event Action<int> OnWaveChanged;
        public event Action OnGameStart;
        public event Action OnGameEnd;

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
            // Load saved game data
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.LoadGame();
            }
        }

        void Update()
        {
            if (isGameRunning)
            {
                survivalTime += Time.deltaTime;

                // Update wave based on time (wave every 60 seconds)
                int newWave = Mathf.FloorToInt(survivalTime / 60f);
                if (newWave != currentWave)
                {
                    SetWave(newWave);
                }
            }
        }

        public void StartGame()
        {
            isGameRunning = true;
            currentState = GameState.Playing;

            // Reset run stats
            currentWave = 0;
            totalKills = 0;
            goldEarnedThisRun = 0;
            survivalTime = 0f;

            OnGameStart?.Invoke();

            Debug.Log("Game Started!");
        }

        public void EndGame()
        {
            isGameRunning = false;
            currentState = GameState.GameOver;

            // Record stats
            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.RecordGameEnd(currentWave, totalKills, survivalTime, goldEarnedThisRun);
            }

            // Add run gold to total
            AddGold(goldEarnedThisRun);

            OnGameEnd?.Invoke();

            Debug.Log($"Game Ended! Survived: {survivalTime:F1}s, Kills: {totalKills}, Gold: {goldEarnedThisRun}");
        }

        public void RestartGame()
        {
            // Reload scene would go here
            StartGame();
        }

        public void AddGold(long amount)
        {
            currentGold += amount;
            OnGoldChanged?.Invoke(currentGold);
        }

        public void AddGoldThisRun(int amount)
        {
            goldEarnedThisRun += amount;
        }

        public bool SpendGold(long amount)
        {
            if (currentGold >= amount)
            {
                currentGold -= amount;
                OnGoldChanged?.Invoke(currentGold);
                return true;
            }
            return false;
        }

        public void AddGems(int amount)
        {
            currentGems += amount;
            OnGemsChanged?.Invoke(currentGems);
        }

        public bool SpendGems(int amount)
        {
            if (currentGems >= amount)
            {
                currentGems -= amount;
                OnGemsChanged?.Invoke(currentGems);
                return true;
            }
            return false;
        }

        public void AddKill()
        {
            totalKills++;
        }

        public void SetWave(int wave)
        {
            currentWave = wave;
            OnWaveChanged?.Invoke(currentWave);
            Debug.Log($"Wave {currentWave}");
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }

        void OnApplicationQuit()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGame();
            }
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Shop
    }
}
