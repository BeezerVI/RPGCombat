using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RPGCombatProject.GameLogic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public static class PlayerLoader
    {
        private static readonly Dictionary<string, PlayerClassData> playerClassRegistry = new Dictionary<string, PlayerClassData>();

        // Static constructor loads player class data when the game starts.
        static PlayerLoader()
        {
            LoadPlayerClassesFromJson("playerClasses.json");
        }

        private static void LoadPlayerClassesFromJson(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Creature", fileName);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"❌ Error: {filePath} not found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var classes = JsonSerializer.Deserialize<List<PlayerClassData>>(json);

                if (classes != null)
                {
                    foreach (var playerClass in classes)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(playerClass.ClassName))
                            {
                                string key = playerClass.ClassName.ToLower();
                                playerClassRegistry[key] = playerClass;
                                Console.WriteLine($"✅ Loaded player class: {playerClass.ClassName}");
                            }
                            else
                            {
                                Console.WriteLine("⚠️ Skipping a class entry with missing ClassName.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Failed to load player class '{playerClass?.ClassName ?? "unknown"}': {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to read player class file: {ex.Message}");
            }
        }


        // Creates a new PlayerCreature using the data loaded from JSON.
        public static PlayerCreature CreatePlayer(string name, string chosenClass)
        {
            string key = chosenClass.Trim().ToLower();
            if (playerClassRegistry.TryGetValue(key, out PlayerClassData? classData) && classData != null)
            {
                var player = new PlayerCreature(
                    name,
                    classData.MaxHealth,
                    classData.MaxHealth,  // Start at full health
                    classData.Shield,
                    classData.Stamina,
                    classData.MaxStamina,
                    effects: null,
                    hand: GetDefaultHand(classData.DefaultAbilities),
                    className: classData.ClassName,
                    level: classData.StartingLevel,
                    upgradePoints: 0
                );
                // Initialize the player's ability progression from the JSON data.
                player.AbilityProgression = classData.AbilityProgression;
                player.NextAbilityIndex = 0;
                return player;
            }
            else
            {
                Console.WriteLine($"Player class '{chosenClass}' not found. Defaulting to Warrior.");
                return CreatePlayer(name, "warrior");
            }
        }


        private static List<Ability> GetDefaultHand(List<string> abilityNames)
        {
            List<Ability> hand = new List<Ability>();
            foreach (string abilityName in abilityNames)
            {
                try
                {
                    hand.Add(Abilities.GetAbility(abilityName));
                }
                catch (ArgumentException)
                {
                    Console.WriteLine($"Warning: Ability '{abilityName}' not found for player class default hand.");
                }
            }
            return hand;
        }
    }

    // Class for JSON deserialization of player classes.
    public class PlayerClassData
    {
        public int MaxHealth { get; set; }
        public int Shield { get; set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }
        public List<string> DefaultAbilities { get; set; } = new List<string>();
        public int StartingLevel { get; set; }
        public string ClassName { get; set; } = "";
        public List<string> AbilityProgression { get; set; } = new List<string>();  // for ability progression.
    }
}
