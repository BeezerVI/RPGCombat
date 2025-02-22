using System.Collections.Generic;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }
        public List<Card> Hand { get; set; }

        public PlayerCreature(string name, int maxHealth, int health, int shield = 0, int stamina = 0, List<Card>? hand = null, string className = "warrior", int level = 0)
            : base(name, maxHealth, health, shield)
        {
            Stamina = stamina;
            Hand = hand ?? new List<Card>();
        }

        // Factory method: creates a new player based on the chosen class.
        public static PlayerCreature CreatePlayer(string name, string chosenClass)
        {
            // Define default stats and hand per class.
            switch (chosenClass.ToLower())
            {
                case "warrior":
                    return new PlayerCreature(
                        name,
                        maxHealth: 120,    // Higher HP for Warrior
                        health: 120,
                        shield: 15,        // Better starting shield
                        stamina: 3,
                        hand: GetDefaultHand("warrior"),
                        className: "warrior",
                        level: 0
                    );
                case "mage":
                    return new PlayerCreature(
                        name,
                        maxHealth: 80,     // Lower HP for Mage
                        health: 80,
                        shield: 5,         // Lower shield, but maybe higher action cost or special cards
                        stamina: 4,
                        hand: GetDefaultHand("mage"),
                        className: "mage",
                        level: 0
                    );
                case "rogue":
                    return new PlayerCreature(
                        name,
                        maxHealth: 100,    // Moderate HP for Rogue
                        health: 100,
                        shield: 10,        // Moderate shield
                        stamina: 4,
                        hand: GetDefaultHand("rogue"),
                        className: "rogue",
                        level: 0
                    );
                default:
                    // Default to Warrior if no valid class was chosen.
                    return new PlayerCreature(
                        name,
                        maxHealth: 120,
                        health: 120,
                        shield: 15,
                        stamina: 3,
                        hand: GetDefaultHand("warrior"),
                        className: "warrior",
                        level: 0
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
                        // You could add a special Rogue card here.
                    };
                default:
                    return new List<Card>
                    {
                        new Card("Sword"),
                        new Card("Deflect")
                    };
            }
        }
    }
}
