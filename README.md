# Overview  
All code was made by me.  

I started this project out of curiosity to see if it was possible to create a fully polished and fleshed-out game entirely through the VScode terminal. I know at least some of us programmers have thought about it, and well, here it is!  

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
- **VScode**  

## **Library Extensions**  
- .NET Install Tool  
- C#  
- C# Dev Kit  
- GitHub Copilot  
- GitHub Copilot Chat  
- IntelliCode for C# Dev Kit  

The code is written in **C#**.  

# Useful Websites  
- [ChatGPT](https://chatgpt.com) - Used to learn new code and libraries.  
- [MonsterWorld](https://scratch.mit.edu/projects/228016745/) - Game of Inspiration on Scratch.  

# **Future Work**  
- None (for now)  

 # **Update Log**
 ### **1.05 - Updated Upgrading System and Leveling Up Abilities/New Class Features** 
 - **Upgraded Upgrading System**  
     - Player can now select abilities that can upgrade.  
 - **New Class Structure**  
     - Changed how all classes are stored in a JSON file. Added new class functions.  
 - **Class Progressions with Abilities**  
     - Each level, the player gains a new ability from a set list stored in the player’s class.  

### **1.04 - Dynamic Enemy Generation and Scaling**  
- **New Enemy Spawning System**:  
  - Each round, **new enemies** are generated dynamically.  
  - Enemies are selected from a JSON-based enemy database.  
  - As rounds progress, **enemies get stronger**, gaining **more HP, shields, and stamina**.  
  - The number of enemies **increases each round**, making survival harder over time.  

- **Improved Enemy AI**:  
  - Enemies now **choose their abilities intelligently**, prioritizing the weakest players.  
  - AI now properly **manages stamina**, ensuring optimal attacks.  
  - Special AI logic added for **healers**, **buffers**, and **aggressive attackers**.  

- **New Enemies Added**:  
  - **Fire Elemental**, **Frost Wraith**, **Shadow Assassin**, **Stone Golem**, **Necromancer**, **Dragon**, **Vampire Lord**, **Thunder Titan**, and more!  
  - Enemies use **new abilities**, making battles more diverse and challenging.  

- **New Abilities Introduced**:  
  - "Burning Touch" (Fire damage over time)  
  - "Frozen Touch" (Freezes an enemy, making them skip turns)  
  - "Multi-Strike" (Hits multiple times in one attack)  
  - "Rock Smash" (High damage + stun)  
  - "Dark Pulse" (Weaken effect on all enemies)  
  - "Roar" (Fear effect causing enemy hesitation)  
  - "Life Drain" (Steals health from the enemy)  

### **1.03 - Abilities Revamp and New Status Effects**  
- **Revamping Abilities**:  
  - Characters can now use **abilities** or **cards** to attack creatures.  
  - Simplified ability addition by defining just a few parameters; the code handles the rest.  
  - All creatures (not just **PlayerCreatures**) can now use abilities. (though no enumy AI has used this yet)

- **New Ability Behavior Rules**:  
  - Introduced new **targeting types**:  
    - **Single Target**: Targets a single player from the team.  
    - **Random Target**: Chooses a random target from the set team.  
    - **All Targets**: Targets all creatures on the team.  
  - Ability effects include **Damage**, **Healing**, **Shielding**, and **Status Effects**.  
  - Ability effects now include **Normal Damage**, **Piercing Damage**, **Health Restoration**, **Gold Health** (planned), and more.  

- **New Status Effects**:  
  - Added **Burning 🔥**, **Frozen ❄**, **Bleeding 🩸**, **Poison ☠️**, **Stun ⚡**, **Weaken 🛑**, and **Regeneration 🌿**.  
  - Effects now expire automatically at the end of their duration or cause **damage over time**.  
  - Each effect has a specific **type**, **description**, and can be applied based on the action taken.

### **1.02 - Player Leveling, Upgrades, and Balance Changes**  
- **Added Player Leveling**:  
  - After each battle, **players now level up** and can choose upgrades.  
  - Players **gain 1 Upgrade Point** per level-up.  
  - They can **save points** for stronger upgrades later.  

- **Upgrade System Introduced**:  
  - Players spend **Upgrade Points** on **Max HP, Stamina, and Abilities**.  
  - **More powerful upgrades cost more points**, creating strategic choices.  
  - Abilities can now **evolve or gain additional effects**.  

- **Reworked Card & Effect System**:  
  - Effects are now **modular**, making it **easier to add new abilities**.  
  - Some cards now **target all enemies or allies** for stronger combos.  
  - Fixed a **bug in Split-targeting cards**, ensuring effects apply correctly.  

- **Balance & Quality of Life Improvements**:  
  - Adjusted **enemy AI** to target players more strategically.  
  - Stamina costs adjusted for better **card play balance**.  
  - More descriptive **combat logs** for improved clarity.  

### **1.01 - Multiplayer & AI Improvements**  
- Added **hot couch multiplayer** (local turn-based mode).  
- Improved **card system** with better targeting mechanics.  
- Enhanced **enemy AI** for more engaging combat.  
- Fixed **small bugs** and general improvements.  

### **1.00 - Initial Release**  
- Published **RPGCombat** with the first combat system.  

