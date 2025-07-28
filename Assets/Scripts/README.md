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

#### Card Management
- **DeckManager.cs** - Handles deck, hand, discard pile operations
- **CardDatabase.cs** - Central repository for all card definitions
- **CardPoolManager.cs** - Manages card rewards and generation
- **CardButton.cs/CardButtonBase.cs** - UI interaction for cards

### 👹 Monster System

#### Core Monster Classes
- **Monster.cs** - Base monster class with health, movement, AI
- **MonsterManager.cs** - Spawns, manages, and controls all monsters
- **MonsterInfoManager.cs** - UI display for monster information

#### Monster Types
- **Chess Pieces**: WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing
- **Dark Pieces**: DarkPawn, DarkKnight, DarkBishop, DarkRook, DarkQueen, DarkKing
- **Creatures**: Slime, SlimeKing, Bat, Hound
- **Special**: GoldPawn, GoldRook

### 🗺️ Level & Location System

#### Level Management
- **LevelManager.cs** - Controls level progression and configuration
- **LevelConfig.cs** - Data structures for level definitions
- **MonsterManager.cs** - Also handles level-specific monster spawning

#### Location System
- **LocationManager.cs** - Manages terrain generation and environmental hazards
- **Location.cs** - Base class for interactive map elements
- **Specific Locations**:
  - `Forest.cs` - Blocking terrain
  - `Wall.cs` - Impassable barriers
  - `FirePoint.cs/FireZone.cs` - Fire-based hazards
  - `ActivatePoints.cs` - Energy system activation points

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
- `levelConfig.json` - Level definitions, monster spawns, terrain types
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
KeywordEffects.AttackWithKnockback()    // Knockback on attack
KeywordEffects.StartBasicRitual()       // Ritual counter system
KeywordEffects.IncrementBasicRitual()   // Progress ritual
KeywordEffects.StopBasicRitual()        // Complete/reset ritual
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

### 🔄 **Design Patterns Used**
- **Strategy Pattern**: Different `OnCardExecuted()` implementations
- **Observer Pattern**: `OnCardPlayed` event system  
- **Factory Pattern**: `CardDatabase` card creation
- **Composite Pattern**: Multi-upgrade card system

**Architecture Benefit**: New cards can easily combine existing special effects through the modular effect system.