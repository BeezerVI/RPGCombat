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

        // NEW: Store a reference to the creature that this creature is targeting.
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

        public abstract void Attack(Creature target);


        /// <summary>
        /// Check if the creature is dead and set the IsDead property accordingly.
        /// </summary>
        public void CheckIfDead()
        {
            if (Health <= 0)
            {
                IsDead = true;
            }
        }

        public void ApplyDamage(int damage)
        {
            if (Shield > 0)
            {
                int remainingDamage = Math.Max(0, damage - Shield);
                Shield = Math.Max(0, Shield - damage);
                damage = remainingDamage;
            }
            Health = Math.Max(0, Health - damage);
            CheckIfDead();
        }

        public void ApplyHealing(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public void ApplyEffect(Effect effect)
        {
            Effects.Add(effect);
        }

        public void ApplyShield(int amount)
        {
            Shield += amount;
        }

        public void ProcessEffects()
        {
            foreach (var effect in Effects.ToList())
            {
                if (effect.EffectName == "Frost")
                    ApplyDamage(effect.Strength);
                if (effect.EffectName == "Poisoned")
                    ApplyDamage(effect.Strength);

                effect.Duration--;
                if (effect.Duration <= 0)
                    Effects.Remove(effect);
            }
        }
    }
}
