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
        public static GameState gameState = null!;

        public static void Main(string[] args)
        {
            UIManager.Write("Welcome to the RPG Combat Game!", clearConsole : 1);
            
            SetUpGame();

            UIManager.SetGameState(gameState);

            StartCombatLoop();

            LevelUpPlayers(gameState.PlayerTeam.Cast<PlayerCreature>().ToList());

            UIManager.DisplayGameState(); //for debugging purposes
        }
        static void SetUpGame()
        {
            // Set up enemies as before.
            var enemies = new List<Creature>
            {
                new EnemyCreature("Slim"),
                new EnemyCreature("Giant Bug")
            };

            // Ask how many players are participating.
            //Console.Clear();
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
                UIManager.Write(UIManager.CreateCenteredText($"Setting up Player {i + 1}", 10, '-'), clearConsole : 1);

                // Ask for player's name.
                Console.Clear();
                Console.Write($"Enter Player {i + 1} name: ");
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
                    case "4":
                        chosenClass = "Phoenix";
                        break;
                    default:
                        UIManager.Write("Invalid choice; defaulting to Warrior.", clearConsole : 1);
                        chosenClass = "Warrior";
                        break;
                }

                // Create a new player creature using the factory method in PlayerCreature.
                PlayerCreature newPlayer = PlayerCreature.CreatePlayer(name, chosenClass);
                players.Add(newPlayer);
                UIManager.Write($"Player {name} has joined the adventure as a {chosenClass}.", clearConsole : 1); // Display the player's name and class
            }

            gameState = new GameState(enemies, players)!;

            if (gameState == null)
            {
                throw new InvalidOperationException("Game state must be initialized.");
            }
        }

        static void StartCombatLoop()
        {
            while (true)
            {
                // Combine both teams into one list for turn order.
                var allCreatures = gameState.PlayerTeam.Concat(gameState.EnemyTeam).ToList();

                foreach (var creature in allCreatures)
                {
                    if (creature.IsDead)
                        continue;

                    Console.Clear();
                    UIManager.DisplayGameState();
                    Console.WriteLine($"{creature.Name}'s turn.");

                    creature.PlayTurn();

                    // Clean up any dead creatures after each turn.
                    CleanBattleField(gameState.EnemyTeam, gameState.PlayerTeam);

                    if (IsCombatOver(gameState.EnemyTeam, gameState.PlayerTeam))
                        return;
                }
            }
            // Optionally, you can display a message when combat has ended.
            // UIManager.Write("Combat has fully ended.");
        }


        static void CleanBattleField(List<Creature> enemyTeam, List<Creature> playerTeam)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(enemyTeam);
            CheckIfDeadForAllCreatures(playerTeam);
            // Remove dead creatures from the enemy team
            DeleteDeadCreatures(enemyTeam);
            // (Optional) You may remove dead players if desired
            // DeleteDeadCreatures(playerTeam);
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
