# Coins of Hope (Unity 2D) — Phase 1 MVP

This repository contains the Phase 1 MVP scaffolding for **Coins of Hope**.

## Unity Version
- Unity **6** (Editor version `6000.0.0f1`) is referenced in `ProjectSettings/ProjectVersion.txt`.

## Scenes
- `Assets/Scenes/SplashScreen.unity`
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/CoinRunnerGameplay.unity`
- `Assets/Scenes/StoryScene.unity`

## Script Layout
- `Assets/Scripts/GameManager.cs` (global state, persistence, daily reward)
- `Assets/Scripts/Audio/AudioManager.cs`
- `Assets/Scripts/Gameplay/*` (Coin Runner runner/spawners/input)
- `Assets/Scripts/UI/*` (menu, story, scene transitions)
- `Assets/Scripts/Data/*` (ScriptableObjects + serialization)

## Notes
- Ads integration is implemented behind conditional compilation (`GOOGLE_MOBILE_ADS`).
  - If the Google Mobile Ads Unity plugin is present, enable the scripting define symbol `GOOGLE_MOBILE_ADS`.
  - Without the plugin, the project still compiles and uses a safe stub implementation.
