using System;
using System.Collections.Generic;
using RPGCombatProject.Models;
using RPGCombatProject.GameLogic;
using RPGCombatProject.Utilityes;
using System.Linq;
using GameState = RPGCombatProject.GameLogic.GameState;

namespace RPGCombatProject
{
    public class Program
    {
        private static GameState gameState = null!;
        private static int playerIndex;

        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the RPG Combat Game!");
            
            SetUpCombat();

            UIManager.SetGameState(gameState);
            StartCombatLoop();

            StartCombatLoop();

            LevelUpPlayers(gameState.PlayerTeam.Cast<PlayerCreature>().ToList());

            UIManager.DisplayGameState();
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
                        UIManager.SetCurrentPlayerIndex(playerIndex); // Update UIManager with the current player's index
                        PlayerTurnForPlayer(i);
                    }
                    else
                    {
                        UIManager.Write($"{gameState.PlayerTeam[i].Name} is dead and cannot act.");
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
            UIManager.Write("Combat has fully ended.");
        }

        static void PlayerTurnForPlayer(int playerIndex)
        {
            // Get the current player.
            PlayerCreature? currentPlayer = gameState.PlayerTeam[playerIndex] as PlayerCreature;
            if (currentPlayer == null)
            {
                UIManager.Write("Error: The current player is not a PlayerCreature.");
                return;
            }

            currentPlayer.Stamina = 3;
            UIManager.Write($"{currentPlayer.Name}'s turn begins.");

            bool isPlayerTurn = true;
            while (isPlayerTurn == true)
            {
                // Display game state
                UIManager.DisplayGameState();

                isPlayerTurn = PlayerCombatOptions(currentPlayer);

                CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam);
                if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                {
                    break;
                }
            }
            UIManager.Write($"{currentPlayer.Name}'s turn is over.");
        }

        static void EnemysTurn()
        {
            UIManager.Write("Enemy's turn.");
            foreach (var enemy in gameState.EnemyTeam)
            {
                if (enemy.IsDead) continue;

                // Get a list of alive players.
                var viablePlayers = gameState.PlayerTeam.Where(p => !p.IsDead).ToList();
                if (!viablePlayers.Any())
                {
                    UIManager.Write("All players are defeated! Enemies win!");
                    return;
                }

                // If the enemy is an EnemyCreature (it should be), use its Act method.
                if (enemy is EnemyCreature e)
                {
                    Console.Clear();                    
                    UIManager.DisplayGameState();
                    Console.WriteLine($"{enemy.Name}'s turn.");
                    e.Act(viablePlayers);
                    Console.ReadLine();
                }
                else
                {
                    // Fallback behavior if enemy is not of type EnemyCreature.
                    UIManager.Write("Error: Enemy is not an EnemyCreature.");
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
                UIManager.Write("Input cannot be null. Please enter a valid input.");
                return true;
            }

            if (input.ToUpper() == "E")
            {
                UIManager.Write($"{currentPlayer.Name} has ended their turn.");
                return false;
            }

            if (!int.TryParse(input, out int cardNumber) || cardNumber < 1 || cardNumber > currentPlayer.Hand.Count)
            {
                UIManager.Write("Invalid input. Please enter a valid card number.");
                return true;
            }

            // Get the selected card from the current player's hand.
            Card selectedCard = currentPlayer.Hand[cardNumber - 1];
            UIManager.Write($"{currentPlayer.Name} selected card: {selectedCard.Name}");

            if (selectedCard.Actions > currentPlayer.Stamina)
            {
                UIManager.Write("Not enough actions to play this card. Please select another card.");
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

        static bool IsCombatOver(List<Creature> enemyTeam, List<Creature> playerTeam)
        {
            // Check if there are no enemies left
            if (enemyTeam.Count == 0)
            {
                UIManager.Write("There are no enemies left. You have won!");
                return true;
            }

            // Check if there are no players left
            else if (playerTeam.Count == 0)
            {
                UIManager.Write("There are no players left. Game over!");
                return true;
            }

            // Check if all enemies are dead
            else if (enemyTeam.All(e => e.IsDead))
            {
                UIManager.Write("You have defeated all enemies!");
                return true;
            }

            // Check if all players are dead
            else if (playerTeam.All(p => p.IsDead))
            {
                UIManager.Write("All players have been defeated. Game over!");
                return true;
            }

            else
            {
                return false;
            }
        }

        static void LevelUpPlayers(List<PlayerCreature> players)
        {
            Console.WriteLine("\n--- Leveling Up ---");
            
            foreach (var player in players)
            {
                player.LevelUp(); // Give them a level and an Upgrade Point

                Console.WriteLine($"\n{player.Name} (Level {player.Level}) - Upgrade Points: {player.UpgradePoints}");
                Console.WriteLine("Would you like to spend your upgrade points now? (Y/N)");

                string? input = Console.ReadLine();
                if (input?.ToUpper() == "Y")
                {
                    player.UpgradeMenu(); // Allow them to spend upgrade points
                }
                else
                {
                    Console.WriteLine($"{player.Name} saved their points for later.");
                }
            }

            Console.WriteLine("--- All Players Finished Leveling Up ---\n");
        }
    }
}
