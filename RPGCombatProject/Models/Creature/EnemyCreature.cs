using System;
using System.Collections.Generic;
using System.Linq;
using RPGCombatProject.GameLogic;
using RPGCombatProject.Utilityes;
using System.Collections.Generic;


namespace RPGCombatProject.Models
{
    public class EnemyCreature : Creature
    {   
        // (Optional) A property to later hold personality information.
        // public string Personality { get; set; } 

        // Constructor to fully define an enemy with abilities.
        public EnemyCreature(string name, int maxHealth, int health, int shield, int stamina, int maxStamina, List<Effect>? effects = null)
            : base(name, maxHealth, health, shield, effects, stamina, maxStamina, hand: null)
        {

        }

        // New constructor: when only a name is supplied, default stats and abilities are assigned.
        public EnemyCreature(string name)
            : base(name, 0, 0, 0, new List<Effect>())
        {
            // Set default stats based on enemy type.
            switch (name.ToLower())
            {
                case "slime":
                    this.MaxHealth = 120;
                    this.Health = 120;
                    this.Shield = 15;
                    this.Stamina = 3;
                    this.MaxStamina = 3;
                    this.Hand = new List<Ability>
                    {
                        Abilities.GetAbility("Sword Strike"),
                        Abilities.GetAbility("Heavy Slash"),
                        Abilities.GetAbility("Fortify"),
                    };
                    break;
                case "giant bug":
                    this.MaxHealth = 120;
                    this.Health = 120;
                    this.Shield = 15;
                    this.Stamina = 3;
                    this.MaxStamina = 3;
                    this.Hand = new List<Ability>
                    {
                        Abilities.GetAbility("Sword Strike"),
                        Abilities.GetAbility("Heavy Slash"),
                        Abilities.GetAbility("Fortify"),
                    };
                    break;
                default:
                    this.MaxHealth = 120;
                    this.Health = 120;
                    this.Shield = 15;
                    this.Stamina = 3;
                    this.MaxStamina = 3;
                    this.Hand = new List<Ability>
                    {
                        Abilities.GetAbility("Sword Strike"),
                        Abilities.GetAbility("Heavy Slash"),
                        Abilities.GetAbility("Fortify"),
                    };
                    break;
            }
        }

        /// <summary>
        /// Chooses an ability based on available stamina and basic logic.
        /// </summary>
        private Ability? ChooseAbility(List<Creature> viablePlayers, GameState gameState)
        {
            // Filter for abilities that can be used (cost less than or equal to current stamina)
            var availableAbilities = this.Hand.Where(a => a.Cost <= this.Stamina).ToList();
            if (availableAbilities.Count == 0)
            {
                return null;
            }

            // If health is low (below 30%), try to use a healing ability if available.
            if ((double)Health / MaxHealth < 0.3)
            {
                var healAbility = availableAbilities.FirstOrDefault(a => a.Type == AbilityType.Heal);
                if (healAbility != null)
                {
                    return healAbility;
                }
            }

            // Future: Here you could add personality-based choices (e.g., defensive enemies may prefer Buffs).

            // Default: try to use an attack ability.
            var attackAbility = availableAbilities.FirstOrDefault(a => a.Type == AbilityType.Attack);
            if (attackAbility != null)
            {
                return attackAbility;
            }

            // Fallback: return the first available ability.
            return availableAbilities.First();
        }

        /// <summary>
        /// Overrides PlayTurn to implement the new AI logic.
        /// </summary>
        public override void PlayTurn()
        {
            UIManager.Write($"{Name} begins its turn.", waitForInput: false);
            ProcessEffects();
            if (IsStunned())
            {
                UIManager.Write($"{Name} is stunned and skips its turn.");
                return;
            }

            // Get viable targets from the public game state.
            var gameState = Program.gameState;
            var viablePlayers = gameState.PlayerTeam.Where(p => !p.IsDead).ToList();
            if (viablePlayers.Count == 0)
            {
                return;
            }

            // Choose an ability based on AI logic.
            Ability? chosenAbility = ChooseAbility(viablePlayers, gameState);
            if (chosenAbility == null)
            {
                UIManager.Write($"{Name} has no abilities available to use.");
                return;
            }

            // (Optional) For attack abilities, you might want to ensure the target is the player with the lowest HP.
            // The Ability.Execute method for non-player creatures defaults to the first valid target.
            // If needed, you could extend Ability.Execute to accept an explicit target.

            // Execute the chosen ability.
            chosenAbility.Execute(gameState, this);

            // Deduct stamina cost for enemy as well.
            // (This logic is similar to what is done for players.)
            if (this.Stamina >= chosenAbility.Cost)
            {
                this.Stamina -= chosenAbility.Cost;
            }

            UIManager.Write($"{Name}'s turn has ended.", clearConsole: 2);
        }
    }
}
