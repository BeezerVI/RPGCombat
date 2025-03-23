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
        public EnemyCreature(string name, int maxHealth, int health, int shield, int stamina, int maxStamina, List<Effect>? effects = null, List<Ability>? hand = null) : base(name, maxHealth, health, shield, stamina, maxStamina, effects, hand)
        {

        }

        // New constructor: when only a name is supplied, default stats and abilities are assigned.
        public EnemyCreature(string name) : base(name, 0, 0, 0, 0, 0, new List<Effect>(), new List<Ability>())
        {
            try
            {
                var loadedEnemy = EnemyLoader.GetEnemy(name);
                this.MaxHealth = loadedEnemy.MaxHealth;
                this.Health = loadedEnemy.MaxHealth;
                this.Shield = loadedEnemy.Shield;
                this.Stamina = loadedEnemy.Stamina;
                this.MaxStamina = loadedEnemy.MaxStamina;
                this.Hand = loadedEnemy.Hand ?? new List<Ability>(); // Ensure it's always initialized
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Using default stats for enemy: {name}");

                // Fallback default stats
                this.MaxHealth = 50;
                this.Health = 50;
                this.Shield = 0;
                this.Stamina = 3;
                this.MaxStamina = 3;
                this.Hand = new List<Ability> { Abilities.GetAbility("Sword Strike") };
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
            var viableTargets = gameState.Teams
                .Where(team => team.Members.Any(member => !member.IsDead))
                .SelectMany(team => team.Members)
                .Where(member => !member.IsDead && !(member is EnemyCreature))
                .ToList();

            if (viableTargets.Count == 0)
            {
                return;
            }

            Stamina = MaxStamina; // Reset stamina at the start of the turn

            bool isTurn = true;
            while (isTurn)
            {
                // Display game state
                UIManager.DisplayGameState();
                UIManager.CombatOptions(this);
                UIManager.PlayerCombatOptions(this);

                // Choose an ability based on AI logic.
                Ability? chosenAbility = ChooseAbility(viableTargets, gameState);
                if (chosenAbility == null)
                {
                    UIManager.Write($"{Name} has no abilities available to use.", clearConsole: 2);
                    return;
                }

                // Execute the chosen ability.
                chosenAbility.Execute(gameState, this);

                if (this.Stamina <= 0)
                {
                    isTurn = false;
                }
            }

            UIManager.Write($"{Name}'s turn has ended.", clearConsole: 2);
        }
    }
}
