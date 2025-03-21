using System.Collections.Generic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Level { get; set; }
        public int UpgradePoints { get; set; }

        public string ClassName  { get; set; }

        public PlayerCreature(string name, int maxHealth, int health, int shield = 0, List<Effect>? effects = null, int stamina = 3, int maxStamina = 3, List<Ability>? hand = null, string className = "warrior", int level = 1, int upgradePoints = 0)
            : base(name, maxHealth, health, shield, effects, stamina, maxStamina, hand)
        {
            Level = level;
            UpgradePoints = upgradePoints;
            ClassName = className;
        }

        // Factory method: creates a new player based on the chosen class.
        public static PlayerCreature CreatePlayer(string name, string chosenClass)
        {
            switch (chosenClass.ToLower())
            {
                case "warrior":
                    return new PlayerCreature(
                        name,
                        maxHealth: 120,    // Warrior has higher HP
                        health: 120,
                        shield: 15,        // Better starting shield
                        stamina: 3,
                        maxStamina: 3,     // Initialize MaxStamina
                        hand: GetDefaultHand("warrior"),
                        className: "warrior",
                        level: 1
                    );
                case "mage":
                    return new PlayerCreature(
                        name,
                        maxHealth: 80,     // Mage has lower HP
                        health: 80,
                        shield: 5,
                        stamina: 4,
                        maxStamina: 4,     // Initialize MaxStamina
                        hand: GetDefaultHand("mage"),
                        className: "mage",
                        level: 1
                    );
                case "rogue":
                    return new PlayerCreature(
                        name,
                        maxHealth: 100,    // Rogue has moderate HP
                        health: 100,
                        shield: 10,
                        stamina: 4,
                        maxStamina: 4,     // Initialize MaxStamina
                        hand: GetDefaultHand("rogue"),
                        className: "rogue",
                        level: 1
                    );

                case "Phoenix":
                    return new PlayerCreature(
                        name,
                        maxHealth: 100,    // Phoenix has moderate HP
                        health: 100,
                        shield: 10,
                        stamina: 1000,
                        maxStamina: 1000,     // Initialize MaxStamina
                        hand: GetDefaultHand("phoenix"),
                        className: "phoenix",
                        level: 1
                    );
                default:
                    // Default to Warrior if an invalid class is chosen.
                    return new PlayerCreature(
                        name,
                        maxHealth: 120,
                        health: 120,
                        shield: 15,
                        stamina: 3,
                        maxStamina: 3,     // Initialize MaxStamina
                        hand: GetDefaultHand("warrior"),
                        className: "warrior",
                        level: 1
                    );
            }
        }

        // Returns a default hand of Abilitys based on the player's class.
        private static List<Ability> GetDefaultHand(string playerClass)
        {
            switch (playerClass.ToLower())
            {
                case "warrior":
                    return new List<Ability>
                    {
                        Abilities.GetAbility("Sword Strike"),
                        Abilities.GetAbility("Heavy Slash"),
                        Abilities.GetAbility("Fortify"),
                    };
                    
                case "mage":
                    return new List<Ability>
                    {
                        Abilities.GetAbility("Fireball"),
                        Abilities.GetAbility("Lightning Bolt"),
                        Abilities.GetAbility("Mass Heal"),
                    };

                case "rogue":
                    return new List<Ability>
                    {
                        Abilities.GetAbility("Poison"),
                        Abilities.GetAbility("Stun"),
                        Abilities.GetAbility("Speed Boost"),
                       // Abilities.GetAbility("Dagger Strike"),
                    };

                case "phoenix":
                    return new List<Ability>
                    {
                        Abilities.GetAbility("Fireball"),
                        Abilities.GetAbility("Lightning Bolt"),
                        Abilities.GetAbility("Mass Heal"),
                    };

                default:
                    return new List<Ability>
                    {
                        Abilities.GetAbility("Sword Strike"),
                        Abilities.GetAbility("Dagger Strike"),
                        Abilities.GetAbility("Heavy Slash"),
                        Abilities.GetAbility("Piercing Strike"),
                        Abilities.GetAbility("Fireball"),
                        Abilities.GetAbility("Lightning Bolt"),
                        Abilities.GetAbility("Ice Shard"),
                        Abilities.GetAbility("Meteor Strike"),
                        Abilities.GetAbility("Heal"),
                        Abilities.GetAbility("Mass Heal"),
                        Abilities.GetAbility("Regeneration"),
                        Abilities.GetAbility("Revive"),
                        Abilities.GetAbility("Fortify"),
                        Abilities.GetAbility("Magic Barrier"),
                        Abilities.GetAbility("Speed Boost"),
                        Abilities.GetAbility("Weaken"),
                        Abilities.GetAbility("Poison"),
                        Abilities.GetAbility("Stun"),
                        Abilities.GetAbility("Shield Breaker"),
                        Abilities.GetAbility("Hammer Slam"),
                    };
            }
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
                    // Display game state
                    UIManager.DisplayGameState();

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
            UpgradePoints += 1;
            Console.WriteLine($"{Name} leveled up to Level {Level}! Gained 1 Upgrade Point.");
        }
    }
}