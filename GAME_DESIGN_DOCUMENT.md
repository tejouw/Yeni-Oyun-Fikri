# NEON SURVIVORS - Oyun Tasarım Dokümanı

## 🎮 OYUN KONSEPTI

**Tür:** Auto-Shooter + Idle Hybrid + Roguelite
**Platform:** Mobil (Android/iOS)
**Hedef Süre:** 10-15 dakikalık run'lar
**Grafik Stili:** Neon Minimal Geometric
**Monetizasyon:** Hybrid (Ads + IAP)

---

## 📋 CORE GAMEPLAY LOOP

### Temel Mekanik
1. **Player otomatik rotate eder ve auto-shoot yapar**
   - Oyuncu sadece hareket kontrolü yapar (joystick/swipe)
   - Silahlar otomatik ateş eder (en yakın düşmana)
   - Her level-up → 3 random upgrade seçeneği

2. **Wave-based düşman spawns**
   - Dalga dalga geometrik düşmanlar (cube, sphere, pyramid, cylinder)
   - Her 5 dakikada bir mini-boss
   - 15 dakika sonra inevitable death (difficulty infinitely scales)

3. **Power-up Synergy System**
   - 30+ upgrade türü
   - Combinations create powerful synergies
   - Örnek: Laser + Piercing + Bounce = screen-clearing beam

4. **Meta-Progression (Offline Idle)**
   - Run bitince gold kazanılır
   - Offline idle earnings (oyunda değilken auto-farm)
   - Permanent upgrades unlock et
   - New ships unlock et

---

## 🚀 SİSTEM TASARIMLARI

### 1. PLAYER SYSTEM

**Ship Types (15-20 unlockable):**
- **Starter Ship** (Triangle): Balanced stats
- **Tank Ship** (Pentagon): High HP, slow
- **Speed Ship** (Arrow): Fast, low HP
- **Laser Ship** (Diamond): Long-range focus
- **Swarm Ship** (Multi-triangle): Multiple weak projectiles

**Her ship unique passive ability:**
- Starter: +10% gold earning
- Tank: Damage reduction shield every 10 sec
- Speed: Dodge chance 15%
- Laser: Pierce through 2 enemies
- Swarm: +2 projectiles

### 2. WEAPON SYSTEM

**Base Weapons (her run başında seç):**
1. **Blaster** - Standard rapid fire
2. **Laser** - Continuous beam
3. **Missiles** - Homing projectiles
4. **Shotgun** - Multi-directional spread
5. **Orbit** - Rotating shields that damage

**Upgrade Categories:**
- **Damage** (+20% per level, 10 levels max)
- **Fire Rate** (+15% per level, 10 levels max)
- **Projectile Count** (+1 per level, 5 levels max)
- **Pierce** (penetrate +1 enemy, 5 levels max)
- **Bounce** (ricochet +1 time, 3 levels max)
- **Explosion** (AOE on hit, radius scales)
- **Chain Lightning** (jump to nearby enemies)
- **Freeze** (slow enemies 50%)
- **Burn** (DOT damage over 3 sec)

### 3. ENEMY SYSTEM

**Enemy Types (Geometric Shapes):**

| Type | Shape | HP | Speed | Behavior |
|------|-------|----|----|----------|
| Basic | Cube | 10 | Medium | Chase player |
| Fast | Tetrahedron | 5 | Fast | Zigzag movement |
| Tank | Sphere | 50 | Slow | Straight line |
| Shooter | Cylinder | 15 | Slow | Ranged attack |
| Splitter | Octahedron | 20 | Medium | Splits into 3 on death |
| Boss | Icosahedron | 500 | Slow | Bullet patterns |

**Difficulty Scaling:**
- HP multiplier: `baseHP × (1 + waveNumber × 0.15)`
- Spawn rate: `baseRate × (1 + waveNumber × 0.1)`
- Speed multiplier: `baseSpeed × (1 + waveNumber × 0.05)`

### 4. PROGRESSION SYSTEM

**During Run (Session-based):**
- Level up every 1000 XP
- XP earned by killing enemies
- Choose 1 of 3 random upgrades per level
- Max level 30 per run

**Meta Progression (Permanent):**

**Prestige System:**
- Reset all run progress
- Earn "Prestige Points" (PP)
- PP formula: `PP = 150 × √(totalGoldLifetime / 10^6)`
- PP unlocks permanent multipliers:
  - +5% damage per PP
  - +3% gold earning per PP
  - +2% movement speed per PP

**Workshop (Permanent Upgrades):**
- Starting HP: 100 → 500 (10 tiers, cost scales 1.5x)
- Starting Damage: +20% per tier (10 tiers)
- Starting Speed: +10% per tier (8 tiers)
- Pickup Range: +15% per tier (5 tiers)
- Gold Multiplier: +25% per tier (15 tiers)

**Ship Collection:**
- 15 ships unlockable
- Unlock conditions:
  - Reach wave 10 (unlock Tank)
  - Kill 10,000 enemies (unlock Speed)
  - Survive 20 minutes (unlock Laser)
  - Prestige 5 times (unlock Swarm)
  - Spend 100,000 gold (unlock Fortress)
  - etc.

### 5. IDLE SYSTEM

**Offline Earnings:**
- Oyuncu offline → auto-farming continues
- Earnings capped at 4 hours (encourage check-ins)
- Formula: `offlineGold = baseGPS × prestigeMultiplier × timeOffline × 0.5`
- baseGPS (gold per second) based on highest wave reached

**Claim Bonus:**
- Offline earnings claim edilince → option to watch ad for 3x multiplier
- 60-70% conversion rate expected

### 6. MONETIZATION SYSTEM

**Rewarded Ads:**
1. **Revive** (once per run): Continue from death with full HP
2. **Double Gold** (end of run): 2x all gold earned
3. **Bonus XP** (mid-run): +50% XP for 2 minutes
4. **Offline Multiplier**: 3x offline earnings claim
5. **Daily Bonus**: Spin wheel for rewards

**In-App Purchases:**
1. **Premium Ships** ($0.99 - $2.99 each)
   - Not pay-to-win, sidegrades
   - Unique abilities, same power level
2. **Starter Packs** ($4.99)
   - 3 premium ships
   - 5,000 gold
   - 10 prestige points
3. **Gold Packs** ($0.99 - $49.99)
   - Small: 10,000 gold
   - Medium: 50,000 gold
   - Large: 200,000 gold
   - Mega: 1,000,000 gold
4. **Ad Removal** ($2.99)
   - Remove interstitials
   - Keep rewarded ads (player choice)
5. **Battle Pass** ($4.99/month)
   - Free track: basic rewards
   - Premium track: exclusive ship + 10,000 gold + cosmetics

**Target Metrics:**
- ARPDAU: $1.50 - $3.00
- Ad revenue: 50-60%
- IAP revenue: 40-50%
- Conversion rate: 3-7%

---

## 🎨 GÖRSEL VE UI TASARIMI (100% KOD İLE)

### Visual Style
- **Neon geometric shapes** (Unity primitives)
- **Particle systems** (trails, explosions)
- **Gradient backgrounds** (procedural skybox)
- **Post-processing** (bloom, color grading)

### UI Elements (UI Toolkit ile kod)
1. **HUD:**
   - HP bar (top-left)
   - XP bar (bottom, full-width)
   - Wave counter (top-center)
   - Time survived (top-right)
   - Gold counter (top-right)

2. **Level-Up Screen:**
   - 3 upgrade cards
   - Each card shows: icon + name + description
   - Tap to select

3. **Death Screen:**
   - Stats recap (kills, time, gold earned)
   - Buttons: Restart, Upgrade, Home
   - Ad button: Watch for double gold

4. **Main Menu:**
   - Play button
   - Workshop button
   - Ships button
   - Settings button
   - Daily bonus button

5. **Workshop Screen:**
   - Scrollable upgrade list
   - Each item: icon + name + level + cost + buy button

6. **Ships Screen:**
   - Grid of ship icons
   - Locked/unlocked state
   - Tap to equip
   - Shows unlock condition

### Color Palette
- **Primary:** Neon Cyan (#00FFFF)
- **Secondary:** Neon Magenta (#FF00FF)
- **Accent:** Neon Yellow (#FFFF00)
- **Background:** Dark Purple Gradient (#0A0015 → #1A0030)
- **Enemy colors:** Red (#FF0040), Orange (#FF6600)

---

## 🔧 TEKNİK İMPLEMENTASYON PLANI

### Unity Versiyon
- **Unity 2022.3 LTS** (latest stable)

### Required Packages
1. **UI Toolkit** (built-in)
2. **Universal Render Pipeline (URP)** (built-in)
3. **Post-Processing** (Package Manager)
4. **TextMeshPro** (built-in)

### Asset Store / External
1. **DOTween Free** (tweening animations)
2. **Easy Save** ($45) - save system
3. **Mobile Monetization Pro** (~$50) - ads/IAP

### Folder Structure
```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── GameManager.cs
│   │   ├── PoolManager.cs
│   │   └── SaveManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerHealth.cs
│   │   ├── PlayerWeapon.cs
│   │   └── PlayerUpgrades.cs
│   ├── Enemy/
│   │   ├── EnemyBase.cs
│   │   ├── EnemySpawner.cs
│   │   └── EnemyTypes/
│   ├── Progression/
│   │   ├── LevelSystem.cs
│   │   ├── PrestigeManager.cs
│   │   ├── WorkshopManager.cs
│   │   └── IdleManager.cs
│   ├── UI/
│   │   ├── UIManager.cs
│   │   ├── HUDController.cs
│   │   ├── MenuController.cs
│   │   └── LevelUpScreen.cs
│   ├── Monetization/
│   │   ├── AdManager.cs
│   │   └── IAPManager.cs
│   └── Data/
│       ├── ScriptableObjects/
│       │   ├── ShipData.cs
│       │   ├── WeaponData.cs
│       │   ├── UpgradeData.cs
│       │   └── EnemyData.cs
│       └── GameData.cs
├── Resources/
│   └── Data/ (ScriptableObject instances)
├── Scenes/
│   ├── MainMenu.unity
│   └── GamePlay.unity
└── UI/
    └── UXML/ (UI Toolkit documents)
```

### Core Systems Breakdown

#### 1. Object Pooling System
```csharp
// Enemies ve projectiles için pooling
// Runtime instantiate ZERO - all pre-pooled
// Warm pool: 500 enemies, 1000 projectiles
```

#### 2. Procedural Mesh Generation
```csharp
// Tüm shapes runtime oluşturulur
// Cube, Sphere, Pyramid, Cylinder meshes
// MeshFilter + MeshRenderer + MeshCollider
```

#### 3. ScriptableObject Architecture
```csharp
// Designer-friendly data
// Yeni ship/weapon/upgrade → sadece ScriptableObject create
// Code change gerekmez
```

#### 4. Save System
```csharp
// Easy Save 3 kullanılacak
// Auto-save every 30 seconds
// Cloud save support (future)
// Data encryption enabled
```

#### 5. Dynamic UI Generation
```csharp
// UI Toolkit ile 100% runtime
// Workshop list → data-driven generation
// Ships grid → data-driven generation
// No manual UI placement in Unity Editor
```

---

## 📊 PROGRESSION MATH

### Gold Earning Formula
- Base gold per kill: `10 × (1 + waveNumber × 0.1)`
- Boss gold: `500 × (1 + waveNumber × 0.5)`
- Run completion bonus: `waveNumber² × 50`

### Upgrade Cost Scaling
- Workshop upgrades: `baseCost × (1.5 ^ currentLevel)`
- Example: HP upgrade costs 100, 150, 225, 337, 505...

### Prestige Requirements
- First prestige: 100,000 lifetime gold
- Each prestige: previous × 2.5
- Formula ensures 30-60 min between prestiges

### XP Earning
- Base XP per kill: `waveNumber × 10`
- Level-up XP requirement: `1000 × (1.1 ^ currentLevel)`

---

## 🎯 DEVELOPMENt ROADMAP

### Week 1-2: Foundation
- [x] Unity project setup
- [ ] Core gameplay loop (player movement + auto-shoot)
- [ ] Basic enemy spawning
- [ ] Object pooling system
- [ ] Procedural mesh generation (geometric shapes)

### Week 3-4: Combat & Upgrades
- [ ] Weapon system (5 base weapons)
- [ ] Upgrade system (30+ upgrades)
- [ ] Level-up screen
- [ ] Synergy mechanics
- [ ] Particle effects (neon trails, explosions)

### Week 5-6: Meta-Progression
- [ ] Gold earning system
- [ ] Workshop (permanent upgrades)
- [ ] Prestige system
- [ ] Ship collection (15 ships)
- [ ] Idle/offline earning system

### Week 7-8: Monetization
- [ ] Ad integration (rewarded + interstitials)
- [ ] IAP integration (ships, gold, battle pass)
- [ ] Analytics integration (Unity Analytics)
- [ ] Daily bonus wheel
- [ ] Battle Pass scaffolding

### Week 9-10: Content & Polish
- [ ] All 15 ships balanced
- [ ] 30+ upgrades implemented
- [ ] Enemy variety (8+ types)
- [ ] Boss encounters (3+ types)
- [ ] Post-processing effects

### Week 11-12: Testing & Optimization
- [ ] Mobile profiling (60 FPS target)
- [ ] Difficulty balancing
- [ ] Retention hooks testing
- [ ] Monetization placement testing
- [ ] Soft launch preparation

---

## 📈 SUCCESS METRICS

### Retention Targets
- **D1:** 40-45%
- **D7:** 18-22%
- **D30:** 8-12%

### Monetization Targets
- **ARPDAU:** $1.50-$3.00
- **Conversion Rate:** 3-7%
- **Ad eCPM:** $12-$30
- **Rewarded Ad Completion:** 80%+

### Engagement Targets
- **Session Length:** 12-18 minutes avg
- **Sessions/Day:** 3-5
- **Daily Ad Views:** 8-12 per DAU

---

## 🚀 LAUNCH STRATEGY

### Soft Launch Markets
1. Canada
2. Australia
3. Philippines

### UA Strategy
- TikTok ads (Gen Z targeting)
- Facebook/Instagram (25-40 demographic)
- Google UAC (broad targeting)
- CPI target: $0.50-$1.50

### Live-Ops Post-Launch
- Weekly events (2x gold weekends)
- New ship releases (monthly)
- Seasonal Battle Passes
- Community challenges

---

## ✅ DEFINITION OF DONE

Oyun şu kriterleri karşılamalı:

1. ✅ Unity'de Play → instant playable
2. ✅ Zero manual UI/level design required
3. ✅ 100% procedural generation
4. ✅ 15+ ships unlockable
5. ✅ 30+ upgrades working with synergies
6. ✅ Meta-progression (prestige + workshop)
7. ✅ Idle earnings functional
8. ✅ Ads + IAP integrated
9. ✅ Save/load system functional
10. ✅ 60 FPS on mid-range Android devices
11. ✅ Infinite replayability
12. ✅ Satisfying neon visual feedback
13. ✅ Daily login rewards
14. ✅ Battle Pass structure ready

---

## 🎮 BEN (KULLANICI) NEDEN HİÇBİR ŞEY YAPMAK ZORUNDA DEĞİLİM?

Çünkü:

1. **Grafikler → Procedural meshes** (kod ile oluşturulur)
2. **UI → UI Toolkit** (UXML/USS kod ile generate edilir)
3. **Levels → Random generation** (manuel design gereksiz)
4. **Prefabs → Runtime instantiation** (pooling ile)
5. **Balance → ScriptableObjects** (data-driven, JSON gibi düzenlenebilir)
6. **Content → Mathematical formulas** (infinite scaling)

**Tek yapman gereken:** Git clone → Unity'de aç → Play

---

## 📝 NOTLAR

- **Vampire Survivors** core formula kullanılıyor ama differentiation:
  - Idle earnings (VS'de yok)
  - Ship collection (VS minimal)
  - Prestige system (VS'de weak)
  - Neon aesthetic (VS pixel art)

- **Block Blast** monetization inspiration:
  - Ad-driven model viable
  - Infinite play = high engagement
  - Rewarded ads strategic placement

- **Royal Match** meta-progression inspiration:
  - Deep collection systems
  - Live-ops events
  - Battle Pass integration

---

## 🎯 NEXT STEPS

1. ✅ Document oluşturuldu
2. [ ] Unity project initialize
3. [ ] Core gameplay prototype (Week 1-2)
4. [ ] Iteration based on playtesting

**Başlayalım! 🚀**
