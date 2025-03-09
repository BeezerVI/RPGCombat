using System;
using System.Collections.Generic;
using RPGCombatProject.GameLogic;

namespace RPGCombatProject.Models
{
    // Types of ability effects.
    public enum AbilityType
    {
        Attack,
        Heal,
        Buff,
        Custom
    }

    // Methods of selecting targets.
    public enum TargetingMethod
    {
        Single,
        All,
        Random
    }

    // Which team the ability applies to.
    public enum TargetTeam
    {
        Enemies,
        Allies,
        Both
    }

    public class Ability
    {
        public string Name { get; set; }
        public int Cost { get; set; } // Stamina cost
        public AbilityType Type { get; set; }
        public TargetingMethod Targeting { get; set; }
        public TargetTeam TeamTarget { get; set; }
        public bool CanTargetSelf { get; set; }
        public List<Effect> Effects { get; set; }

        // Optional chained action, allowing follow-up logic (for example, prompting a heal after damage)
        public Action<GameState, Creature>? ChainAction { get; set; }

        public Ability(string name, int cost, AbilityType type, TargetingMethod targeting, TargetTeam teamTarget, bool canTargetSelf, List<Effect>? effects = null, Action<GameState, Creature>? chainAction = null)
        {
            Name = name;
            Cost = cost;
            Type = type;
            Targeting = targeting;
            TeamTarget = teamTarget;
            CanTargetSelf = canTargetSelf;
            Effects = effects ?? new List<Effect>();
            ChainAction = chainAction;
        }

        // Executes the ability: deducts cost, selects targets, applies effects, and runs any chained actions.
        public void Execute(GameState gameState, Creature user)
        {
            // Deduct cost for player-controlled creatures.
            if (user is PlayerCreature player)
            {
                if (player.Stamina < Cost)
                {
                    Console.WriteLine("Not enough stamina to use this ability.");
                    return;
                }
                player.Stamina -= Cost;
            }
            // (For enemy AI, cost deduction can be integrated later.)

            // Determine all possible targets based on the ability’s team targeting rules.
            List<Creature> possibleTargets = DetermineTargets(gameState, user);
            if (possibleTargets.Count == 0)
            {
                Console.WriteLine("No valid targets available for ability.");
                return;
            }

            // For abilities that affect all targets, iterate through each.
            if (Targeting == TargetingMethod.All)
            {
                foreach (var target in possibleTargets)
                {
                    ApplyEffects(target);
                }
                Console.WriteLine($"{user.Name} used {Name} on all applicable targets.");
            }
            else
            {
                Creature target = SelectTarget(possibleTargets, user);
                if (target == null)
                {
                    Console.WriteLine("No target selected. Ability canceled.");
                    return;
                }
                ApplyEffects(target);
                Console.WriteLine($"{user.Name} used {Name} on {target.Name}.");
            }

            // Execute any chained action after the main effects.
            ChainAction?.Invoke(gameState, user);
        }

        // Determines valid targets based on the ability's TeamTarget setting.
        private List<Creature> DetermineTargets(GameState gameState, Creature user)
        {
            List<Creature> targets = new List<Creature>();

            if (TeamTarget == TargetTeam.Enemies)
            {
                targets.AddRange(user is PlayerCreature ? gameState.EnemyTeam : gameState.PlayerTeam);
            }
            else if (TeamTarget == TargetTeam.Allies)
            {
                targets.AddRange(user is PlayerCreature ? gameState.PlayerTeam : gameState.EnemyTeam);
            }
            else if (TeamTarget == TargetTeam.Both)
            {
                targets.AddRange(gameState.PlayerTeam);
                targets.AddRange(gameState.EnemyTeam);
            }

            if (!CanTargetSelf)
            {
                targets.Remove(user);
            }
            return targets;
        }

        // Handles target selection: random if specified or prompting the player for a single target.
        private Creature SelectTarget(List<Creature> targets, Creature user)
        {
            if (Targeting == TargetingMethod.Random)
            {
                Random rnd = new Random();
                return targets[rnd.Next(targets.Count)];
            }
            else if (Targeting == TargetingMethod.Single)
            {
                if (user is PlayerCreature)
                {
                    Console.WriteLine("Select a target:");
                    for (int i = 0; i < targets.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {targets[i].Name} (HP: {targets[i].Health}/{targets[i].MaxHealth})");
                    }
                    Console.Write("Enter target number: ");
                    string input = Console.ReadLine() ?? string.Empty;
                    if (int.TryParse(input, out int selection) && selection > 0 && selection <= targets.Count)
                    {
                        return targets[selection - 1];
                    }
                    Console.WriteLine("Invalid selection. Defaulting to first target.");
                    return targets[0];
                }
                else
                {
                    // For non-player creatures, default to the first valid target.
                    return targets[0];
                }
            }
            return targets[0];
        }

        // Applies each effect defined in the ability to the target.
        private void ApplyEffects(Creature target)
        {
            foreach (var effect in Effects)
            {
                // In this design, the Effect's EffectName (e.g. "damage", "heal", "shield") defines its behavior.
                switch (effect.EffectName.ToLower())
                {
                    case "damage":
                        target.ApplyDamage(effect.Strength);
                        Console.WriteLine($"{target.Name} takes {effect.Strength} damage.");
                        break;
                    case "heal":
                        target.ApplyHealing(effect.Strength);
                        Console.WriteLine($"{target.Name} heals for {effect.Strength} HP.");
                        break;
                    case "shield":
                        target.ApplyShield(effect.Strength);
                        Console.WriteLine($"{target.Name} gains {effect.Strength} shield.");
                        break;
                    case "piercing":
                        target.ApplyPiercingDamage(effect.Strength);
                        Console.WriteLine($"{target.Name} takes {effect.Strength} piercing damage.");
                        break;
                    case "bludgeoning":
                        target.ApplyBludgeoningDamage(effect.Strength);
                        Console.WriteLine($"{target.Name} takes {effect.Strength} bludgeoning damage.");
                        break;
                    default:
                        // For custom or status effects.
                        target.ApplyEffect(effect);
                        Console.WriteLine($"{target.Name} is affected by {effect.EffectName} (Duration: {effect.Duration}, Strength: {effect.Strength}).");
                        break;
                }
            }
        }
    }
}
