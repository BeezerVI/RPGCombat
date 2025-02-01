using System.Collections.Generic;

namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }
        // New property: each player has their own hand of cards.
        public List<Card> Hand { get; set; }

        public PlayerCreature(string name, int maxHealth = 100, int health = 100, int shield = 0, int stamina = 50, List<Card>? hand = null)
            : base(name, maxHealth, health, shield)
        {
            Stamina = stamina;
            // Initialize the hand with either the provided hand or a new list.
            Hand = hand ?? new List<Card>();
        }

        public override void Attack(Creature target)
        {
            // Example attack implementation.
            System.Console.WriteLine($"{Name} attacks {target.Name} for 10 damage!");
            target.ApplyDamage(10);
        }
    }
}
