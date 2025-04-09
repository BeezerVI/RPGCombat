using System;
using System.Collections.Generic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Level { get; set; }
        public int UpgradePoints { get; set; }
        public string ClassName { get; set; }
        public List<string> AbilityProgression { get; set; } = new List<string>();
        public int NextAbilityIndex { get; set; } = 0;

        public PlayerCreature(string name, int maxHealth, int health, int shield, int stamina, int maxStamina, List<Effect>? effects = null, List<Ability>? hand = null, string className = "warrior", int level = 1, int upgradePoints = 0)
            : base(name, maxHealth, health, shield, stamina, maxStamina, effects, hand)
        {
            Level = level;
            UpgradePoints = upgradePoints;
            ClassName = className;
        }

        public override void PlayTurn()
            {
            // Check if the player is stunned
            if (IsStunned())
            {
                UIManager.Write($"{Name} is stunned and skips their turn.");
                ProcessEffects();
                return;
            }
            ProcessEffects();            
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
                Console.WriteLine("3. Upgrade an Ability (Cost: 1)");
                Console.WriteLine("4. Save Upgrade Points for later");
                Console.Write("Enter your choice (1-4): ");

                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid choice. Try again."); continue;
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
                        if (UpgradePoints < 1)
                        {
                            Console.WriteLine("Not enough points!"); break;
                        }
                        // List upgradable abilities
                        var upgradable = Hand.Where(a => !string.IsNullOrEmpty(a.UpgradedTo)).ToList();
                        if (!upgradable.Any())
                        {
                            Console.WriteLine("No abilities available to upgrade."); break;
                        }
                        Console.WriteLine("Select ability to upgrade:");
                        for (int i = 0; i < upgradable.Count; i++)
                            Console.WriteLine($"{i+1}. {upgradable[i].Name} -> {upgradable[i].UpgradedTo}");
                        Console.Write("Choice: ");
                        if (int.TryParse(Console.ReadLine(), out int idx)
                            && idx >= 1 && idx <= upgradable.Count)
                        {
                            var oldAbility = upgradable[idx-1];
                            var newAbility = Abilities.GetAbility(oldAbility.UpgradedTo!);
                            Hand.Remove(oldAbility);
                            Hand.Add(newAbility);
                            UpgradePoints--;
                            Console.WriteLine($"Upgraded {oldAbility.Name} to {newAbility.Name}!");
                        }
                        else Console.WriteLine("Invalid selection.");
                        break;
                    case 4:
                        Console.WriteLine($"{Name} saved their Upgrade Points for later.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // Call this function after combat to level up and gain upgrade points.
        public void LevelUp()
            {
                Level++;
                UpgradePoints++;

                // Unlock next ability if available
                if (NextAbilityIndex < AbilityProgression.Count)
                {
                    string abilityName = AbilityProgression[NextAbilityIndex];
                    var ability = Abilities.GetAbility(abilityName);
                    Hand.Add(ability);
                    NextAbilityIndex++;
                    Console.WriteLine($"{Name} learned new ability: {abilityName}!");
                }

                Console.WriteLine($"{Name} leveled up to Level {Level}! Gained 1 Upgrade Point.");
            }
    }
}
