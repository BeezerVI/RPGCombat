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
            var enemyTeam = new Team("Enemy Team", new List<Creature> { /* enemy creatures */ });
            var playerTeam = new Team("Player Team", new List<Creature> { /* player creatures */ });

            var gameState = new GameState(new List<Team> { enemyTeam, playerTeam });
            
            UIManager.Write("Welcome to the RPG Combat Game!", clearConsole : 1);
            
            SetUpGame();

            UIManager.SetGameState(gameState);

            StartCombatLoop();

           LevelUpPlayers(playerTeam.Members);

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

            if (gameState == null)
            {
                throw new InvalidOperationException("Game state must be initialized.");
            }
        }

        static void StartCombatLoop()
        {
            while (true)
            {
                // Combine all teams into one list for turn order.
                var allCreatures = gameState.Teams.SelectMany(team => team.Members).ToList();

                foreach (var creature in allCreatures)
                {
                    if (creature.IsDead)
                        continue;

                    Console.Clear();
                    UIManager.DisplayGameState();
                    Console.WriteLine($"{creature.Name}'s turn.");

                    creature.PlayTurn();

                    // Clean up any dead creatures after each turn.
                    foreach (var team in gameState.Teams)
                    {
                        CleanBattleField(team.Members);
                    }

                    if (IsCombatOver(gameState.Teams))
                        return;
                }
            }
            // Optionally, you can display a message when combat has ended.
            // UIManager.Write("Combat has fully ended.");
        }


        static void CleanBattleField(List<Creature> Team)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(Team);
            // Remove dead creatures from the enemy team
            DeleteDeadCreatures(Team);
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

        static bool IsCombatOver(List<Team> teams)
        {
            // Count the number of teams that still have living members
            int teamsWithLivingMembers = teams.Count(team => team.Members.Any(member => !member.IsDead));

            // If only one team has living members, the combat is over
            if (teamsWithLivingMembers <= 1)
            {
                var winningTeam = teams.FirstOrDefault(team => team.Members.Any(member => !member.IsDead));
                if (winningTeam != null)
                {
                    UIManager.Write($"The {winningTeam.Name} has won the combat!");
                }
                else
                {
                    UIManager.Write("All teams have been defeated. Game over!");
                }
                return true;
            }

            // If more than one team has living members, the combat is not over
            return false;
        }

        static void LevelUpPlayers(List<Creature> players)
        {
            Console.WriteLine("\n--- Leveling Up ---");
            
            foreach (var player in players)
            {
                // if (player is not PlayerCreature)
                // {
                //     continue;
                // }
                if (player is PlayerCreature){
                    ((PlayerCreature)player).LevelUp(); // Give them a level and an Upgrade Point

                    Console.WriteLine($"\n{player.Name} (Level {((PlayerCreature)player).Level}) - Upgrade Points: {((PlayerCreature)player).UpgradePoints}");
                    Console.WriteLine("Would you like to spend your upgrade points now? (Y/N)");

                    string? input = Console.ReadLine();
                    if (input?.ToUpper() == "Y")
                    {
                        ((PlayerCreature)player).UpgradeMenu(); // Allow them to spend upgrade points
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} saved their points for later.");
                    }
                }
                else
                {
                    Console.WriteLine($"{player.Name} is not a player creature and cannot be leveled up.");
                }
                
            }

            Console.WriteLine("--- All Players Finished Leveling Up ---\n");
        }
    }
}
