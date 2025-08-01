# Chess Dungeon - Code Structure Documentation

## Overview
Chess Dungeon is a Unity-based tactical card game that combines chess-like movement mechanics with dungeon crawling elements. Players use cards to move and attack on a grid-based board while fighting monsters and collecting rewards.

## Core Architecture

### 🎮 Game Management
- **GameManager.cs** - Main game state controller, handles save/load functionality
- **TurnManager.cs** - Manages turn-based gameplay, action points, and turn progression
- **Player.cs** - Core player controller with movement, combat, and card interaction systems

### 🃏 Card System

#### Card Base Classes
- **Card.cs** - Abstract base class for all cards with core functionality
  - Supports card types: Move, Attack, Special
  - Handles upgrades, cloning, and execution callbacks
  - Contains cost, quick actions, energy mechanics

#### Card Categories
- **Move Cards** (`Card/Move/`)
  - `pawn_card.cs` - Basic forward movement (upgradeable)
  - `knight_card.cs` - L-shaped movement pattern
  - `bishop_card.cs` - Diagonal movement
  - `rook_card.cs` - Straight line movement
  - `assassin_card.cs` - Stealth movement mechanics

- **Attack Cards** (`Card/Attack/`)
  - `sword_card.cs` - Basic directional attack with knockback
  - `blade_card.cs` - Diagonal attacks
  - `spear_card.cs` - Extended range attacks
  - `bow_card.cs` - Ranged attacks anywhere on board
  - `flail_card.cs` - Area-of-effect attacks

- **Special Cards** (`Card/Special/`)
  - `potion_card.cs` - Quick action for extra action points
  - `energy_core.cs` - Energy system activation
  - `vine_card.cs` - Movement with damage effects
  - `book_*.cs` - Various spell-like effects
  - **BS Series** - Advanced special effects:
    - `BS01_card.cs` - 刃祝: Damage highest health enemy X times
    - `BS02_card.cs` - 罗盘: Draw cards until move card found
    - `BS05_card.cs` - 战舞: Gain action point + Grace (draw card)
    - `BS06_card.cs` - 狮鹫势: Next weapon card used twice
    - `BS07_card.cs` - 卷轴匣: Next card returns to deck top
    - `BS08_card.cs` - 盐袋: Gain armor for each enemy death

#### Card Management
- **DeckManager.cs** - Handles deck, hand, discard pile operations
- **CardDatabase.cs** - Central repository for all card definitions
- **CardPoolManager.cs** - Manages card rewards and generation
- **CardButton.cs/CardButtonBase.cs** - UI interaction for cards

### 👹 Monster System

#### Core Monster Classes
- **Monster.cs** - Base monster class with health, movement, AI, status effects
- **MonsterManager.cs** - Spawns, manages, and controls all monsters with special spawn rules
- **MonsterInfoManager.cs** - UI display for monster information

#### Monster Categories

**A系列 (Aerial/Advanced)**
- **A01-A06** - Advanced creatures with special abilities
- **A05** - 混血龙种 (Hybrid Dragon) - Queen movement + Wings ability

**B系列 (Beast/Boss)**
- **B01-B07** - Beast creatures with varying abilities
- **B08** - 强盗王 (Bandit King) - Boss with mode switching + Wings + Skills:
  - Cycles through Rook/Knight/Bishop/Queen movement modes each turn
  - Wings ability: Can fly over obstacles but can't land on them
  - Special skills when can't move: Create planks, summon monsters
  - At 6 HP: Special turn clears all locations + transforms other monsters

**I系列 (Insect/Infantry)**
- **I01-I05** - Basic to intermediate creatures

**Chess Pieces**
- **White/Dark Series**: Pawn, Knight, Bishop, Rook, Queen, King
- **Gold Series**: GoldPawn, GoldRook

#### Monster Abilities
- **Wings (翅膀)**: Can move through obstacles but not land on them (A05, B08)
- **Mode Switching**: Changes movement pattern each turn (B08)
- **Summoning**: Can spawn other monsters (B08)
- **Status Effects**: Stun, Lure, Karma Link support
- **Special Turns**: Triggered by health thresholds (B08)

### 🗺️ Level & Location System

#### Level Management
- **LevelConfig.cs** - Data structures for level definitions with monster templates
- **MonsterManager.cs** - Handles level-specific monster spawning with special rules
- **SpecialSpawnRule** - Configuration for monster-specific spawn areas

#### Location System
- **LocationManager.cs** - Manages terrain generation, ASCII layouts, and special spawn areas
- **Location.cs** - Base class for interactive map elements

#### Location Types
**Terrain Obstacles**
- **Forest.cs** - Blocking terrain (non-enterable)
- **Wall.cs** - Impassable barriers with corner variants
- **PlankLocation.cs** - Destructible wooden barriers (1 HP)

**Interactive Locations**
- **FirePoint.cs/FireZone.cs** - Fire-based hazards with duration
- **ActivatePoints.cs** - Energy system activation points
- **BarrierLocation.cs** - Temporary barriers with durability
- **AnchorLocation.cs** - Pulls player at turn end
- **MireLocation.cs** - Traps and removes monsters that step on them

#### Terrain Generation
**ASCII-Based Layouts**
- **Prison** - Plank-surrounded 2x2 center with special spawn areas
- **ForestMaze** - Complex forest patterns with clearings
- **RiverForest** - Forest borders with water streams
- **PlankField** - Destructible plank coverage with open center

**Procedural Terrains**
- **Borderland** - Edge obstacles with open center
- **DenseForest** - Cross-pattern forest with open corridors
- **FortifiedBorderland** - Wall borders with corner pieces

#### Special Spawn System
- **specialSpawnAreas** - Dictionary-based spawn area management
- **ASCII Integration** - Characters in ASCII maps define spawn zones
- **Monster-Area Binding** - Configuration links monster types to specific areas
- **Exclusion Logic** - Random spawning avoids special areas

#### Level Selection
- **NodeManager.cs** - Manages level selection map
- **LevelNode.cs** - Individual level nodes
- **RewardNode.cs** - Reward selection nodes
- **UpgradeNode.cs** - Card upgrade nodes

### 💎 Progression Systems

#### Rewards & Upgrades
- **RewardManager.cs** - Handles post-level card and relic rewards
- **RelicManager.cs** - Manages persistent player upgrades
- **Relic.cs** - Base class for permanent upgrades

#### Save System
- **SaveSystem.cs** - File-based save/load functionality
- **GameData.cs** - Serializable game state data
- **CollectionData.cs** - Player collection persistence
- **GameStateManager.cs** - Runtime state management

### 🎨 UI & Interaction

#### Core UI
- **MainMenuManager.cs** - Main menu navigation
- **PersistentCanvas.cs** - UI elements that persist across scenes
- **ClickBlocker.cs** - Input management during transitions

#### Specialized UI
- **MoveHighlight.cs** - Visual feedback for valid moves/attacks
- **HintManager.cs** - Tutorial and help system
- **ShopManager.cs** - In-game store functionality

#### Utility Systems
- **MusicManager.cs** - Audio management
- **CameraAspectController.cs** - Dynamic camera adjustment
- **GridScaler.cs/DynamicGrid.cs** - Responsive grid scaling

## Key Game Mechanics

### 🎯 Combat System
- Grid-based tactical combat
- Card-driven actions with limited action points per turn
- Directional attacks with knockback effects
- Monster AI with chess-piece movement patterns

### ⚡ Energy System
- Special cards can activate energy mechanics
- ActivatePoints charge the player
- DeactivatePoints trigger exhaust effects on hand

### 🔄 Card Mechanics
- **Quick Cards**: Don't consume action points
- **Partner Cards**: Draw additional partner cards when played
- **Temporary Cards**: Removed after battle
- **Upgrades**: Cards can be enhanced with multiple effects

### 🏰 Level Progression
- JSON-configured level layouts
- Dynamic terrain generation (Plains, Borderlands, Forests, Mazes)
- Monster templates for varied encounters
- Reward system with card and relic choices

## Data Flow

1. **Game Start**: GameManager loads save data or initializes new game
2. **Level Load**: MonsterManager spawns monsters based on level config
3. **Turn Cycle**: 
   - Player draws cards and takes actions
   - TurnManager processes turn end
   - Monsters move and attack
   - Check win/lose conditions
4. **Level Complete**: RewardManager presents choices
5. **Progression**: Save game state and return to level selection

## Configuration Files

### levelConfig.json Structure
```json
{
  "levels": [
    {
      "levelNumber": 9,
      "monsterTemplates": [
        {
          "monsterTypes": ["B07","B06","B04","B04","A01","A01"],
          "terrainType": "Prison",
          "specialSpawnRules": [
            {
              "monsterType": "A01",
              "spawnArea": "predefined"
            }
          ]
        }
      ]
    }
  ],
  "terrains": [
    {
      "name": "Prison",
      "mapSize": 8,
      "specialSpawnArea": {
        "x": 3, "y": 3, "width": 2, "height": 2
      }
    }
  ]
}
```

### Configuration Features
- **Monster Templates** - Multiple spawn configurations per level
- **Special Spawn Rules** - Link monster types to specific areas
- **Terrain Definitions** - ASCII layouts with special area markers
- **Dynamic Generation** - Procedural and template-based terrain

### Other Configuration
- Card definitions embedded in code classes
- Relic configurations in ScriptableObjects

## Architecture Strengths
- **Modular Design**: Clear separation between systems
- **Extensible**: Easy to add new cards, monsters, and mechanics
- **Data-Driven**: Level configuration through JSON
- **Save System**: Comprehensive game state persistence
- **UI Flexibility**: Responsive design with dynamic scaling

This codebase demonstrates a well-structured Unity game with clear MVC patterns, extensive customization options, and robust state management suitable for a complex tactical card game.

## Turn-Based Gameplay Workflow

### 🎯 **Turn Start**
```
TurnManager.Start()
├── Player.Start()
├── DeckManager.DrawCards() → Draw hand cards
└── UI Updates (actionText, turnPanel)
```

### 🃏 **Player Action Phase**
```
Player clicks card
├── CardButton.OnClick()
├── Player.ShowMoveOptions() / ShowAttackOptions()
├── Player clicks highlighted tile
├── Player.Move() / Attack()
├── Player.ExecuteCurrentCard()
├── DeckManager.UseCard() → Move card to discard pile
├── TurnManager.MoveCursor() → Consume action point
└── TurnManager.UpdateActionText()
```

### 🔄 **Turn End** (EndTurn button clicked)
```
TurnManager.AdvanceTurn()
├── TurnManager.HandleTurnEnd() [Coroutine]
├── TurnManager.DisableAllButtons()
├── DeckManager.DiscardHand() → Discard all hand cards
├── Player.ResetEffectsAtEndOfTurn()
├── Player.ClearMoveHighlights()
├── MonsterManager.OnTurnEnd()
│   ├── Check level completion
│   ├── MonsterManager.MoveMonsters() → Monster movement
│   └── Possibly trigger RewardManager.StartRewardProcess()
├── Wait for monster movement completion
├── Wait for reward panel to close (if any)
├── DeckManager.HandleEndOfTurnEffects() → Hoarding effects
├── DeckManager.DrawCards() → Draw new hand
├── TurnManager.EnableAllButtons()
├── TurnManager.ResetCursor()
└── New turn begins
```

### 📋 **Core Classes Involved**

**Main Control Flow:**
- `TurnManager` - Turn management and coordination
- `Player` - Player actions and state
- `DeckManager` - Card management and effects

**Interaction Components:**
- `CardButton/CardButtonBase` - Card UI interaction
- `MoveHighlight` - Movement/attack visualization

**Game Logic:**
- `MonsterManager` - Monster AI and spawning
- `RewardManager` - Post-level rewards
- `Card` (various card classes) - Card-specific effects

**Key Flow**: TurnManager acts as the central coordinator, orchestrating interactions between Player, DeckManager, and MonsterManager throughout each turn cycle.

## Card System & Special Effects

### 🎯 **Core Card Execution Methods**
```csharp
// Player class card execution methods
Player.Attack(Vector2Int attackPosition)           // Single target attack
Player.MultipleAttack(Vector2Int[] positions)      // Multi-target attack
Player.MultipleSpecial(Vector2Int[] positions)     // Special multi-target effects
Player.Move(Vector2Int newPosition)                // Movement
Player.ShowMoveOptions() / ShowAttackOptions()     // Display valid targets
```

### 🔧 **Reusable Special Effect Systems**

#### **KeywordEffects** - Shared Combat Effects
```csharp
// Combat Effects
KeywordEffects.AttackWithKnockback()    // Knockback on attack

// Ritual System
KeywordEffects.StartBasicRitual()       // Ritual counter system
KeywordEffects.IncrementBasicRitual()   // Progress ritual
KeywordEffects.StopBasicRitual()        // Complete/reset ritual

// BS Series Card Effects
KeywordEffects.ActivateGriffinStance()  // BS06: Next weapon used twice
KeywordEffects.ActivateScrollCase()     // BS07: Next card returns to deck top
KeywordEffects.ActivateSaltBag()        // BS08: Gain armor on enemy death
KeywordEffects.TriggerSaltBagOnEnemyDeath() // Handle salt bag effect
KeywordEffects.ResetBSEffects()         // Reset BS effects at turn end
```

#### **Energy System** - Global State Management
```csharp
player.Charge() / Decharge()            // Energy state control
player.isCharged                        // Energy status check
ActivatePoints / DeactivatePoints       // Board energy triggers
```

#### **Fire Zone System** - Environmental Effects
```csharp
LocationManager.CheckAndFormFireZone()  // Create fire zones from points
FireZone.OnEnemyTurnStart()            // Deal damage each turn
FireZone.DestroySelf()                 // Duration-based cleanup
```

#### **Card State Effects**
```csharp
vineEffectActive        // Movement triggers adjacent damage
isMadness              // Discard effects (ritual spear)
isPartner              // Draw partner cards when played
isQuick                // No action point cost
isTemporary            // Removed after battle
isExhaust              // Removed from game after use
isGrace                // Draw 1 card when played
isLingering            // Stays in hand at turn end
isTriumph              // Returns to deck top from discard

// BS Series State Effects
nextWeaponCardDoubleUse    // BS06: Next attack card used twice
nextCardReturnToDeckTop    // BS07: Next card returns to deck
saltBagEffectActive        // BS08: Gain armor on enemy death
```

#### **Card Upgrade System**
```csharp
CardUpgrade.Quick      // Make card quick
CardUpgrade.Draw1/2    // Draw cards on use
CardUpgrade.GainArmor  // Gain armor on use
card.AddUpgrade()      // Apply upgrade effects
```

### 📋 **Special Effect Categories**

| Effect Type | Implementation | Reusability |
|-------------|----------------|-------------|
| **Knockback** | KeywordEffects | High - Multiple attack cards |
| **Ritual** | KeywordEffects | Medium - Specific card series |
| **Fire Zone** | LocationManager | Medium - Environmental |
| **Energy** | Player/DeckManager | High - Global system |
| **Vine** | Player state | Low - Single card effect |
| **Madness** | Card base class | Medium - Discard triggers |
| **BS Effects** | KeywordEffects | High - Modular special states |
| **Grace** | Card base class | High - Draw card keyword |
| **Exhaust** | DeckManager | High - Permanent removal |

### 🔄 **Design Patterns Used**
- **Strategy Pattern**: Different `OnCardExecuted()` implementations
- **Observer Pattern**: `OnCardPlayed` event system  
- **Factory Pattern**: `CardDatabase` card creation
- **Composite Pattern**: Multi-upgrade card system

**Architecture Benefit**: New cards can easily combine existing special effects through the modular effect system.

## FA系列卡牌实现流程 (Fire/Flame Series Implementation)

### 🔥 **FA01 灼锋 (Searing Edge)**
**实现流程：**
1. 创建FA01_card.cs - Attack类型，十字I级攻击模式
2. 实现GetDamageAmount()返回1点伤害
3. OnCardExecuted中调用PlaceFirePointAt()在目标位置创造燃点
4. 添加到Uncommon卡池

### 🏹 **FA02 烈矢 (Blazing Arrow)**
**实现流程：**
1. 创建FA02_card.cs - Attack类型，全场攻击模式
2. 使用allBoardDirections数组覆盖8x8棋盘所有位置
3. GetDamageAmount()返回1点伤害
4. OnCardExecuted中创造燃点
5. 添加到Epic卡池（稀有）

### 🕯️ **FA03 蛟油蜡烛 (Tidewyrm-Oil Candle)**
**实现流程：**
1. 创建FA03_card.cs - Attack类型（0伤害），十字I级
2. GetDamageAmount()返回0（不造成直接伤害）
3. OnCardExecuted中调用CreateRandomFirePoints()在目标周围随机创造3处燃点
4. 获取目标点周围9个位置，随机选择3个有效位置
5. 添加到Uncommon卡池

### 🛢️ **FA04 焚化油 (Incineration Oil)**
**实现流程：**
1. 创建FA04_card.cs - Attack类型（0伤害），全方向II级
2. 属性：快速（isQuick）、消耗（isExhaust）
3. OnClick中检查IsPlayerInFireZone()，只能在火域上使用
4. OnCardExecuted中在目标十字相邻格创造燃点
5. 添加到Common卡池

### ⚔️ **FA05 炽炬-熄灭 (Torch-Extinguish)**
**实现流程：**
1. 创建FA05_card.cs - Attack类型，十字II级，1点伤害
2. OnCardExecuted中检查IsTargetOnFirePointOrZone()
3. 若目标在燃点/火域上，创建FA05a衍生卡牌并添加到手牌
4. FA05a设置为临时卡牌（isTemporary）
5. 添加到Epic卡池

### 🔥 **FA05a 炽炬-点燃 (Torch-Ignite)**
**实现流程：**
1. 创建FA05a_card.cs - Attack类型，十字II级
2. 属性：快速（isQuick）、衍生（isTemporary）
3. GetDamageAmount()返回场上燃点数量（动态伤害）
4. 仅作为衍生卡牌，不添加到卡池

### 🐉 **FA06 蛇龙之涎 (Drakon's Froth)**
**实现流程：**
1. 创建FA06_card.cs - Attack类型，十字无限攻击
2. OnClick中检查IsPlayerInFireZone()，只能在火域上使用
3. OnCardExecuted中实现AttackInDirection()沿方向攻击所有敌人
4. 统计命中敌人数量，≥2时调用player.AddFervent(1)
5. 添加到Legendary卡池（史诗）

### ⚔️ **FA07 灼骨之刃 (Bone-Searing Blade)**
**实现流程：**
1. 创建FA07_card.cs - Attack类型，斜向I级，1点伤害
2. 使用diagonalDirections数组定义四个对角线方向
3. OnCardExecuted中检查目标怪物血量是否≤0（参考BA13实现）
4. 击杀成功时调用player.AddFervent(1)
5. 添加到Legendary卡池

### 🗡️ **FA08 焰形剑 (Flamberge)**
**实现流程：**
1. 创建FA08_card.cs - Attack类型，十字I级，2点伤害
2. 在DeckManager中添加DiscardAllCards()方法返回丢弃数量
3. OnCardExecuted中丢弃所有手牌，获得丢弃数量
4. 实现TriggerFireZoneDamage()触发火域伤害X次
5. 在FireZone中添加TriggerDamageOnly()方法（只造成伤害不减少持续时间）
6. 添加到Epic卡池

### 🔥 **炽烈机制 (Fervent System)**
**实现流程：**
1. 在Player类中添加ferventStacks变量和AddFervent()方法
2. 修改FireZone.OnEnemyTurnStart()，火域伤害 = 2 + 炽烈层数
3. 炽烈层数只在关卡结束时清零，不会每回合减少
4. 与愤怒系统类似但持续时间更长

### 🏗️ **通用实现模式**
1. **创建卡牌文件** - 在对应文件夹创建XXX_card.cs
2. **定义攻击模式** - 使用方向数组定义攻击范围
3. **实现核心效果** - 在OnCardExecuted中实现特殊效果
4. **添加到管理器** - 更新CardPoolManager、DeckManager、CardDatabase
5. **测试验证** - 确保效果正确触发

### 🔧 **关键技术点**
- **火域检测**: 通过LocationManager.activeFireZones检查
- **燃点创造**: 使用LocationManager.CreateFirePoint()
- **击杀检测**: 参考BA13，检查monster.health <= 0
- **衍生卡牌**: 设置isTemporary属性，战斗结束后自动移除
- **条件使用**: 在OnClick中添加使用条件检查