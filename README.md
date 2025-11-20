# 🎮 NEON SURVIVORS - Auto-Shooter + Idle Hybrid Game

## 📖 Proje Hakkında

Bu proje, **%100 kod ile** oluşturulacak bir mobil oyun geliştirme sürecini içerir. Unity Editor'de hiçbir manuel UI tasarımı, level tasarımı veya prefab oluşturma işi yapılmayacak - her şey programmatic olarak kodlanacak.

### Oyun Türü
- **Auto-Shooter** (Vampire Survivors tarzı)
- **Idle/Incremental** mechanics
- **Roguelite** progression
- **Minimal Neon Geometric** art style

### Platform
- Mobil (Android + iOS)
- Unity 2022.3 LTS

---

## 📚 Dokümantasyon

### 1. [GAME_DESIGN_DOCUMENT.md](./GAME_DESIGN_DOCUMENT.md)
**Kapsamlı oyun tasarım dokümanı:**
- Core gameplay loop
- Sistem tasarımları (player, weapon, enemy, progression)
- Monetizasyon stratejisi
- Görsel tasarım (100% kod ile)
- Development roadmap
- Success metrics

### 2. [TECHNICAL_IMPLEMENTATION.md](./TECHNICAL_IMPLEMENTATION.md)
**Detaylı teknik implementasyon:**
- Architecture overview (design patterns)
- Core systems (pooling, procedural generation, data)
- Kod örnekleri (PlayerController, EnemySpawner, UIManager, etc.)
- Save system (Easy Save 3)
- Monetization integration (Ads + IAP)
- Performance optimization
- Deployment checklist

---

## 🎯 Proje Hedefi

**"Play dediğimde hazır oyun olsun"** - Unity Editor'de:
- ❌ Hiçbir UI tasarımı yapmayacağım
- ❌ Hiçbir level tasarımı yapmayacağım
- ❌ Hiçbir prefab oluşturmayacağım
- ❌ Hiçbir manuel visual adjustment yapmayacağım

**✅ Tek yapacağım:** Git clone → Unity'de aç → Play

---

## 🚀 Özellikler

### Core Gameplay
- **Auto-shooting** - En yakın düşmana otomatik ateş
- **Wave-based enemies** - Sonsuz dalga sistemi
- **Level-up system** - Her level'da 3 random upgrade seç
- **30+ upgrades** - Synergy mechanics (kombinasyonlar güçlü etkiler)
- **5 base weapons** - Blaster, Laser, Missiles, Shotgun, Orbit

### Meta-Progression
- **15+ unlockable ships** - Her biri unique ability
- **Prestige system** - Reset → permanent multipliers
- **Workshop** - Permanent stat upgrades
- **Idle earnings** - Offline gold farming (4-hour cap)
- **Collection system** - Achievement-based unlocks

### Monetization
- **Hybrid model** (50% ads, 50% IAP)
- **Rewarded ads** (revive, double gold, offline multiplier)
- **IAP** (premium ships, gold packs, ad removal, battle pass)
- **Target ARPDAU:** $1.50-$3.00

### Visual Style
- **Neon geometric shapes** (procedural meshes)
- **Particle systems** (trails, explosions)
- **Post-processing** (bloom, color grading)
- **Gradient backgrounds**
- **No textures needed** - Pure code

---

## 🔧 Teknoloji Stack

### Unity Packages
- **UI Toolkit** (built-in) - Runtime UI generation
- **URP** (Universal Render Pipeline)
- **Post-Processing** (Package Manager)
- **TextMeshPro** (built-in)

### Asset Store / External
- **DOTween Free** - Tweening animations
- **Easy Save 3** (~$45) - Save system
- **Mobile Monetization Pro** (~$50) - Ads/IAP integration

### Required Services
- Unity Analytics (free)
- Unity Cloud Save (optional, free tier)
- AdMob / IronSource mediation
- Unity IAP

---

## 📂 Proje Yapısı

```
Yeni-Oyun-Fikri/
├── README.md                          # Bu dosya
├── GAME_DESIGN_DOCUMENT.md           # Oyun tasarım dokümanı
├── TECHNICAL_IMPLEMENTATION.md        # Teknik implementasyon
│
└── (Unity projesi oluşturulacak)
    ├── Assets/
    │   ├── Scripts/
    │   │   ├── Core/               # GameManager, PoolManager, SaveManager
    │   │   ├── Player/             # PlayerController, PlayerWeapon, etc.
    │   │   ├── Enemy/              # EnemyBase, EnemySpawner
    │   │   ├── Progression/        # LevelSystem, PrestigeManager
    │   │   ├── UI/                 # UIManager (UI Toolkit)
    │   │   ├── Monetization/       # AdManager, IAPManager
    │   │   └── Data/               # ScriptableObjects
    │   │
    │   ├── Resources/
    │   │   └── Data/               # ScriptableObject instances
    │   │
    │   ├── Scenes/
    │   │   ├── MainMenu.unity
    │   │   └── GamePlay.unity
    │   │
    │   └── UI/
    │       └── UXML/               # UI Toolkit documents
    │
    └── (Build output)
```

---

## 📊 Development Timeline

### Week 1-2: Foundation ✅
- Unity project setup
- Core gameplay (movement, auto-shoot)
- Enemy spawning
- Object pooling
- Procedural meshes

### Week 3-4: Combat & Upgrades
- Weapon system (5 types)
- 30+ upgrades with synergies
- Level-up screen
- Particle effects

### Week 5-6: Meta-Progression
- Gold earning
- Workshop system
- Prestige mechanics
- Ship collection
- Idle earnings

### Week 7-8: Monetization
- Ad integration
- IAP system
- Battle Pass
- Analytics

### Week 9-10: Content & Polish
- All ships balanced
- All upgrades implemented
- Boss encounters
- Post-processing

### Week 11-12: Testing & Launch
- Performance optimization (60 FPS)
- Difficulty balancing
- Soft launch (Canada, Australia)

---

## 🎯 Success Metrics (Target)

### Retention
- **D1:** 40-45%
- **D7:** 18-22%
- **D30:** 8-12%

### Monetization
- **ARPDAU:** $1.50-$3.00
- **Conversion Rate:** 3-7%
- **Ad eCPM:** $12-$30
- **Rewarded Ad Completion:** 80%+

### Engagement
- **Session Length:** 12-18 minutes
- **Sessions/Day:** 3-5
- **Daily Ad Views:** 8-12 per DAU

---

## 🚀 Başlarken

### Gereksinimler
- Unity 2022.3 LTS veya daha yeni
- Visual Studio / Rider (IDE)
- Android SDK / Xcode (mobile build için)

### Kurulum
```bash
# 1. Repository'yi clone et
git clone <repo-url>

# 2. Unity Hub'da projeyi aç
# File → Open → Proje klasörünü seç

# 3. Play tuşuna bas
# Oyun anında çalışır olmalı
```

### İlk Çalıştırma
1. Unity'de GamePlay scene'ini aç
2. Play tuşuna bas
3. Joystick ile hareket et
4. Düşmanlar spawn olur ve otomatik ateş eder
5. Level atla, upgrade seç, sonsuz oyna

---

## 📝 Development Notes

### Procedural Generation Yaklaşımı
- **Meshes:** Runtime'da oluşturulur (ProceduralMeshGenerator)
- **UI:** UI Toolkit ile kod ile generate edilir
- **Enemies:** Object pooling ile runtime spawn
- **Levels:** Matematiksel formüllerle infinite scaling

### Data-Driven Design
- **ScriptableObjects** her şey için:
  - Ships (15+ data assets)
  - Weapons (5+ data assets)
  - Upgrades (30+ data assets)
  - Enemies (8+ data assets)

- **Yeni içerik eklemek:** Create → Game → Ship Data → Inspector'da doldur → Kod değişikliği ZERO

### Performance Targets
- **60 FPS** on mid-range Android (Snapdragon 660+)
- **Draw calls:** <100
- **Memory:** <512MB
- **Battery:** Efficient (no heavy physics)

---

## 🎮 Inspiration

Bu oyun şu başarılı oyunlardan ilham alıyor:

- **Vampire Survivors** ($100M+ revenue) - Core auto-shooter mechanics
- **Block Blast** ($200M+ annual) - Ad-driven model, infinite play
- **Royal Match** ($1.46B revenue) - Deep meta-progression
- **Gold & Goblins** ($13.1M in 4 months) - Small team success

**Differentiation:**
- ✅ Idle earnings (VS'de yok)
- ✅ Prestige system (VS'de weak)
- ✅ Ship collection (VS minimal)
- ✅ Neon aesthetic (unique visual identity)
- ✅ Hybrid monetization (balanced ads+IAP)

---

## 📈 Market Opportunity

### 2024-2025 Mobile Gaming Market
- **$92 billion** total mobile gaming revenue
- **Hybrid-casual** segment: +37% YoY growth (fastest growing)
- **Puzzle/idle** genres: 75% female, 35+ age (high spending)
- **Ad-driven** model viable: Block Blast proves $17.5M monthly possible

### Why This Game Will Succeed
1. ✅ **Proven formula** (Vampire Survivors blueprint)
2. ✅ **Low development cost** (minimal assets, code-only)
3. ✅ **High replayability** (procedural, infinite)
4. ✅ **Deep meta-progression** (100+ hours content)
5. ✅ **Balanced monetization** (no predatory tactics)
6. ✅ **Viral potential** (daily challenges, leaderboards)

---

## 🤝 Contributing

Bu proje tek geliştirici (AI-assisted development) ile oluşturulmaktadır.

**Development Model:**
- 🤖 AI (Claude) - Tüm kod yazımı
- 👤 Human - Direction, testing, feedback

**Workflow:**
1. Tasarım kararları → Human
2. Implementation → AI
3. Testing → Human
4. Iteration → AI
5. Launch → Human

---

## 📄 License

Bu proje eğitim ve ticari kullanım için tasarlanmıştır.

---

## 🎯 Next Steps

**Sıradaki:** Unity project initialization

```bash
# Unity projesi oluştur
# Package'leri kur (URP, UI Toolkit, etc.)
# Klasör yapısını oluştur
# İlk core scripts'i yaz (GameManager, PoolManager)
# First playable prototype (Week 1-2)
```

**Ready to start! 🚀**

---

## 📞 Contact

Sorular veya geri bildirim için:
- GitHub Issues kullanabilirsiniz
- Development blogu (yakında)

---

**"Zero manual work in Unity Editor - Pure code, pure gameplay."** ⚡
