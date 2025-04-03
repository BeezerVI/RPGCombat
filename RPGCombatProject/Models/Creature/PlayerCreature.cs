using System.Collections.Generic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Level { get; set; }
        public int UpgradePoints { get; set; }
        public string ClassName { get; set; }

        public PlayerCreature(string name, int maxHealth, int health, int shield, int stamina, int maxStamina, List<Effect>? effects = null, List<Ability>? hand = null, int level = 1, string className = "warrior", int upgradePoints = 0)
            : base(name, maxHealth, health, shield, stamina, maxStamina, effects, hand)
        {
            Level = level;
            UpgradePoints = upgradePoints;
            ClassName = className;
        }

        public override void PlayTurn()
            {
                ProcessEffects();
                // Check if the player is stunned
                if (IsStunned())
                {
                    UIManager.Write($"{Name} is stunned and skips their turn.");
                    return;
                }

            Stamina = MaxStamina; // Reset stamina at the start of the turn
            UIManager.Write($"{Name}'s turn begins.");

            bool isTurn = true;
            while (isTurn)
            {
                // Display game state and handle player options.
                UIManager.DisplayGameState();
                UIManager.CombatOptions(this);
                isTurn = UIManager.PlayerCombatOptions(this);
            }
            UIManager.Write($"{Name}'s turn is over.");
        }

        public void UpgradeMenu()
        {
            while (true)
            {
                Console.WriteLine($"\n--- Upgrade Store (Points: {UpgradePoints}) ---");
                Console.WriteLine("1. Increase Max HP by 10 (Cost: 1)");
                Console.WriteLine("2. Increase Stamina by 1 (Cost: 2)");
                Console.WriteLine("3. Upgrade Abilities (Unlock new Ability / improve existing ones) (Cost: 1)");
                Console.WriteLine("4. Save Upgrade Points for later");
                Console.Write("Enter your choice (1-4): ");

                string? input = Console.ReadLine();
                int choice;
                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Invalid choice. Try again.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        if (UpgradePoints >= 1)
                        {
                            MaxHealth += 10;
                            Health += 10; // Also boost current health
                            UpgradePoints -= 1;
                            Console.WriteLine($"{Name}'s Max HP increased to {MaxHealth}.");
                        }
                        else
                        {
                            Console.WriteLine("Not enough Upgrade Points!");
                        }
                        break;

                    case 2:
                        if (UpgradePoints >= 2)
                        {
                            MaxStamina += 1; // Increase MaxStamina
                            Stamina = MaxStamina; // Reset current Stamina to MaxStamina
                            UpgradePoints -= 2;
                            Console.WriteLine($"{Name}'s Max Stamina increased to {MaxStamina}.");
                        }
                        else
                        {
                            Console.WriteLine("Not enough Upgrade Points!");
                        }
                        break;

                    case 3:
                        if (UpgradePoints >= 1)
                        {
                            Console.WriteLine("Ability upgrade is not implemented yet.");
                            UpgradePoints -= 1;
                        }
                        else
                        {
                            Console.WriteLine("Not enough Upgrade Points!");
                        }
                        break;

                    case 4:
                        Console.WriteLine($"{Name} saved their Upgrade Points for later.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        // Call this function after combat to level up and gain upgrade points.
        public void LevelUp()
        {
            Level += 1;
            UpgradePoints += 3;
            Console.WriteLine($"{Name} leveled up to Level {Level}! Gained 3 Upgrade Point.");
        }
    }
}
