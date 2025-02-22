using System;
using System.Collections.Generic;
using RPGCombatProject.Models;
using RPGCombatProject.GameLogic;
using GameState = RPGCombatProject.GameLogic.GameState;

namespace RPGCombatProject
{
    public class Program
    {
        private static GameState gameState = null!;
        private static int playerIndex;

        public static void Main(string[] args)
        {

            SetUpCombat();

            StartCombatLoop();
        }
        static void SetUpCombat()
        {
            // Set up enemies as before.
            var enemies = new List<Creature>
            {
                new EnemyCreature("Slim"),
                new EnemyCreature("Giant Bug")
            };

            // Ask how many players are participating.
            Console.Clear();
            Console.Write("Enter the number of players: ");
            int numPlayers = 0;
            while (!int.TryParse(Console.ReadLine(), out numPlayers) || numPlayers <= 0)
            {
                Console.Write("Invalid input. Please enter a positive integer: ");
            }

            var players = new List<Creature>();

            // For each player, ask for their name and class.
            for (int i = 0; i < numPlayers; i++)
            {
                Console.Clear();
                Console.WriteLine($"\n--- Setting up Player {i + 1} ---");

                // Ask for player's name.
                Console.Write("Enter your name: ");
                string name = Console.ReadLine() ?? $"Player{i + 1}";

                // Ask for player's class.
                Console.WriteLine("Choose a class:");
                Console.WriteLine("1. Warrior");
                Console.WriteLine("2. Mage");
                Console.WriteLine("3. Rogue");
                Console.Write("Enter your choice (1-3): ");
                string classChoice = Console.ReadLine() ?? "1";

                string chosenClass;
                switch (classChoice)
                {
                    case "1":
                        chosenClass = "Warrior";
                        break;
                    case "2":
                        chosenClass = "Mage";
                        break;
                    case "3":
                        chosenClass = "Rogue";
                        break;
                    default:
                        Console.WriteLine("Invalid choice; defaulting to Warrior.");
                        chosenClass = "Warrior";
                        break;
                }

                // Create a new player creature using the factory method in PlayerCreature.
                PlayerCreature newPlayer = PlayerCreature.CreatePlayer(name, chosenClass);
                players.Add(newPlayer);
            }

            gameState = new GameState(enemies, players)!;
            playerIndex = 0; // Assign a default value to playerIndex

            if (gameState == null)
            {
                throw new InvalidOperationException("Game state must be initialized.");
            }
        }

        // This is the main combat loop
        static void StartCombatLoop()
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
                        playerIndex = i;
                        PlayerTurnForPlayer(i);
                    }
                    else
                    {
                        Write($"{gameState.PlayerTeam[i].Name} is dead and cannot act.");
                    }
                }

                // After all players have finished, process enemy effects and then enemy turn
                ProcessTurnEffects(gameState.EnemyTeam);
                EnemysTurn();

                // Check if the combat is over
                if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                {
                    break;
                }
            }
            Write("Combat has fully ended.");
        }

        static void PlayerTurnForPlayer(int playerIndex)
        {
            // Get the current player.
            PlayerCreature? currentPlayer = gameState.PlayerTeam[playerIndex] as PlayerCreature;
            if (currentPlayer == null)
            {
                Write("Error: The current player is not a PlayerCreature.");
                return;
            }

            currentPlayer.Stamina = 3;
            Write($"{currentPlayer.Name}'s turn begins.");

            bool isPlayerTurn = true;
            while (isPlayerTurn == true)
            {
                // Display game state
                DisplayGameState();

                isPlayerTurn = PlayerCombatOptions(currentPlayer);

                CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam);
                if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                {
                    break;
                }
            }
            Write($"{currentPlayer.Name}'s turn is over.");
        }

        static void EnemysTurn()
        {
            Write("Enemy's turn.");
            foreach (var enemy in gameState.EnemyTeam)
            {
                if (enemy.IsDead) continue;

                // Get a list of alive players.
                var viablePlayers = gameState.PlayerTeam.Where(p => !p.IsDead).ToList();
                if (!viablePlayers.Any())
                {
                    Write("All players are defeated! Enemies win!");
                    return;
                }

                // If the enemy is an EnemyCreature (it should be), use its Act method.
                if (enemy is EnemyCreature e)
                {
                    Console.Clear();                    
                    DisplayGameState();
                    Console.WriteLine($"{enemy.Name}'s turn.");
                    e.Act(viablePlayers);
                    Console.ReadLine();
                }
                else
                {
                    // Fallback behavior if enemy is not of type EnemyCreature.
                    Write("Error: Enemy is not an EnemyCreature.");
                }
            }

            CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam);
        }


        static bool PlayerCombatOptions(PlayerCreature currentPlayer)
        {
            Console.Write("Enter the number of the card you want to play (or 'E' to end turn): ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Write("Input cannot be null. Please enter a valid input.");
                return true;
            }

            if (input.ToUpper() == "E")
            {
                Write($"{currentPlayer.Name} has ended their turn.");
                return false;
            }

            if (!int.TryParse(input, out int cardNumber) || cardNumber < 1 || cardNumber > currentPlayer.Hand.Count)
            {
                Write("Invalid input. Please enter a valid card number.");
                return true;
            }

            // Get the selected card from the current player's hand.
            Card selectedCard = currentPlayer.Hand[cardNumber - 1];
            Write($"{currentPlayer.Name} selected card: {selectedCard.Name}");

            if (selectedCard.Actions > currentPlayer.Stamina)
            {
                Write("Not enough actions to play this card. Please select another card.");
                return true;
            }

            // Ask for a target if needed and execute ability
            selectedCard.Play(gameState, currentPlayer);

            return true;
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

        static void CleanBattleField(List<Creature> enemyTeam, List<Creature> playerTeam)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(enemyTeam);
            CheckIfDeadForAllCreatures(playerTeam);
            // Remove dead creatures from the list
            DeleteDeadCreatures(enemyTeam);
        //    DeleteDeadCreatures(playerTeam);
        }

        /// <summary>
        /// Check if any creatures are dead and set the IsDead property accordingly.
        /// </summary>
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

        /// <summary>
        /// Write a line of text to the console and wait for the user to press Enter if waitForInput is true.
        /// </summary>        
        static void Write(string text, bool waitForInput = true, bool clearConsole = true)
        // Write a line of text to the console and wait for the user to press Enter if waitForInput is true
        // clearConsole will clear the console after the text is displayed if true
        // text is the string to be displayed
        {
            if (clearConsole)
            {
                Console.Clear();
                if (gameState != null)
                {
                    DisplayGameState();
                }
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
        static void DisplayGameState()
        {
            Console.Clear();

            // Display the enemies
            PrintCreatureList("Enemies", gameState.EnemyTeam);

            // Display the players
            PrintCreatureList("Your Team", gameState.PlayerTeam);

            // Get the current player (assumed to be a PlayerCreature)
            if (playerIndex >= 0 && playerIndex < gameState.PlayerTeam.Count && gameState.PlayerTeam[playerIndex] is PlayerCreature currentPlayer)
            {
                // Display the current player's hand
                CombatOptions(currentPlayer);
            }
            else
            {
                Console.WriteLine("Error: Current player is not a PlayerCreature or index out of bounds.");
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
        /// </summary>
        static void PrintCreatureList(string title, List<Creature> creatures)
        {
            // Print the title centered within a 60-character wide line, filled with '=' characters
            Console.WriteLine(CreateCenteredText(title, 60, '='));

            // Iterate through each creature in the list
            foreach (var creature in creatures)
            {
                // Determine if the current creature is the target and if it is dead
                // string marker = (creature == gameState.PlayerTeam[playerIndex] || creature == gameState.EnemyTeam[playerIndex]) ? ">> " : "   ";
                //string marker =  $"{creatures.IndexOf(creature) + 1}. ";
                string marker =  "   ";
                string status = creature.IsDead ? " [DEAD]" : "";
                
                // Print the creature's name with a marker if it is the target
                Console.WriteLine($"{marker}{creature.Name}{status}");

                // Print the creature's health, max health, and shield (if any)
                Console.WriteLine($"   - HP: {creature.Health} / {creature.MaxHealth}" +
                                $"{(creature.Shield > 0 ? $" | Shield: {creature.Shield}" : "")}");

                // Print the list of effects on the creature
                Console.WriteLine($"   - Effects: {EffectList(creature.Effects)}\n");
            }
        }

        /// <summary>
        /// Create a string representation of a list of effects.
        /// </summary>
        static string EffectList(List<Effect> effects)
        {
            if (effects.Count == 0) return "None";
            return string.Join(", ", effects.Select(e => $"{e.EffectName}{new string('|', e.Duration)}{new string('*', e.Strength)}"));
        }

        /// <summary>
        /// Display the available combat options based on the player's hand and actions remaining.
        /// </summary>
        static void CombatOptions(PlayerCreature currentPlayer)
        {
            var playersHand = currentPlayer.Hand;
            Console.WriteLine(CreateCenteredText("Combat Options", 60, '-'));
            Console.WriteLine($"[{currentPlayer.Stamina} Actions Remaining]\n");
            for (int i = 0; i < playersHand.Count; i++)
            {
                var card = playersHand[i];
                Console.WriteLine($"{i + 1}. {card.Name}    [Cost: {card.Actions} Action(s)]");
                Console.WriteLine($"   - {card.CardAbilitys}\n");
            }
        }

        /// <summary>
        /// Create a centered text within a specified width and fill character.
        /// </summary>
        static string CreateCenteredText(string text = "Example", int width = 50, char fillChar = '-')
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(fillChar, leftPadding) + text + new string(fillChar, rightPadding);
        }
    }
}
