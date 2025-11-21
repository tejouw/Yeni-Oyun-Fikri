using UnityEngine;
using System.Collections.Generic;
using NeonSurvivors.Core;

namespace NeonSurvivors.Audio
{
    /// <summary>
    /// AudioManager - Centralized Audio System
    /// Handles all sound effects and background music
    /// This is a placeholder implementation for future audio integration
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance => _instance;

        [Header("Audio Sources")]
        private AudioSource musicSource;
        private List<AudioSource> sfxSources = new List<AudioSource>();
        private int sfxPoolSize = 10;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        [Range(0f, 1f)]
        public float sfxVolume = 0.8f;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        void InitializeAudioSources()
        {
            // Create dedicated music source
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume * masterVolume;

            // Create pooled SFX sources
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject($"SFXSource_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume * masterVolume;
                sfxSources.Add(sfxSource);
            }

            Debug.Log($"AudioManager initialized with {sfxPoolSize} SFX sources");
        }

        // ==================== PLACEHOLDER AUDIO METHODS ====================

        /// <summary>
        /// Plays background music
        /// TODO: Load and play music AudioClip
        /// </summary>
        public void PlayMusic(string musicName)
        {
            Debug.Log($"Audio: Playing music '{musicName}'");
            // TODO: Load AudioClip from Resources or Addressables
            // AudioClip clip = Resources.Load<AudioClip>($"Audio/Music/{musicName}");
            // musicSource.clip = clip;
            // musicSource.Play();
        }

        /// <summary>
        /// Stops background music
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
                Debug.Log("Audio: Music stopped");
            }
        }

        /// <summary>
        /// Plays a sound effect
        /// TODO: Load and play SFX AudioClip
        /// </summary>
        public void PlaySFX(string sfxName, float volumeScale = 1f)
        {
            Debug.Log($"Audio: Playing SFX '{sfxName}' at volume {volumeScale}");
            // TODO: Find available AudioSource from pool
            // AudioSource source = GetAvailableSFXSource();
            // AudioClip clip = Resources.Load<AudioClip>($"Audio/SFX/{sfxName}");
            // source.PlayOneShot(clip, volumeScale * sfxVolume * masterVolume);
        }

        /// <summary>
        /// Plays a positional 3D sound effect
        /// TODO: Implement spatial audio
        /// </summary>
        public void PlaySFX3D(string sfxName, Vector3 position, float volumeScale = 1f)
        {
            Debug.Log($"Audio: Playing 3D SFX '{sfxName}' at {position}");
            // TODO: Create temporary AudioSource at position or use pooled 3D sources
        }

        // ==================== GAME EVENT SOUNDS ====================

        public void PlayPlayerShoot()
        {
            PlaySFX("player_shoot", 0.5f);
        }

        public void PlayEnemyHit()
        {
            PlaySFX("enemy_hit", 0.6f);
        }

        public void PlayEnemyDeath()
        {
            PlaySFX("enemy_death", 0.7f);
        }

        public void PlayExplosion()
        {
            PlaySFX("explosion", 0.8f);
        }

        public void PlayLevelUp()
        {
            PlaySFX("level_up", 1f);
        }

        public void PlayUpgradeSelect()
        {
            PlaySFX("upgrade_select", 0.6f);
        }

        public void PlayGoldPickup()
        {
            PlaySFX("gold_pickup", 0.4f);
        }

        public void PlayPlayerDamage()
        {
            PlaySFX("player_damage", 0.7f);
        }

        public void PlayPlayerDeath()
        {
            PlaySFX("player_death", 1f);
        }

        public void PlayUIClick()
        {
            PlaySFX("ui_click", 0.5f);
        }

        public void PlayFreeze()
        {
            PlaySFX("freeze", 0.6f);
        }

        public void PlayBurn()
        {
            PlaySFX("burn", 0.5f);
        }

        public void PlayChainLightning()
        {
            PlaySFX("chain_lightning", 0.7f);
        }

        // ==================== VOLUME CONTROL ====================

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }

        void UpdateAllVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }

            foreach (var source in sfxSources)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        AudioSource GetAvailableSFXSource()
        {
            // Find first available (not playing) source
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            // All sources busy, return first one (will interrupt)
            return sfxSources[0];
        }

        // ==================== FUTURE IMPLEMENTATION NOTES ====================

        /*
         * RECOMMENDED IMPLEMENTATION APPROACH:
         *
         * 1. Audio Asset Sourcing:
         *    - Use free asset packs from Unity Asset Store
         *    - Freesound.org for CC-licensed sounds
         *    - Generate with tools like BFXR, SFXR, or ChipTone
         *
         * 2. Recommended SFX List:
         *    - Shooting: Short laser/pew sound (50-100ms)
         *    - Enemy death: Explosion/pop (200-300ms)
         *    - Level up: Ascending chime (500ms)
         *    - Gold pickup: Coin clink (100ms)
         *    - UI click: Soft click (50ms)
         *    - Explosion: Deep boom (500ms)
         *    - Freeze: Ice crack (200ms)
         *    - Burn: Fire whoosh (300ms)
         *    - Chain lightning: Electric zap (200ms)
         *
         * 3. Background Music:
         *    - Synthwave/retrowave loop (2-3 minutes)
         *    - High energy, ~120-140 BPM
         *    - Seamless loop point
         *
         * 4. Performance:
         *    - Compress audio: Vorbis for music, PCM for short SFX
         *    - Load mode: Streaming for music, Compressed in memory for SFX
         *    - Pool size: 10-20 AudioSources sufficient
         *
         * 5. Integration Points:
         *    - GameManager: Background music start/stop
         *    - PlayerWeapon: Shooting sounds
         *    - EnemyBase: Hit and death sounds
         *    - UIManager: UI interaction sounds
         *    - LevelSystem: Level up sound
         *
         * 6. Free Asset Recommendations:
         *    - "Sci-Fi SFX" pack (Unity Asset Store)
         *    - "Retro Sounds" pack
         *    - Generate custom with BFXR (browser-based, free)
         */
    }
}
