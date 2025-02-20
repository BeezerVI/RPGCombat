using System;
using System.Collections.Generic;
using System.Linq;

namespace CardSystemDemo
{
    // ---------------------------
    // Interfaces & Effect Classes
    // ---------------------------

    // Interface for card effects.
    public interface ICardEffect
    {
        // Apply the effect to a given creature.
        void Apply(Creature target);
        // Return a short description of this effect.
        string GetDescription();
    }

    // A damage effect that subtracts health.
    public class DamageEffect : ICardEffect
    {
        public int Damage { get; }
        public DamageEffect(int damage)
        {
            Damage = damage;
        }
        public void Apply(Creature target)
        {
            target.ApplyDamage(Damage);
            Console.WriteLine($"Dealt {Damage} damage to {target.Name}.");
        }
        public string GetDescription() => $"Deal {Damage} damage";
    }

    // A heal effect that restores health.
    public class HealEffect : ICardEffect
    {
        public int Healing { get; }
        public HealEffect(int healing)
        {
            Healing = healing;
        }
        public void Apply(Creature target)
        {
            target.ApplyHealing(Healing);
            Console.WriteLine($"{target.Name} healed for {Healing} HP.");
        }
        public string GetDescription() => $"Heal {Healing} HP";
    }

    // A shield effect that grants a protective shield.
    public class ShieldEffect : ICardEffect
    {
        public int ShieldAmount { get; }
        public ShieldEffect(int shieldAmount)
        {
            ShieldAmount = shieldAmount;
        }
        public void Apply(Creature target)
        {
            target.ApplyShield(ShieldAmount);
            Console.WriteLine($"{target.Name} gained {ShieldAmount} shield.");
        }
        public string GetDescription() => $"Grant {ShieldAmount} shield";
    }

    // ---------------------------
    // Target Selection Interfaces & Classes
    // ---------------------------

    // Interface for choosing targets.
    public interface ITargetSelector
    {
        // Given a list of candidate creatures, select one or more targets.
        List<Creature> SelectTargets(List<Creature> candidates);
        // Provide a textual description of the target selection type.
        string GetTargetDescription();
    }

    // A selector that prompts the user to choose one target.
    public class SingleTargetSelector : ITargetSelector
    {
        public List<Creature> SelectTargets(List<Creature> candidates)
        {
            if (candidates.Count == 0)
            {
                Console.WriteLine("No available targets.");
                return new List<Creature>();
            }

            Console.WriteLine("Choose a target:");
            for (int i = 0; i < candidates.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {candidates[i].Name} (HP: {candidates[i].Health}/{candidates[i].MaxHealth})");
            }

            Console.Write("Enter target number: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int index) && index >= 1 && index <= candidates.Count)
            {
                return new List<Creature> { candidates[index - 1] };
            }
            Console.WriteLine("Invalid selection.");
            return new List<Creature>();
        }

        public string GetTargetDescription() => "Single target";
    }

    // A selector that automatically chooses all available candidates.
    public class AllTargetsSelector : ITargetSelector
    {
        public List<Creature> SelectTargets(List<Creature> candidates) => candidates;
        public string GetTargetDescription() => "All targets";
    }

    // ---------------------------
    // Card Class & Factory
    // ---------------------------

    // The Card class composes a name, cost, a target selector, and one or more effects.
    public class Card
    {
        public string Name { get; }
        public int Cost { get; }
        public ITargetSelector TargetSelector { get; }
        public List<ICardEffect> Effects { get; }

        // The description is auto-generated from its components.
        public string Description
        {
            get
            {
                string effectsDesc = string.Join(", ", Effects.Select(e => e.GetDescription()));
                return $"{Name}: Cost {Cost}. Effects: {effectsDesc}. Targeting: {TargetSelector.GetTargetDescription()}.";
            }
        }

        public Card(string name, int cost, ITargetSelector targetSelector, List<ICardEffect> effects)
        {
            Name = name;
            Cost = cost;
            TargetSelector = targetSelector;
            Effects = effects;
        }

        // Play the card: deduct cost, choose targets, and apply each effect to every target.
        public void Play(GameState gameState, PlayerCreature actor)
        {
            if (actor.Stamina < Cost)
            {
                Console.WriteLine("Not enough stamina to play this card.");
                return;
            }

            actor.Stamina -= Cost;

            // For demonstration, if the card has a damage effect, assume the targets are enemies;
            // otherwise, assume the targets are from the player's team.
            List<Creature> candidates = Effects.Any(e => e is DamageEffect)
                ? gameState.EnemyTeam
                : gameState.PlayerTeam;

            List<Creature> targets = TargetSelector.SelectTargets(candidates);
            if (targets.Count == 0)
            {
                Console.WriteLine("No valid targets selected.");
                return;
            }
            foreach (var target in targets)
            {
                foreach (var effect in Effects)
                {
                    effect.Apply(target);
                }
            }
            Console.WriteLine($"{actor.Name} played {Name}.");
        }
    }

    // A simple factory to create cards based on an ID or name.
    public static class CardFactory
    {
        public static Card CreateCard(string cardId)
        {
            switch (cardId)
            {
                case "Sword":
                    return new Card(
                        name: "Sword",
                        cost: 1,
                        targetSelector: new SingleTargetSelector(),
                        effects: new List<ICardEffect> { new DamageEffect(6) }
                    );
                case "Heal":
                    return new Card(
                        name: "Heal",
                        cost: 1,
                        targetSelector: new SingleTargetSelector(),
                        effects: new List<ICardEffect> { new HealEffect(10) }
                    );
                case "Defend":
                    return new Card(
                        name: "Defend",
                        cost: 1,
                        targetSelector: new SingleTargetSelector(),
                        effects: new List<ICardEffect> { new ShieldEffect(4) }
                    );
                case "Whirlwind":
                    // Example: an area-of-effect card that deals damage to all enemies.
                    return new Card(
                        name: "Whirlwind",
                        cost: 2,
                        targetSelector: new AllTargetsSelector(),
                        effects: new List<ICardEffect> { new DamageEffect(4) }
                    );
                default:
                    Console.WriteLine($"Warning: No preset defined for card '{cardId}'. Using default Sword.");
                    return CreateCard("Sword");
            }
        }
    }

    // ---------------------------
    // Game State & Creature Classes
    // ---------------------------

    // A basic Creature class.
    public class Creature
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Shield { get; set; }

        public Creature(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Shield = 0;
        }

        public void ApplyDamage(int damage)
        {
            // Damage is reduced by the shield value.
            int effectiveDamage = Math.Max(0, damage - Shield);
            Shield = Math.Max(0, Shield - damage);
            Health -= effectiveDamage;
            if (Health < 0) Health = 0;
        }

        public void ApplyHealing(int healing)
        {
            Health = Math.Min(MaxHealth, Health + healing);
        }

        public void ApplyShield(int shieldAmount)
        {
            Shield += shieldAmount;
        }
    }

    // PlayerCreature inherits from Creature and includes stamina.
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }
        public PlayerCreature(string name, int maxHealth, int stamina)
            : base(name, maxHealth)
        {
            Stamina = stamina;
        }
    }

    // Game state holding both enemy and player teams.
    public class GameState
    {
        public List<Creature> EnemyTeam { get; set; }
        public List<Creature> PlayerTeam { get; set; }
        public GameState(List<Creature> enemyTeam, List<Creature> playerTeam)
        {
            EnemyTeam = enemyTeam;
            PlayerTeam = playerTeam;
        }
    }

    // ---------------------------
    // Main Program for Testing
    // ---------------------------
    class Program
    {
        static void Main(string[] args)
        {
            // Create sample enemies and player team.
            List<Creature> enemies = new List<Creature>
            {
                new Creature("Goblin", 20),
                new Creature("Orc", 30)
            };

            List<Creature> players = new List<Creature>
            {
                new PlayerCreature("Hero", 50, 5)
            };

            GameState gameState = new GameState(enemies, players);
            PlayerCreature hero = (PlayerCreature)players[0];

            // Create a Sword card and play it.
            Card swordCard = CardFactory.CreateCard("Sword");
            Console.WriteLine(swordCard.Description);
            swordCard.Play(gameState, hero);

            Console.WriteLine("\n---\n");

            // Create and play a Heal card.
            Card healCard = CardFactory.CreateCard("Heal");
            Console.WriteLine(healCard.Description);
            healCard.Play(gameState, hero);

            // Pause before exit.
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
