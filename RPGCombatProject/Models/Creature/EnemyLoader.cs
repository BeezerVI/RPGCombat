using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RPGCombatProject.GameLogic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public static class EnemyLoader
    {
        private static readonly Dictionary<string, EnemyCreature> enemyRegistry = new Dictionary<string, EnemyCreature>();

        // Static constructor loads enemies when the game starts.
        static EnemyLoader()
        {
            LoadEnemiesFromJson("enemies.json");
        }

        private static void LoadEnemiesFromJson(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Creature", fileName);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"❌ Error: {filePath} not found.");
                UIManager.Write("Error loading enemies. Press any key to exit.", clearConsole: 3);
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var enemies = JsonSerializer.Deserialize<List<EnemyData>>(json);

                if (enemies != null)
                {
                    foreach (var enemyData in enemies)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(enemyData.Name))
                            {
                                Console.WriteLine("⚠️ Skipping enemy with missing name.");
                                continue;
                            }

                            List<Ability> abilities = new List<Ability>();
                            if (enemyData.Abilities != null)
                            {
                                foreach (var abilityName in enemyData.Abilities)
                                {
                                    try
                                    {
                                        abilities.Add(Abilities.GetAbility(abilityName));
                                    }
                                    catch (ArgumentException)
                                    {
                                        Console.WriteLine($"⚠️ Warning: Ability '{abilityName}' not found for enemy '{enemyData.Name}'.");
                                    }
                                }
                            }

                            var enemy = new EnemyCreature(
                                enemyData.Name,
                                enemyData.MaxHealth,
                                enemyData.MaxHealth,
                                enemyData.Shield,
                                enemyData.Stamina,
                                enemyData.MaxStamina,
                                new List<Effect>(),  // Optional: add default effects
                                abilities
                            );

                            enemyRegistry[enemyData.Name.ToLower()] = enemy;
                            Console.WriteLine($"✅ Loaded enemy: {enemyData.Name}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Failed to load enemy entry: {enemyData?.Name ?? "unknown"} - {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load enemies.json: {ex.Message}");
            }
        }


        // Retrieves an enemy by name
        public static EnemyCreature GetEnemy(string enemyName)
        {
            string formattedEnemyName = enemyName.Trim().ToLower();
            if (enemyRegistry.TryGetValue(formattedEnemyName, out EnemyCreature? enemy))
            {
                return new EnemyCreature(
                    enemy.Name,
                    enemy.MaxHealth,
                    enemy.MaxHealth,  // Clone to avoid modifying the stored template
                    enemy.Shield,
                    enemy.Stamina,
                    enemy.MaxStamina,
                    enemy.Effects != null ? new List<Effect>(enemy.Effects) : new List<Effect>(),
                    enemy.Hand != null ? new List<Ability>(enemy.Hand) : new List<Ability>()
                );
            }
            throw new ArgumentException($"Enemy '{formattedEnemyName}' not found.");
        }
    }

    // A helper class for JSON deserialization
    public class EnemyData
    {
        public string Name { get; set; } = string.Empty;
        public int MaxHealth { get; set; }
        public int Shield { get; set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }
        public List<string> Abilities { get; set; } = new List<string>();
    }
}
