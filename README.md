# Overview  
All code was made by me.  

I started this project out of curiosity to see if it was possible to create a fully polished and fleshed-out game entirely through the VSCode terminal. I know at least some of us programmers have thought about it, and well, here it is!  

I haven’t decided on a name for the game yet, so for now, it’s called **RPGCombat**! RPGCombat is a roguelike game where you play cards to defeat enemies, encounter events, and experience much, much more!  

This is a tutorial explaining the new content of the game.  
[Video Demo NEW 1.05](https://youtu.be/placeholder)  

## **Old Videos**  
- [Video Demo 1.04](https://www.youtube.com/watch?v=TDxLxGdfLmI)  
- [Video Demo 1.03](https://www.youtube.com/watch?v=iRXw389TukM)  
- [Video Demo 1.02](https://www.youtube.com/watch?v=yfPR-MDi1Uo)  
- [Video Demo 1.01](https://www.youtube.com/watch?v=K2lNMMITx70)  
- [Video Demo 1.00](https://www.youtube.com/watch?v=wr_GwDA3vfk)  

# Development Environment  
- **VSCode**  

## **Library Extensions**  
- .NET Install Tool  
- C#  
- C# Dev Kit  
- GitHub Copilot  
- GitHub Copilot Chat  
- IntelliCode for C# Dev Kit  

The code is written in **C#**.  

# Useful Websites  
- [ChatGPT](https://chatgpt.com) – Used to learn new code and libraries.  
- [MonsterWorld](https://scratch.mit.edu/projects/228016745/) – Game of Inspiration on Scratch.  

# **Future Work**  
- None (for now)  

# **Update Log**

### **1.05 – Ability Upgrades & JSON‑Driven Classes**  
- **Ability Upgrade System**  
  - Added `UpgradedTo` field in `abilities.json` to define next‑level ability.  
  - `Ability` class now stores `UpgradedTo` and loader/factory propagate it.  
  - **Upgrade Menu** lists only upgradable abilities; replaces old ability with its upgraded version.  
- **JSON‑Driven Player Classes**  
  - Moved all player class definitions (stats, default hand, progression) into `playerClasses.json`.  
  - Added `PlayerLoader` with per‑entry `try/catch` and console logging to safely load classes.  
  - `PlayerCreature` now holds `AbilityProgression` & `NextAbilityIndex`; auto‑unlocks new abilities on level up.  
- **Robust JSON Loaders**  
  - Wrapped each item load in its own `try/catch` in **Abilities**, **PlayerLoader**, and **EnemyLoader**.  
  - Logs successes (`✅ Loaded …`) and failures (`⚠️ Failed to load …`) to surface malformed entries without halting.  

### **1.04 – Dynamic Enemy Generation & Scaling**  
- **New Enemy Spawning System**  
  - Each round, **GenerateEnemiesForRound** picks from `enemies.json` and scales stats by round.  
  - `EnemyLoader` loads templates from JSON with safe per‑entry error handling.  
- **Improved Enemy AI**  
  - Enemies choose abilities by stamina, health thresholds, and target lowest‑HP players.  
  - AI supports healing, buffing, and aggressive behaviors.  
- **Expanded Enemy Roster & Abilities**  
  - Added dozens of new enemies (Fire Elemental, Frost Wraith, Stone Golem, Dragon, Vampire Lord, Thunder Titan, etc.).  
  - Introduced new abilities: Burning Touch, Frozen Touch, Multi‑Strike, Rock Smash, Dark Pulse, Roar, Life Drain, Hypnotize, Thunder Clap, Storm Shield.  

### **1.03 – Abilities Revamp & Status Effects**  
- **Core Ability Overhaul**  
  - Unified `Ability` class drives both players and enemies.  
  - Added targeting methods: Single, Random, All.  
  - Effects: Damage, Piercing, Bludgeoning, Heal, Shield, and Custom status.  
- **Status Effects**  
  - Burning 🔥, Frozen ❄, Bleeding 🩸, Poison ☠️, Stun ⚡, Weaken 🛑, Regeneration 🌿.  
  - Effects apply each turn, expire by duration, and show visually in UI.  

### **1.02 – Player Leveling & Upgrades**  
- **Level‑Up System**  
  - Players gain levels after combat, earn Upgrade Points.  
  - Upgrade Points can boost Max HP, Stamina, or unlock/upgrade abilities.  
- **Modular Effects & Cards**  
  - Effects now fully data‑driven, making new abilities trivial to add.  
  - Cards support multi‑target, self‑target, ally‑target.  

### **1.01 – Multiplayer & AI Improvements**  
- Local hot‑seat multiplayer.  
- Smarter enemy targeting in combat.  
- General bug fixes and polish.  

### **1.00 – Initial Release**  
- First playable combat prototype.  
