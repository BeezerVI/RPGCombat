using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGCombatProject.Models
{
    public class EnemyCreature : Creature
    {
        // Existing constructor for custom initialization.
        public EnemyCreature(string name, int maxHealth = 0, int health = 0, int shield = 0, List<Effect>? effects = null)
            : base(name, maxHealth, health, shield, effects)
        {
        }

        // New constructor: When you only supply a name, default stats and AI behavior are assigned.
        public EnemyCreature(string name)
            : base(name, 0, 0, 0, new List<Effect>())
        {
            // Set default stats based on enemy type.
            switch (name.ToLower())
            {
                case "slime":
                    MaxHealth = 40;
                    Health = 40;
                    Shield = 0;
                    break;
                case "giant bug":
                    MaxHealth = 30;
                    Health = 30;
                    Shield = 8;
                    break;
                default:
                    MaxHealth = 40;
                    Health = 40;
                    Shield = 0;
                    break;
            }
        }

        /// <summary>
        /// Performs the enemy’s AI-based action. This method is designed to work on a list of viable player targets.
        /// </summary>
        /// <param name="players">The list of alive player creatures.</param>
        public void Act(List<Creature> players)
        {
            if (players == null || players.Count == 0) return;

            // Switch on the enemy's name (in lower case) to decide its behavior.
            switch (Name.ToLower())
            {
                case "slime":
                    // For slime: randomly choose a small attack (5 damage) or a big attack (10 damage).
                    var target = players.OrderBy(p => p.Health).First();
                    int attackChoice = new Random().Next(2); // returns 0 or 1
                    if (attackChoice == 0)
                    {
                        Console.WriteLine($"{Name} uses a small attack on {target.Name} for 5 damage!");
                        target.ApplyDamage(5);
                    }
                    else
                    {
                        Console.WriteLine($"{Name} uses a big attack on {target.Name} for 10 damage!");
                        target.ApplyDamage(10);
                    }
                    break;

                case "giant bug":
                    // For giant bug: deal 10 damage and grant self 8 shield.
                    var target2 = players.OrderBy(p => p.Health).First();
                    Console.WriteLine($"{Name} attacks {target2.Name} for 10 damage and gains 8 shield!");
                    target2.ApplyDamage(10);
                    Shield += 8;
                    break;

                default:
                    // Default behavior: attack the player with the lowest health for 5 damage.
                    var targetDefault = players.OrderBy(p => p.Health).First();
                    Console.WriteLine($"{Name} attacks {targetDefault.Name} for 5 damage!");
                    targetDefault.ApplyDamage(5);
                    break;
            }
        }
    }
}
