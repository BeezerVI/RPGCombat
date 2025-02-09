using System;
using System.Collections.Generic;
using RPGCombatProject.GameLogic;

namespace RPGCombatProject.Models
{
    // Define the card's category (attack, heal, buff, etc.)
    public enum CardCategory
    {
        Attack,
        Heal,
        Buff
    }

    // Define the targeting type:
    // Single = the effect applies only to one target,
    // All = applies to every creature in the chosen group,
    // Split = first applies to one target, then an additional effect is applied to all.
    public enum CardTargetType
    {
        Single,
        All,
        Split
    }

    public class Card
    {
        public string Name { get; set; }
        public int Actions { get; set; }              // Cost in actions/stamina
        public string CardAbilitys { get; set; }        // Auto-generated description

        // Preset properties for the card effect
        public CardCategory Category { get; set; }
        public CardTargetType TargetType { get; set; }
        public int Damage { get; set; }                 // Primary damage (for attack cards)
        public int ShieldAmount { get; set; }           // Shield amount (for buff cards)
        public int Healing { get; set; }                // Healing amount (for heal cards)

        // For cards with split effects (like Frost), additional damage/effect applied to all
        public int AdditionalDamage { get; set; }
        public int AdditionalEffectDuration { get; set; }
        public int AdditionalEffectStrength { get; set; }

        // Constructor that allows you to specify all properties.
        public Card(string name, int actions, CardCategory category, CardTargetType targetType,
                    int damage = 0, int shieldAmount = 0, int healing = 0,
                    int additionalDamage = 0, int additionalEffectDuration = 0, int additionalEffectStrength = 0)
        {
            Name = name;
            Actions = actions;
            Category = category;
            TargetType = targetType;
            Damage = damage;
            ShieldAmount = shieldAmount;
            Healing = healing;
            AdditionalDamage = additionalDamage;
            AdditionalEffectDuration = additionalEffectDuration;
            AdditionalEffectStrength = additionalEffectStrength;
            CardAbilitys = GenerateDescription();
        }

        // NEW: Overloaded constructor that only requires the card name.
        // This constructor presets the remaining properties based on the name.
        public Card(string name)
        {
            Name = name;
            // Default values in case no preset is found
            Actions = 1;
            Category = CardCategory.Attack;
            TargetType = CardTargetType.Single;
            Damage = 0;
            ShieldAmount = 0;
            Healing = 0;
            AdditionalDamage = 0;
            AdditionalEffectDuration = 0;
            AdditionalEffectStrength = 0;

            // Set presets based on the card name.
            // You can expand this switch as you add more cards.
            switch (name)
            {
                case "Sword":
                    Actions = 1;
                    Category = CardCategory.Attack;
                    TargetType = CardTargetType.Single;
                    Damage = 6;
                    break;
                case "Frost":
                    Actions = 1;
                    Category = CardCategory.Attack;
                    TargetType = CardTargetType.Split;
                    Damage = 1;
                    AdditionalDamage = 2;
                    AdditionalEffectDuration = 3;
                    AdditionalEffectStrength = 1;
                    break;
                case "Deflect":
                    Actions = 1;
                    Category = CardCategory.Buff;
                    TargetType = CardTargetType.Single;
                    ShieldAmount = 4;
                    break;
                case "One Shot":
                    Actions = 3;
                    Category = CardCategory.Attack;
                    TargetType = CardTargetType.Single;
                    // Damage is determined by target's current HP (handled in effect)
                    break;
                case "Heal":
                    Actions = 1;
                    Category = CardCategory.Heal;
                    TargetType = CardTargetType.Single;
                    Healing = 10;
                    break;
                default:
                    // If no preset is defined for this card, you may either throw an exception
                    // or leave default values.
                    Console.WriteLine($"Warning: No preset defined for card '{name}'. Using default values.");
                    break;
            }
            CardAbilitys = GenerateDescription();
        }

        // Auto-generate a description based on the card's preset values.
        private string GenerateDescription()
        {
            if (Category == CardCategory.Attack)
            {
                if (TargetType == CardTargetType.Single)
                {
                    return (Name == "One Shot")
                        ? "One Shot any enemy (reduces HP to 0)."
                        : $"Deal {Damage} damage to one enemy.";
                }
                else if (TargetType == CardTargetType.All)
                {
                    return $"Deal {Damage} damage to all enemies.";
                }
                else if (TargetType == CardTargetType.Split)
                {
                    return $"Deal {Damage} damage to one enemy and {AdditionalDamage} additional damage to all enemies with an effect (Duration: {AdditionalEffectDuration}, Strength: {AdditionalEffectStrength}).";
                }
            }
            else if (Category == CardCategory.Heal)
            {
                if (TargetType == CardTargetType.Single)
                {
                    return $"Heal {Healing} HP for one ally.";
                }
                else if (TargetType == CardTargetType.All)
                {
                    return $"Heal {Healing} HP for all allies.";
                }
            }
            else if (Category == CardCategory.Buff)
            {
                if (TargetType == CardTargetType.Single)
                {
                    return $"Grant {ShieldAmount} shield to one ally.";
                }
                else if (TargetType == CardTargetType.All)
                {
                    return $"Grant {ShieldAmount} shield to all allies.";
                }
            }
            return "Unknown card effect.";
        }

        // The Play method now encapsulates the entire process of playing a card.
        // It deducts the action cost from the actor, prompts for target selection if needed,
        // and then applies the appropriate effects.
        public void Play(GameState gameState, PlayerCreature actor)
        {
            // Deduct the action cost from the actor's stamina.
            if (actor.Stamina < Actions)
            {
                Console.WriteLine("Not enough actions to play this card.");
                return;
            }
            actor.Stamina -= Actions;

            // Determine the target team based on the card's category.
            // For Attack cards, target the enemy team; for Heal/Buff, target the player's team.
            List<Creature> targetTeam = (Category == CardCategory.Attack) ? gameState.EnemyTeam : gameState.PlayerTeam;
            Creature? target = null;

            // Handle target selection based on TargetType.
            switch (TargetType)
            {
                case CardTargetType.Single:
                    target = ChooseTarget(targetTeam, (Category == CardCategory.Attack) ? "Choose an enemy:" : "Choose an ally:");
                    if (target == null)
                    {
                        Console.WriteLine("Invalid target selection. The card was not played.");
                        return;
                    }
                    ApplyEffectSingle(target);
                    break;

                case CardTargetType.All:
                    foreach (var creature in targetTeam)
                    {
                        ApplyEffectSingle(creature);
                    }
                    break;

                case CardTargetType.Split:
                    // For a split card, ask for a primary target, then apply additional effect to all.
                    target = ChooseTarget(targetTeam, "Choose an enemy for primary damage:");
                    if (target == null)
                    {
                        Console.WriteLine("Invalid target selection. The card was not played.");
                        return;
                    }
                    ApplyEffectSingle(target);
                    foreach (var creature in targetTeam)
                    {
                        ApplyAdditionalEffect(creature);
                    }
                    break;
            }
            Console.WriteLine($"{actor.Name} played {Name}.");
        }

        // Applies the card's primary effect to a single target.
        private void ApplyEffectSingle(Creature target)
        {
            if (Category == CardCategory.Attack)
            {
                if (Name == "One Shot")
                {
                    target.ApplyDamage(target.Health); // One shot: reduce HP to 0
                    Console.WriteLine($"{target.Name} has been one-shotted!");
                }
                else
                {
                    target.ApplyDamage(Damage);
                    Console.WriteLine($"Dealt {Damage} damage to {target.Name}.");
                }
            }
            else if (Category == CardCategory.Heal)
            {
                target.ApplyHealing(Healing);
                Console.WriteLine($"{target.Name} healed for {Healing} HP.");
            }
            else if (Category == CardCategory.Buff)
            {
                target.ApplyShield(ShieldAmount);
                Console.WriteLine($"{target.Name} gained {ShieldAmount} shield.");
            }
        }

        // Applies an additional effect, used by split cards.
        private void ApplyAdditionalEffect(Creature target)
        {
            target.ApplyDamage(AdditionalDamage);
            target.ApplyEffect(new Effect("Frost", AdditionalEffectDuration, AdditionalEffectStrength));
            Console.WriteLine($"Also dealt {AdditionalDamage} additional damage and applied Frost effect to {target.Name}.");
        }

        // Prompts the user to choose a target from a given team.
        private Creature? ChooseTarget(List<Creature> team, string prompt)
        {
            Console.Clear();
            Console.WriteLine(prompt);
            for (int i = 0; i < team.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {team[i].Name} (HP: {team[i].Health}/{team[i].MaxHealth})");
            }
            Console.Write("Enter target number: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int targetIndex) && targetIndex >= 1 && targetIndex <= team.Count)
            {
                return team[targetIndex - 1];
            }
            return null;
        }
    }
}
