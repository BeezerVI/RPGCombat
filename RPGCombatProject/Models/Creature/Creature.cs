using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGCombatProject.Models
{
    public abstract class Creature
    {
        public string Name { get; set; }
        public bool IsDead { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Shield { get; set; }
        public List<Effect> Effects { get; set; }
        public Creature? Target { get; set; }

        public Creature(string name, int maxHealth = 100, int health = 100, int shield = 0, List<Effect>? effects = null)
        {
            Name = name;
            IsDead = false;
            Health = health;
            MaxHealth = maxHealth;
            Shield = shield;
            Effects = effects ?? new List<Effect>();
            Target = null;
        }

        /// <summary>
        /// Check if the creature is dead and set the IsDead property accordingly.
        /// </summary>
        public void CheckIfDead()
        {
            if (Health <= 0)
            {
                IsDead = true;
                Console.WriteLine($"{Name} has died!");
            }
        }

        /// <summary>
        /// Applies standard damage, first reducing the shield, then applying remaining damage to health.
        /// </summary>
        public void ApplyDamage(int damage)
        {
            if (Shield > 0)
            {
                int remainingDamage = Math.Max(0, damage - Shield);
                Shield = Math.Max(0, Shield - damage);
                damage = remainingDamage;
            }
            Health = Math.Max(0, Health - damage);
            Console.WriteLine($"{Name} takes {damage} damage!");
            CheckIfDead();
        }

        /// <summary>
        /// Applies piercing damage directly to health, bypassing shields.
        /// </summary>
        public void ApplyPiercingDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
            Console.WriteLine($"{Name} takes {damage} piercing damage! (Bypassed shield)");
            CheckIfDead();
        }

        public void ApplyHealing(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
            Console.WriteLine($"{Name} heals for {amount} HP.");
        }

        public void ApplyEffect(Effect effect)
        {
            Effects.Add(effect);
            Console.WriteLine($"{Name} is affected by {effect.EffectName} for {effect.Duration} turns.");
        }

        public void ApplyShield(int amount)
        {
            Shield += amount;
            Console.WriteLine($"{Name} gains {amount} shield.");
        }

        /// <summary>
        /// Applies bludgeoning damage: 
        /// - Deals FULL damage to SHIELDS first.
        /// - If no shield remains, it deals normal health damage.
        /// </summary>
        public void ApplyBludgeoningDamage(int damage)
        {
            if (Shield > 0)
            {
                // Damage shield first
                Shield = Math.Max(0, Shield - damage);
                Console.WriteLine($"{Name} takes {damage} bludgeoning damage to their shield!");
            }
            else
            {
                // If no shield remains, apply damage normally to health
                Health = Math.Max(0, Health - damage);
                Console.WriteLine($"{Name} takes {damage} bludgeoning damage to their health!");
            }
        }


        public bool IsStunned()
        {
            return Effects.Any(effect => effect.EffectName.ToLower() == "stun" && effect.Duration > 0);
        }


        /// <summary>
        /// Processes all active effects (poison, burn, regeneration, stun, etc.).
        /// </summary>
        public void ProcessEffects()
        {
            foreach (var effect in Effects.ToList())
            {
                switch (effect.EffectName.ToLower())
                {
                    case "stun":
                        Console.WriteLine($"{Name} is stunned and cannot act.");
                        break;

                    case "burning":
                        ApplyDamage(effect.Strength);
                        Console.WriteLine($"{Name} suffers {effect.Strength} burn damage.");
                        break;

                    case "frozen":
                        Console.WriteLine($"{Name} is frozen and skips this turn.");
                        break;

                    case "bleeding":
                        ApplyDamage(effect.Strength);
                        Console.WriteLine($"{Name} bleeds for {effect.Strength} damage.");
                        break;

                    case "poison":
                        ApplyDamage(effect.Strength);
                        Console.WriteLine($"{Name} takes {effect.Strength} poison damage.");
                        break;

                    case "weaken":
                        Console.WriteLine($"{Name}'s attack power is reduced.");
                        break;

                    case "regeneration":
                        ApplyHealing(effect.Strength);
                        Console.WriteLine($"{Name} regenerates {effect.Strength} health.");
                        break;
                }

                // Reduce effect duration
                effect.Duration--;

                // Remove expired effects
                if (effect.Duration <= 0)
                {
                    Console.WriteLine($"{effect.EffectName} on {Name} has ended.");
                    Effects.Remove(effect);
                }
            }
        }

    }
}
