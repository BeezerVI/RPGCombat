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

        public void Ability(List<Creature> enemyTeam, List<Creature> playerTeam, ref int enemyTargeted, ref int playerTargeted)
        {
            var targetEnemy = enemyTeam[enemyTargeted];
            var targetPlayer = playerTeam[playerTargeted];

            switch (Name)
            {
                case "Sword":
                    targetEnemy.ApplyDamage(6);
                    Console.WriteLine($"Dealt 6 damage to {targetEnemy.Name}.");
                    break;
                case "Deflect":
                    targetPlayer.ApplyShield(4);
                    Console.WriteLine($"Gained 4 Shield for {targetPlayer.Name}.");
                    break;
                case "Frost":
                    foreach (var enemy in enemyTeam)
                    {
                        enemy.ApplyDamage(1);
                        enemy.ApplyEffect(new Effect("Frost", 3, 1));
                    }
                    Console.WriteLine("Dealt 1 damage to all enemies and afflicted 3 Frost.");
                    break;
                default:
                    Console.WriteLine("Card ability not implemented.");
                    break;
            }
        }
    }
}
