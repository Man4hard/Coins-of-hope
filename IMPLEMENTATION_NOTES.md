# Coins of Hope - Phase 1 MVP Implementation Notes

## Project Structure

The project is implemented with the following structure:

```
Assets/
├── Scenes/
│   ├── SplashScreen.unity
│   ├── MainMenu.unity
│   ├── CoinRunnerGameplay.unity
│   └── StoryScene.unity
├── Scripts/
│   ├── GameManager.cs (Core game state & persistence)
│   ├── AdManager.cs (Rewarded ads stub/integration)
│   ├── Bootstrapper.cs (Auto-initializes managers)
│   ├── SceneBuilder.cs (Runtime scene construction)
│   ├── Audio/
│   │   └── AudioManager.cs
│   ├── Data/
│   │   ├── GameConstants.cs
│   │   ├── PlayerData.cs
│   │   ├── PersistenceService.cs
│   │   ├── DailyRewardService.cs
│   │   ├── LevelConfig.cs (ScriptableObject)
│   │   └── StoryCharacter.cs (ScriptableObject)
│   ├── Gameplay/
│   │   ├── PlayerController.cs
│   │   ├── CoinRunnerManager.cs
│   │   ├── ObstacleMover.cs
│   │   └── SpriteFactory.cs
│   └── UI/
│       ├── SceneNames.cs
│       ├── UITheme.cs
│       ├── UIFactory.cs
│       ├── SplashScreenController.cs
│       ├── MainMenuController.cs
│       ├── StorySceneController.cs
│       └── LevelSelectController.cs
└── Resources/
    └── (LevelConfig and StoryCharacter assets can be placed here)
```

## Key Features Implemented

### 1. Game Manager (Singleton)
- Persistent coin tracking
- Level progression (5 levels)
- Story progress tracking
- Daily reward system (10 coins per day)
- Save/Load via PlayerPrefs

### 2. Coin Runner Mini-Game
- Player controls: Tap to jump, Swipe down to slide
- Obstacle spawning with collision detection
- Coin collection
- 5 difficulty levels (configurable via LevelConfig ScriptableObjects)
- Progressive difficulty: speed, spawn rates, obstacle density
- Coins earned based on survival time + collected coins

### 3. Story System
- 2 default stories:
  - "Amina's Journey": Single mother (Food: 50, Books: 30, Clothes: 40)
  - "Rahim's Hope": Elderly man (Medical: 60, Warm Clothes: 40, Food: 50)
- Donate coins to fulfill needs (10 coins per tap)
- Progress tracking per need
- Visual progress indication
- Story unlocking: Complete Level 5 to unlock stories

### 4. Audio System
- Background music (menu & gameplay)
- SFX: coin collect, obstacle hit, story help, button click
- Mute toggle (saved via PlayerPrefs)

### 5. Ad Integration (Stub)
- AdManager with conditional compilation (#if GOOGLE_MOBILE_ADS)
- Test ad unit IDs ready
- Rewarded ad support
- Interstitial ad support
- Works without Google Mobile Ads SDK (stub mode)

### 6. UI System
- Runtime scene construction via SceneBuilder
- Color theme (UITheme):
  - Primary: Warm teal (#2EC4B6)
  - Secondary: Soft orange (#FF9F1C)
  - Background: Light cream (#FDFFFC)
  - Text: Dark gray (#2B2D42)
- Responsive canvas scaling (1080x1920 portrait)
- Scene transitions: Splash → MainMenu → CoinRunner/Story

### 7. Persistence
- PlayerPrefs-based save system
- JSON serialization for player data
- Daily reward timestamp tracking
- Story progress persistence

## Scene Flow

1. **SplashScreen**: Auto-loads MainMenu after 2 seconds
2. **MainMenu**: Shows coins, Play, Stories, Settings buttons
3. **CoinRunnerGameplay**: Endless runner gameplay
4. **StoryScene**: View and donate to character stories

## Runtime Scene Construction

The project uses a unique runtime scene building system via `SceneBuilder.cs`. When a scene loads, it automatically constructs the UI and gameplay elements programmatically. This approach:

- Reduces scene file complexity
- Makes it easier to maintain consistent UI
- Allows for dynamic scene generation
- Works without Unity Editor

## Google Mobile Ads Integration

To enable Google Mobile Ads:

1. Install the Google Mobile Ads Unity Plugin
2. Add `GOOGLE_MOBILE_ADS` to Scripting Define Symbols (Player Settings)
3. Replace test ad unit IDs in `AdManager.cs` with your real IDs

Without the plugin, the project uses a stub implementation that logs ad events but doesn't show real ads.

## Level Configuration

Levels are configured via `LevelConfig` ScriptableObjects. You can create them in Unity:

```
Create → Coins of Hope → Level Config
```

Properties:
- `levelNumber`: 1-5
- `scrollSpeed`: Movement speed
- `obstacleSpawnInterval`: Time between spawns
- `obstacleSpawnChance`: Probability (0-1)
- `coinSpawnInterval`: Coin spawn timing
- `coinSpawnChance`: Coin probability (0-1)
- `coinsPerSecond`: Base coin earning rate

If no configs are found in Resources, the system creates default configurations.

## Story Character Configuration

Create story characters via ScriptableObjects:

```
Create → Coins of Hope → Story Character
```

Properties:
- `storyId`: Unique identifier
- `characterName`: Display name
- `introText`: Story introduction
- `resolutionText`: Success message
- `needs`: List of needs (name, cost, description)

## Controls

### Coin Runner Gameplay:
- **Tap** or **Space**: Jump
- **Swipe Down** or **S/Down Arrow**: Slide

### Collision Rules:
- **Obstacles** (red cubes): Game over on hit
- **Coins** (yellow spheres): Collect to add to total

## Testing Checklist

✅ GameManager persists across scenes
✅ Coins save and load correctly
✅ Daily reward triggers once per day
✅ Level progression unlocks next level
✅ Story unlocks after Level 5 completion
✅ Coin donation updates story progress
✅ Audio plays for events
✅ Mute toggle works
✅ Scene transitions are smooth
✅ Player controls responsive
✅ Obstacles spawn and move
✅ Coins spawn and collectible

## Known Limitations

1. **No actual art assets**: Using primitive shapes (cubes, spheres)
2. **No audio files**: Audio clips need to be assigned in AudioManager
3. **ScriptableObjects**: Level configs and story characters are created at runtime if missing
4. **Physics2D**: Using basic 2D physics, may need tuning
5. **Portrait orientation**: Hardcoded for mobile (1080x1920)

## Next Steps (Post-Phase 1)

- Add proper sprites for player, obstacles, coins
- Add background art and parallax scrolling
- Implement proper ad integration with real ad unit IDs
- Add particle effects for coin collection
- Add animations for character sprites
- Polish UI with better fonts and icons
- Add sound effects and music tracks
- Implement level select screen (currently Play goes directly to gameplay)
- Add pause menu in gameplay
- Add game over screen with retry/menu options
- Test on actual Android devices
- Implement interstitial ads after X level completions

## Development Notes

The project is designed to work without Unity Editor as much as possible:

- Managers auto-initialize via `Bootstrapper`
- Scenes auto-build via `SceneBuilder`
- Default data created at runtime if missing
- Conditional compilation for optional dependencies (ads)

This makes the codebase resilient and easier to test.
