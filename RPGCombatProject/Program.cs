using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGCombatProject
{
    public class Effect
    {
        public string EffectName { get; set; }
        public int Duration { get; set; }
        public int Strength { get; set; }

        // Constructor to initialize an Effect object
        public Effect(string effectName, int duration = 1, int strength = 1)
        {
            EffectName = effectName;
            Duration = duration;
            Strength = strength;
        }
    }

    public class Creature
    {
        public string Name { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsDead { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Shield { get; set; }
        public List<Effect> Effects { get; set; }

        // Constructor to initialize a Creature object
        public Creature(string name, bool isPlayer = false, bool isDead = false, int maxHealth = 100, int health = 100, int shield = 0, List<Effect>? effects = null)
        {
            Name = name;
            IsPlayer = isPlayer;
            IsDead = isDead;
            Health = health;
            MaxHealth = maxHealth;
            Shield = shield;
            Effects = effects ?? new List<Effect>(); // Handle null list gracefully
        }

        // Method to check if the creature is dead based on its health
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
                // Subtract damage from the shield first
                if (damage <= Shield)
                {
                    Shield -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= Shield;
                    Shield = 0;
                }
            }
            // Apply remaining damage to health
            Health -= damage;

            // Check if the creature is dead
            CheckIfDead();
        }

        public void ApplyHealing(int amount)
        {
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth; // Prevent overhealing
        }

        public void ApplyEffect(Effect effect)
        {
            Effects.Add(effect);
        }

        // Updated ability logic
        public void ApplyShield(int amount)
        {
            Shield += amount;
        }

        // Method to process effects on the creature each turn (like damage-over-time)
        public void ProcessEffects()
        {
            foreach (var effect in Effects.ToList()) // Use ToList to avoid modifying the list during iteration
            {
                switch (effect.EffectName)
                {
                    case "Frost":
                        ApplyDamage(effect.Strength); // Frost deals damage over time
                        break;
                    case "Poisoned":
                        ApplyDamage(effect.Strength); // Poison effect example
                        break;
                }

                effect.Duration--;
                if (effect.Duration <= 0)
                {
                    Effects.Remove(effect);
                }
            }
        }
    }

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
                    targetPlayer.ApplyShield(5);
                    Console.WriteLine($"Gained 5 Shield for {targetPlayer.Name}.");
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
                    targetEnemy.ApplyDamage(targetEnemy.Health); // Effectively reduces health to 0
                    Console.WriteLine($"{targetEnemy.Name} has been one-shotted!");
                    break;
                default:
                    Console.WriteLine("Card ability not implemented.");
                    break;
            }
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            // Example data for testing
            var enemies = new List<Creature>
            {
                new Creature("Slim"),
                new Creature("Giant Bug", health: 20, effects: new List<Effect>
                {
                    new Effect("Frost", 1, 2),
                    new Effect("Poisoned", 2)
                })
            };

            // Making it a list to allow for multiple players in the future
            var player = new List<Creature>
            {
                new Creature("You", true, false, 100, 100, 10),
                new Creature("Liam", true, false, 15000, 15000, 50),
            };

            // This is an example hand of cards that the player can use
            var hand = new List<Card>
            {
                new Card("Sword", 1, "Deal 6 damage"),
                new Card("Frost", 0, "Deal 1 damage to all enemies. Afflict 3 Frost."),
                new Card("Deflect", 1, "Gain 5 Shield"),
                new Card("One Shot", 0, "One shots any creature")
            };

            StartCombatLoop(enemies, player, hand);
            // Display game state
        }

        static void SetUpCombat()
        {
            // This is where the combat will be set up
            // This will include setting up the enemies, the player's team, and the player's hand
        }
        // This is the main combat loop
        static void StartCombatLoop(List<Creature>enemieTeam, List<Creature>playersTeam, List<Card>playersHand){
            // This is where the combat will be handled
            // These vairbels will be moved to SetUpCombat when the combat is fully set up
            int enemieTargeted = 0;
            int playerTargeted = 0;


            while (true)
            {
                ProcessTurnEffects(playersTeam);
                PlayersTurn(enemieTeam, playersTeam, playersHand,  ref enemieTargeted, ref playerTargeted);
                
                ProcessTurnEffects(enemieTeam);
                EnemysTurn(enemieTeam, playersTeam, ref enemieTargeted, ref playerTargeted);

                    // Check if the combat is over
                if (IsCombatOver(enemieTeam, playersTeam) == true)
                {
                    // End the combat
                    break;
                }
            }
            Write("Combat has fully ended.");
        }

        static void PlayersTurn(List<Creature>enemieTeam, List<Creature>playersTeam, List<Card>playersHand, ref int enemieTargeted, ref int playerTargeted){
            // This is where the player's turn will be handled
            int actionsRemaining = 3;

            while (true)
            {
                // Display the current game state
                DisplayGameState(enemieTeam, playersTeam, enemieTargeted, playerTargeted, actionsRemaining, playersHand);

                // Get the player's input
                Console.Write("Enter the number of the card you want to play (or 'T' to change target, 'E' to end turn): ");
                string? input = Console.ReadLine();
                if (input == null)
                {
                    Write("Input cannot be null. Please enter a valid input.");
                    continue;
                }

                // Handle special commands
                if (input.ToUpper() == "E")
                {
                    Write("Ending turn.");
                    break; // End the turn
                }
                else if (input.ToUpper() == "T")
                {
                    Console.Write("Enter the number of the enemy you want to target: ");
                    string? targetInput = Console.ReadLine();
                    if (targetInput == null || !int.TryParse(targetInput, out int targetNumber) || targetNumber < 1 || targetNumber > enemieTeam.Count)
                    {
                        Write("Invalid target number. Please enter a valid number.");
                        continue;
                    }
                    enemieTargeted = targetNumber - 1;
                    Write($"Targeted enemy: {enemieTeam[enemieTargeted].Name}");
                    continue;
                }

                // Check if the input is a valid number
                if (!int.TryParse(input, out int cardNumber))
                {
                    Write("Invalid input. Please enter a number.");
                    continue;
                }

                // Check if the input is within the range of the player's hand
                if (cardNumber < 1 || cardNumber > playersHand.Count)
                {
                    Write("Invalid input. Please enter a number within the range of your hand.");
                    continue;
                }

                // Get the selected card from the player's hand
                Card selectedCard = playersHand[cardNumber - 1];

                // Debug statement to check the selected card
                Write($"Selected card: {selectedCard.Name}");

                // Check if the player has enough actions to play the card
                if (selectedCard.Actions > actionsRemaining)
                {
                    Write("Not enough actions to play this card. Please select another card.");
                    continue;
                }

                // Play the selected card
                PlayCard(selectedCard, enemieTeam, playersTeam, ref actionsRemaining, ref enemieTargeted, ref playerTargeted);

                // Debug statement to confirm the card was played
                Write($"You played the card: {selectedCard.Name}");

                CleanBattleField(enemieTeam, playersTeam);
                if (IsCombatOver(enemieTeam, playersTeam) == true)
                {
                    // End the combat
                    break;
                }
            }
            Write("Player's turn ended.");
        }

        static void EnemysTurn(List<Creature> enemyTeam, List<Creature> playerTeam, ref int enemyTargeted, ref int playerTargeted)
        {
            Write("Enemy's turn.");
            foreach (var enemy in enemyTeam)
            {
                if (enemy.IsDead) continue;

                // Ensure there's at least one alive player to target
                var viablePlayers = playerTeam.Where(p => !p.IsDead).ToList();
                if (!viablePlayers.Any())
                {
                    Write("All players are defeated! Enemies win!");
                    return;
                }

                // Enemy AI: Target the player with the lowest combined health and shield
                var target = viablePlayers.OrderBy(p => p.Health + p.Shield).First();

                // Enemy attack power (can be dynamic)
                int attackPower = 5;

                // Apply damage
                DamageCreature(target, attackPower);

                Write($"{enemy.Name} dealt {attackPower} damage to {target.Name}.");

                // Update target index if needed
                playerTargeted = playerTeam.IndexOf(target);

                // Check if the target died and update the list
                if (target.IsDead)
                {
                    Write($"{target.Name} has been defeated!");
                    viablePlayers.Remove(target);
                }
            }
            CleanBattleField(enemyTeam, playerTeam);
        }


        /// <summary>
        /// Handles applying damage to a creature, considering their shield and health.
        /// </summary>
        static void DamageCreature(Creature target, int damage)
        {
            // Apply damage to shield first
            if (target.Shield > 0)
            {
                int remainingDamage = Math.Max(0, damage - target.Shield);
                target.Shield = Math.Max(0, target.Shield - damage);
                damage = remainingDamage;
            }

            // Apply any remaining damage to health
            target.Health = Math.Max(0, target.Health - damage);

            // Check if the target is dead
            target.CheckIfDead();
        }


        static void ProcessTurnEffects(List<Creature> creatures)
        {
            foreach (var creature in creatures)
            {
                if (!creature.IsDead)
                {
                    creature.ProcessEffects();
                }
            }
        }

        static void CleanBattleField(List<Creature> enemieTeam, List<Creature> playersTeam)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(enemieTeam);
            CheckIfDeadForAllCreatures(playersTeam);
            // Remove dead creatuers from the list
            DeleteDeadCreatures(enemieTeam);
            DeleteDeadCreatures(playersTeam);
        }


        static void PlayCard(Card card, List<Creature> enemyTeam, List<Creature> playerTeam, ref int actionsRemaining, ref int enemyTargeted, ref int playerTargeted)
        {
            // Check if the player has enough actions to play the card
            if (card.Actions > actionsRemaining)
            {
                Console.WriteLine("Not enough actions to play this card.");
                return;
            }

            // Deduct actions and call the card's ability
            actionsRemaining -= card.Actions;
            card.Ability(enemyTeam, playerTeam, ref enemyTargeted, ref playerTargeted);
        }


        static void CheckIfDeadForAllCreatures(List<Creature> creatureTeam)
        {
            foreach (var creature in creatureTeam)
            {
                creature.CheckIfDead();
            }
        }
        static void DeleteDeadCreatures(List<Creature> creatureTeam)
        {
            {
                creatureTeam.RemoveAll(c => c.IsDead);
            }
        }

        static void Write(string text, bool waitForInput = true, bool clearConsole = true)
        // Write a line of text to the console and wait for the user to press Enter if waitForInput is true
        // clearConsole will clear the console after the text is displayed if true
        // text is the string to be displayed
        {
            if (clearConsole)
            {
                Console.Clear();
            }
            if (waitForInput)
            {
                Console.WriteLine(text + "\nPress Enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        static void DisplayGameState(List<Creature> enemieTeam, List<Creature> playersTeam, int enemieTargeted, int playerTargeted, int actionsRemaining, List<Card> playersHand)
        // Display the current game state
        {
            Console.Clear();
            PrintCreatureList("Enemies", enemieTeam, enemieTargeted);
            PrintCreatureList("Your Team", playersTeam, playerTargeted);
            CombatOptions(actionsRemaining, playersHand);
        }


        static bool IsCombatOver(List<Creature> enemieTeam, List<Creature> playersTeam)
        {
            // Check if there are no enemies left
            if (enemieTeam.Count == 0)
            {
                Write("There are no enemies left. You have won!");
                return true;
            }

            // Check if there are no players left
            else if (playersTeam.Count == 0)
            {
                Write("There are no players left. Game over!");
                return true;
            }

            // Check if all enemies are dead
            else if (enemieTeam.All(e => e.IsDead))
            {
                Write("You have defeated all enemies!");
                return true;
            }

            // Check if all players are dead
            else if (playersTeam.All(p => p.IsDead))
            {
                Write("All players have been defeated. Game over!");
                return true;
            }

            else{
                return false;
            }
        }

        static void PrintCreatureList(string title, List<Creature> creatures, int targetedCreature = 0)
        {
        // Print the title centered within a 60-character wide line, filled with '=' characters
        Console.WriteLine(CreateCenteredText(title, 60, '='));

        // Initialize an index variable to keep track of the current index in the foreach loop
        int index = 0;

        // Iterate through each creature in the list
            foreach (var creature in creatures)
            {
                // Determine if the current creature is the target by comparing indices
                string marker = index == targetedCreature ? ">> " : "   ";

                // Print the creature's name with a marker if it is the target
                Console.WriteLine($"{marker}{creature.Name}");

                // Print the creature's health, max health, and shield (if any)
                Console.WriteLine($"   - HP: {creature.Health} / {creature.MaxHealth}" +
                                $"{(creature.Shield > 0 ? $" | Shield: {creature.Shield}" : "")}");

                // Print the list of effects on the creature
                Console.WriteLine($"   - Effects: {EffectList(creature.Effects)}\n");

                // Increment the index variable
                index++;
            }
        }

        static string EffectList(List<Effect> effects)
        {
            if (effects.Count == 0) return "None";
            return string.Join(", ", effects.Select(e => $"{e.EffectName}{new string('|', e.Duration)}{new string('*', e.Strength)}"));
        }

        static void CombatOptions(int actionsRemaining, List<Card> playersHand)
        {
            Console.WriteLine(CreateCenteredText("Combat Options", 60, '-'));
            Console.WriteLine($"[{actionsRemaining} Actions Remaining]\n");
            for (int i = 0; i < playersHand.Count; i++)
            {
                var card = playersHand[i];
                Console.WriteLine($"{i + 1}. {card.Name}    [Cost: {card.Actions} Action(s)]");
                Console.WriteLine($"   - {card.CardAbilitys}\n");
            }
        }

        static string CreateCenteredText(string text = "Example", int width = 50, char fillChar = '-')
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(fillChar, leftPadding) + text + new string(fillChar, rightPadding);
        }
    }
}
