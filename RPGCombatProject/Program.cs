using System;
using System.Collections.Generic;
using RPGCombatProject.Models;
using RPGCombatProject.GameLogic;
using GameState = RPGCombatProject.GameLogic.GameState;

namespace RPGCombatProject
{
    public class Program //test
    {
        public static void Main(string[] args)
        {
            var enemies = new List<Creature>
            {
                new EnemyCreature("Slim"),
                new EnemyCreature("Giant Bug", health: 20, effects: new List<Effect>
                {
                    new Effect("Frost", 1, 2),
                    new Effect("Poisoned", 2)
                })
            };

            // Initialize each player with a unique hand.
            var player = new List<Creature>
            {
                new PlayerCreature("You", 30, 21, 10, 50, new List<Card>
                {
                    new Card("Sword", 1, "Deal 6 damage"),
                    new Card("Frost", 0, "Deal 1 damage to all enemies. Afflict 3 Frost."),
                    new Card("Deflect", 1, "Gain 4 Shield")
                }),
                new PlayerCreature("Liam", 15000, 5, 0, 50, new List<Card>
                {
                    new Card("Sword", 1, "Deal 6 damage"),
                    new Card("Deflect", 1, "Gain 4 Shield"),
                    new Card("One Shot", 3, "One Shot any creature")
                }),
            };


            // The enemies remain the same, and you no longer need a shared hand.
            GameState gameState = new GameState(enemies, player, 3);

            StartCombatLoop(gameState);
        }


        static void SetUpCombat()
        {
            // This is where the combat will be set up
            // This will include setting up the enemies, the player's team, and the player's hand
        }

        // This is the main combat loop
        static void StartCombatLoop(GameState gameState)
        {
            while (true)
            {
                // Process effects on all players at the start of the round
                ProcessTurnEffects(gameState.PlayerTeam);

                // Each living player takes a turn
                for (int i = 0; i < gameState.PlayerTeam.Count; i++)
                {
                    if (!gameState.PlayerTeam[i].IsDead)
                    {
                        PlayerTurnForPlayer(gameState, i);
                    }
                    else
                    {
                        Write($"{gameState.PlayerTeam[i].Name} is dead and cannot act.");
                    }
                }

                // After all players have finished, process enemy effects and then enemy turn
                ProcessTurnEffects(gameState.EnemyTeam);
                EnemysTurn(gameState);

                // Check if the combat is over
                if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                {
                    break;
                }
            }
            Write("Combat has fully ended.");
        }
        static void PlayerTurnForPlayer(GameState gameState, int playerIndex)
        {
            // Get the current player.
            PlayerCreature? currentPlayer = gameState.PlayerTeam[playerIndex] as PlayerCreature;
            if (currentPlayer == null)
            {
                Write("Error: The current player is not a PlayerCreature.");
                return;
            }

            int actionsRemaining = 3;
            Write($"{currentPlayer.Name}'s turn begins.");

            while (actionsRemaining > 0)
            {
                // Display game state with the current player's hand
                DisplayGameState(gameState, playerIndex);

                Console.WriteLine($"{currentPlayer.Name}, you have {actionsRemaining} action(s) remaining.");
                Console.Write("Enter the number of the card you want to play (or 'T' to change target, 'E' to end turn): ");
                string? input = Console.ReadLine();

                if (input == null)
                {
                    Write("Input cannot be null. Please enter a valid input.");
                    continue;
                }

                if (input.ToUpper() == "E")
                {
                    Write($"{currentPlayer.Name} has ended their turn.");
                    break;
                }
                else if (input.ToUpper() == "T")
                {
                    Console.Write("Enter the number of the enemy you want to target: ");
                    string? targetInput = Console.ReadLine();
                    if (targetInput != null && int.TryParse(targetInput, out int targetNumber) && targetNumber >= 1 && targetNumber <= gameState.EnemyTeam.Count)
                    {
                        // Set the current player's Target to the chosen enemy.
                        currentPlayer.Target = gameState.EnemyTeam[targetNumber - 1];
                        Write($"{currentPlayer.Name} targeted enemy: {currentPlayer.Target.Name}");
                    }
                    else
                    {
                        Write("Invalid target number. Please enter a valid number.");
                    }
                    continue;
                }

                if (!int.TryParse(input, out int cardNumber) || cardNumber < 1 || cardNumber > currentPlayer.Hand.Count)
                {
                    Write("Invalid input. Please enter a valid card number.");
                    continue;
                }

                // Get the selected card from the current player's hand.
                Card selectedCard = currentPlayer.Hand[cardNumber - 1];
                Write($"{currentPlayer.Name} selected card: {selectedCard.Name}");

                if (selectedCard.Actions > actionsRemaining)
                {
                    Write("Not enough actions to play this card. Please select another card.");
                    continue;
                }

                actionsRemaining -= selectedCard.Actions;
                // Pass the current player to PlayCard so that Ability can use currentPlayer.Target.
                PlayCard(selectedCard, gameState, currentPlayer);
                Write($"{currentPlayer.Name} played the card: {selectedCard.Name}");

                CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam, gameState);
                if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                {
                    break;
                }
            }
            Write($"{currentPlayer.Name}'s turn is over.");
        }



        static void EnemysTurn(GameState gameState)
        {
            Write("Enemy's turn.");
            foreach (var enemy in gameState.EnemyTeam)
            {
                if (enemy.IsDead) continue;

                // Ensure there's at least one alive player to target
                var viablePlayers = gameState.PlayerTeam.Where(p => !p.IsDead).ToList();
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


                // Check if the target died and update the list
                if (target.IsDead)
                {
                    Write($"{target.Name} has been defeated!");
                    viablePlayers.Remove(target);
                }
            }
            CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam, gameState);
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

        static void CleanBattleField(List<Creature> enemyTeam, List<Creature> playerTeam, GameState gameState)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(enemyTeam);
            CheckIfDeadForAllCreatures(playerTeam);
            // Remove dead creatures from the list
            DeleteDeadCreatures(enemyTeam);
            DeleteDeadCreatures(playerTeam);

                    
            // // Ensure enemyTargeted is set to a living creature
            // if (gameState.EnemyTeam.Count > 0 && (gameState.EnemyTargeted >= gameState.EnemyTeam.Count || gameState.EnemyTeam[gameState.EnemyTargeted].IsDead))
            // {
            //     gameState.EnemyTargeted = gameState.EnemyTeam.FindIndex(creature => !creature.IsDead);
            // }

            // // Ensure playerTargeted is set to a living creature
            // if (gameState.PlayerTeam.Count > 0 && (gameState.PlayerTargeted >= gameState.PlayerTeam.Count || gameState.PlayerTeam[gameState.PlayerTargeted].IsDead))
            // {
            //     gameState.PlayerTargeted = gameState.PlayerTeam.FindIndex(creature => !creature.IsDead);
            // }
        }

        static void PlayCard(Card card, GameState gameState, PlayerCreature actor)
        {
            // Check if the player has enough actions (this check may now be redundant if done in turn logic)
            if (card.Actions > gameState.ActionsRemaining)
            {
                Console.WriteLine("Not enough actions to play this card.");
                return;
            }

            gameState.ActionsRemaining -= card.Actions;
            // Pass the acting player to the card ability.
            card.Ability(gameState.EnemyTeam, gameState.PlayerTeam, actor);
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
            creatureTeam.RemoveAll(c => c.IsDead);
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

        /// <summary>
        /// Display the current game state, including the list of enemies, players, and available actions.
        /// </summary>
        static void DisplayGameState(GameState gameState, int currentPlayerIndex)
        {
            Console.Clear();

            // Display the enemies
            PrintCreatureList("Enemies", gameState.EnemyTeam, currentPlayerIndex, gameState);

            // Display the players
            PrintCreatureList("Your Team", gameState.PlayerTeam, currentPlayerIndex, gameState);

            // Get the current player (assumed to be a PlayerCreature)
            if (gameState.PlayerTeam[currentPlayerIndex] is PlayerCreature currentPlayer)
            {
                // Display the current player's hand
                CombatOptions(gameState.ActionsRemaining, currentPlayer.Hand);
            }
            else
            {
                Console.WriteLine("Error: Current player is not a PlayerCreature.");
            }
        }



        static bool IsCombatOver(List<Creature> enemyTeam, List<Creature> playerTeam)
        {    
            // Check if there are no enemies left
            if (enemyTeam.Count == 0)
            {
                Write("There are no enemies left. You have won!");
                return true;
            }

            // Check if there are no players left
            else if (playerTeam.Count == 0)
            {
                Write("There are no players left. Game over!");
                return true;
            }

            // Check if all enemies are dead
            else if (enemyTeam.All(e => e.IsDead))
            {
                Write("You have defeated all enemies!");
                return true;
            }

            // Check if all players are dead
            else if (playerTeam.All(p => p.IsDead))
            {
                Write("All players have been defeated. Game over!");
                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// Print a list of creatures with their health, shield, and effects.
        /// </summary
        static void PrintCreatureList(string title, List<Creature> creatures, int currentPlayerIndex, GameState gameState)
        {
            // Print the title centered within a 60-character wide line, filled with '=' characters
            Console.WriteLine(CreateCenteredText(title, 60, '='));

            // Initialize an index variable to keep track of the current index in the foreach loop
            int index = 0;

            // Iterate through each creature in the list
            foreach (var creature in creatures)
            {
                // Determine if the current creature is the target and if it is dead
                string marker = creature == gameState.PlayerTeam[currentPlayerIndex].Target ? ">> " : "   ";
                string status = creature.IsDead ? " [DEAD]" : "";
                Console.WriteLine($"{marker}{creature.Name}{status}");
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
        /// <summary>
        /// Create a string representation of a list of effects.
        /// </summary
        static string EffectList(List<Effect> effects)
        {
            if (effects.Count == 0) return "None";
            return string.Join(", ", effects.Select(e => $"{e.EffectName}{new string('|', e.Duration)}{new string('*', e.Strength)}"));
        }

        /// <summary>
        /// Display the available combat options based on the player's hand and actions remaining.
        /// </summary
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

        /// <summary>
        /// Create a centered text within a specified width and fill character.
        /// </summary
        static string CreateCenteredText(string text = "Example", int width = 50, char fillChar = '-')
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(fillChar, leftPadding) + text + new string(fillChar, rightPadding);
        }
    }
}
