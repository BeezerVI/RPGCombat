using System.Collections.Generic;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }
        public List<Card> Hand { get; set; }
        public int Level { get; set; }
        public int UpgradePoints { get; set; }

        public PlayerCreature(string name, int maxHealth, int health, int shield = 0, int stamina = 0, List<Card>? hand = null, string className = "warrior", int level = 1, int upgradePoints = 0)
            : base(name, maxHealth, health, shield)
        {
            Stamina = stamina;
            Hand = hand ?? new List<Card>();
            Level = level;
            UpgradePoints = upgradePoints;
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
                        hand: GetDefaultHand("rogue"),
                        className: "rogue",
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
                        hand: GetDefaultHand("warrior"),
                        className: "warrior",
                        level: 1
                    );
            }
        }

        // Returns a default hand of cards based on the player's class.
        private static List<Card> GetDefaultHand(string playerClass)
        {
            switch (playerClass.ToLower())
            {
                case "warrior":
                    return new List<Card>
                    {
                        new Card("Sword"),
                        new Card("Deflect"),
                        new Card("One Shot")
                    };
                case "mage":
                    return new List<Card>
                    {
                        new Card("Frost"),
                        new Card("Heal"),
                        new Card("Deflect")
                    };
                case "rogue":
                    return new List<Card>
                    {
                        new Card("Sword"),
                        new Card("Deflect")
                        // You can add a special Rogue card here.
                    };
                default:
                    return new List<Card>
                    {
                        new Card("Sword"),
                        new Card("Deflect")
                    };
            }
        }

        // New method: Prompts the player to choose an upgrade from the upgrade store.
        public void UpgradeMenu()
        {
            while (true)
            {
                Console.WriteLine($"\n--- Upgrade Store (Points: {UpgradePoints}) ---");
                Console.WriteLine("1. Increase Max HP by 10 (Cost: 1)");
                Console.WriteLine("2. Increase Stamina by 1 (Cost: 2)");
                Console.WriteLine("3. Upgrade Abilities (Unlock new card / improve existing ones) (Cost: 1)");
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
                            Stamina += 1;
                            UpgradePoints -= 2;
                            Console.WriteLine($"{Name}'s Stamina increased to {Stamina}.");
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
