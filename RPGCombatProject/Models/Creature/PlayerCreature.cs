using System.Collections.Generic;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }
        public List<Card> Hand { get; set; }

        public PlayerCreature(string name, int maxHealth, int health, int shield, int stamina, List<Card>? hand = null)
            : base(name, maxHealth, health, shield)
        {
            Stamina = stamina;
            Hand = hand ?? new List<Card>();
        }

        public override void Attack(Creature target)
        {
            System.Console.WriteLine($"{Name} attacks {target.Name} for 10 damage!");
            target.ApplyDamage(10);
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
                        hand: GetDefaultHand("warrior")
                    );
                case "mage":
                    return new PlayerCreature(
                        name,
                        maxHealth: 80,     // Lower HP for Mage
                        health: 80,
                        shield: 5,         // Lower shield, but maybe higher action cost or special cards
                        stamina: 4,
                        hand: GetDefaultHand("mage")
                    );
                case "rogue":
                    return new PlayerCreature(
                        name,
                        maxHealth: 100,    // Moderate HP for Rogue
                        health: 100,
                        shield: 10,        // Moderate shield
                        stamina: 4,
                        hand: GetDefaultHand("rogue")
                    );
                default:
                    // Default to Warrior if no valid class was chosen.
                    return new PlayerCreature(
                        name,
                        maxHealth: 120,
                        health: 120,
                        shield: 15,
                        stamina: 3,
                        hand: GetDefaultHand("warrior")
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
