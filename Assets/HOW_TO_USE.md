# Neon Survivors - Unity Setup Guide

## 🚀 Quick Start

### Step 1: Unity Setup
1. Open Unity Hub
2. Create new project with Unity 2022.3 LTS or newer
3. Select **3D (URP)** template
4. Copy all files from `Assets/` folder to your Unity project's `Assets/` folder

### Step 2: Scene Setup
1. Create new scene called `GamePlay`
2. Create empty GameObject named `GameSetup`
3. Add `GameSetup.cs` script to it
4. Press Play!

That's it! The game will auto-create all managers and start running.

---

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/              # Core managers (GameManager, PoolManager, etc.)
│   ├── Player/            # Player controller, health, weapon, upgrades
│   ├── Enemy/             # Enemy base, spawner, AI
│   ├── Weapons/           # Projectile system
│   ├── Progression/       # Level, Prestige, Workshop, Idle, Ships
│   ├── UI/                # UIManager (UI Toolkit)
│   ├── Monetization/      # AdManager, IAPManager
│   ├── Data/              # ScriptableObject definitions
│   └── Utilities/         # ProceduralMeshGenerator
│
└── Resources/
    └── Data/              # ScriptableObject instances (ships, enemies, upgrades)
```

---

## 🎮 How It Works

### Automatic Initialization
`GameSetup.cs` automatically creates all required managers on game start:
- GameManager
- PoolManager
- SaveManager
- LevelSystem
- ProgressionManager
- WorkshopManager
- IdleManager
- ShipManager
- UIManager
- AdManager
- IAPManager

### Player Creation
Player is created procedurally with:
- Movement controller (WASD or touch)
- Auto-shooting weapon
- Health system
- Upgrade system
- Procedural mesh based on ship type

### Enemy Spawning
`EnemySpawner` automatically spawns waves of enemies:
- Spawn rate increases with wave number
- Enemies scale in difficulty
- All enemies use object pooling

### UI System
UI is generated at runtime using UI Toolkit:
- HUD (health, XP, gold, wave, time)
- Level-up screen (3 random upgrades)
- Death screen (stats + restart)
- Offline earnings screen

---

## 🔧 Creating Game Data

### Creating a Ship
1. Right-click in Project window
2. Create → Neon Survivors → Ship Data
3. Fill in:
   - Ship ID (unique, e.g., "ship_tank")
   - Ship Name (display name)
   - Stats (health, speed, damage, etc.)
   - Mesh Type (Triangle, Pentagon, etc.)
   - Colors (primary, emission)
   - Ability (gold bonus, speed boost, etc.)
   - Unlock condition

4. Save to `Resources/Data/Ships/`

### Creating an Enemy
1. Create → Neon Survivors → Enemy Data
2. Fill in similar to Ship
3. Save to `Resources/Data/Enemies/`

### Creating an Upgrade
1. Create → Neon Survivors → Upgrade Data
2. Set modifiers:
   - Damage multiplier
   - Fire rate bonus
   - Projectile count
   - Special effects (pierce, bounce, explosion)
3. Set rarity (affects drop chance)
4. Save to `Resources/Data/Upgrades/`

---

## 🎨 Visual Customization

All visuals are generated procedurally via code. To change:

### Colors
Edit in ScriptableObject data:
- Ships: `primaryColor`, `emissionColor`
- Enemies: `enemyColor`
- UI: Edit `UIManager.cs` color values

### Shapes
Ships and enemies use procedural meshes:
- Triangle, Pentagon, Hexagon, Star, Arrow, Diamond
- Pyramid, Cube, Sphere, Cylinder, Octahedron, Icosahedron

All generated in `ProceduralMeshGenerator.cs`

### Neon Glow
Emission intensity can be adjusted:
```csharp
Material mat = ProceduralMeshGenerator.CreateNeonMaterial(color, 3f); // 3f = intensity
```

---

## 🎯 Gameplay Features

### Auto-Shooting
- Player auto-targets nearest enemy
- Fire rate determined by ship + upgrades
- Multiple projectiles with spread

### Level System
- Gain XP by killing enemies
- Level up every 1000 XP (scaled)
- Choose 1 of 3 random upgrades
- 30+ upgrade types with synergies

### Progression
- **Workshop:** Permanent stat upgrades (buy with gold)
- **Prestige:** Reset run for permanent multipliers
- **Ships:** Unlock 15+ ships with unique abilities
- **Idle:** Earn gold while offline (4-hour cap)

### Monetization (Placeholder)
- Rewarded ads: Revive, double gold, offline multiplier
- IAP: Premium ships, gold packs, ad removal
- Battle Pass (structure ready)

---

## ⚙️ Configuration

### Difficulty Scaling
Edit in `EnemyBase.Initialize()`:
```csharp
maxHealth = baseHealth * (1 + waveNumber * 0.15f);  // +15% HP per wave
moveSpeed = baseSpeed * (1 + waveNumber * 0.05f);   // +5% speed per wave
```

### Spawn Rate
Edit in `EnemySpawner.cs`:
```csharp
baseSpawnRate = 2f; // Enemies per second
```

### Gold Earning
Edit in `EnemyData`:
```csharp
goldValue = 10; // Base gold per enemy
```

### Upgrade Drop Rates
Edit in `LevelSystem.SelectWeightedRandomUpgrade()`:
- Common: 60%
- Uncommon: 25%
- Rare: 12%
- Epic: 3%

---

## 🐛 Troubleshooting

### "No ship selected" warning
- Create a ShipData ScriptableObject
- Set shipID to "ship_starter"
- Save to Resources/Data/Ships/

### Player not shooting
- Ensure enemies have tag "Enemy"
- Check PlayerWeapon.targetRange (default: 20)

### UI not showing
- UIManager creates UI at runtime
- Check Console for errors
- Ensure UIDocument component exists on UIManager GameObject

### Enemies not spawning
- Create EnemyData ScriptableObjects
- Add to EnemySpawner.enemyTypes list
- Or let GameSetup create defaults

### Black screen
- Check Camera setup (should be top-down orthographic)
- Position: (0, 15, 0)
- Rotation: (90, 0, 0)

---

## 📊 Performance Tips

### Mobile Optimization
1. Target 60 FPS (set in GameSetup)
2. Object pooling prevents GC spikes
3. Procedural meshes are lightweight
4. URP provides good mobile performance

### Pool Sizes
Adjust in GameSetup or PoolManager:
```csharp
Enemy pools: 100 each
Projectile pool: 500
```

Increase if seeing instantiation warnings.

---

## 🚀 Next Steps

### Essential Tasks
1. Create ship ScriptableObjects (at least starter ship)
2. Create enemy ScriptableObjects (3-5 types)
3. Create upgrade ScriptableObjects (10-20 types)
4. Test gameplay loop
5. Balance difficulty

### Optional Enhancements
1. Add particle effects (pooled)
2. Add sound effects
3. Add background music
4. Implement Battle Pass
5. Connect real ads/IAP SDKs
6. Add more ships (goal: 15+)
7. Add boss encounters
8. Add daily missions

---

## 📝 Notes

### Why No Prefabs?
- Everything is generated procedurally via code
- Easier to iterate and balance
- Smaller build size
- No manual Unity Editor work required

### Data-Driven Design
- All content defined in ScriptableObjects
- Designers can create content without touching code
- Easy to balance and tweak

### Save System
- Auto-saves every 30 seconds
- Saves on app pause/quit
- Uses PlayerPrefs (can upgrade to Easy Save 3)

---

## 🎮 Controls

### Desktop (Testing)
- **WASD:** Move
- **Auto-shoot:** Automatic

### Mobile
- **Touch & Drag:** Move towards touch point
- **Auto-shoot:** Automatic

---

## ✅ Checklist

Before first run:
- [ ] Copy all scripts to Unity project
- [ ] Create GamePlay scene
- [ ] Add GameSetup script to GameObject
- [ ] Press Play
- [ ] Game should auto-initialize

For full experience:
- [ ] Create ShipData for starter ship
- [ ] Create 3+ EnemyData types
- [ ] Create 10+ UpgradeData types
- [ ] Test level-up system
- [ ] Test death and restart
- [ ] Balance difficulty

---

## 🎯 Design Philosophy

**Zero Manual Unity Work**
- No prefab creation in editor
- No UI layout in editor
- No scene setup beyond empty GameSetup object
- Everything runtime-generated

**Data-Driven**
- Content in ScriptableObjects
- Easy to expand
- Designer-friendly

**Performance-First**
- Object pooling
- No GC allocations in gameplay
- Mobile-optimized

**Monetization-Ready**
- Ad integration points
- IAP system scaffolding
- Metrics tracking

---

## 📧 Support

Check console logs for detailed information.
All systems log their initialization and key events.

**Game not starting?**
- Check Console for errors
- Verify GameSetup.cs is attached to GameObject
- Ensure all scripts compiled without errors

**Need more help?**
- Review TECHNICAL_IMPLEMENTATION.md
- Review GAME_DESIGN_DOCUMENT.md
- Check individual script comments
