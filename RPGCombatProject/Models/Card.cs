using System;
using System.Collections.Generic;
using RPGCombatProject.Models;

namespace RPGCombatProject.Models
{
    public class Card
    {
        public string Name { get; set; }
        public int Actions { get; set; }
        public string CardAbilitys { get; set; }

        public Card(string name, int actions, string cardAbilitys)
        {
            Name = name;
            Actions = actions;
            CardAbilitys = cardAbilitys;
        }

        // NEW: Ability now accepts the acting creature.
        public void Ability(List<Creature> enemyTeam, List<Creature> playerTeam, Creature actor)
        {
            switch (Name)
            {
                case "Sword":
                    // Use the actor's target if set; otherwise, default to the first enemy.
                    Creature targetEnemy = actor.Target ?? enemyTeam[0];
                    targetEnemy.ApplyDamage(6);
                    Console.WriteLine($"Dealt 6 damage to {targetEnemy.Name}.");
                    break;
                case "Deflect":
                    // Self-targeted ability.
                    actor.ApplyShield(4);
                    Console.WriteLine($"Gained 4 Shield for {actor.Name}.");
                    break;
                case "Frost":
                    foreach (var enemy in enemyTeam)
                    {
                        enemy.ApplyDamage(1);
                        enemy.ApplyEffect(new Effect("Frost", 3, 1));
                    }
                    Console.WriteLine("Dealt 1 damage to all enemies and afflicted 3 Frost.");
                    break;
                case "One Shot":
                    // Example: target enemy is set on actor.Target (or default to first enemy)
                    Creature target = actor.Target ?? enemyTeam[0];
                    target.ApplyDamage(target.Health); // Reduce health to 0
                    Console.WriteLine($"{target.Name} has been one-shotted!");
                    break;
                default:
                    Console.WriteLine("Card ability not implemented.");
                    break;
            }
        }
    }
}
