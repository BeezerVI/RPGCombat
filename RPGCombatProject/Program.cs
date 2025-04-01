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

            gameState = new GameState(new List<Team> { enemyTeam, playerTeam });
            
            UIManager.Write("Welcome to the RPG Combat Game!", clearConsole: 3);

            UIManager.SetGameState(gameState); //This is needed for UIManager to display the game state
            
            SetUpGame(); // Set up the game with players and enemies

            int currentRound = 1;

            while (true)
            {
                enemyTeam.Members.AddRange(GenerateEnemiesForRound(currentRound)); // Generate new enemies for the current round

                HealAllMembers(playerTeam.Members); // Heal all players at the start of each round
                UIManager.Write($"--- Round {currentRound} ---", clearConsole: 2);

                StartCombatLoop(); // Start the combat loop

                if (playerTeam.Members.All(player => player.IsDead))
                {
                    UIManager.Write("All Heros Have Fallen");
                    UIManager.Write(UIManager.CreateCenteredText("--Game Over--", 60, 'X'), clearConsole: 3);
                    break;
                }

                LevelUpPlayers(playerTeam.Members); // Level up the players after combat

                currentRound++;
            }

        }

        static void SetUpGame()
        {
            // Initialize enemies
            // UIManager.InitializeEnemies();

            UIManager.SetUpPlayers(); // Set up the players
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

        static void CleanBattleField(List<Creature> team)
        {
            // Check if any creatures are dead and remove them from the list
            CheckIfDeadForAllCreatures(team);
            // Remove dead creatures from the team
            DeleteDeadCreatures(team);
        }

        static void HealAllMembers(List<Creature> teamMembers)
        {
            // Heal all players at the start of each round
            foreach (var creature in teamMembers)
            {
                // Add logic to heal the creature
                creature.Health = creature.MaxHealth; // Reset health to max for simplicity
            }
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
                if (player is PlayerCreature)
                {
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
        /// <summary>
        /// Generates new enemies for the current round. 
        /// As rounds increase, enemies get tougher and more numerous.
        /// </summary>
        /// <param name="round">The current round number (starting at 1).</param>
        /// <returns>A list of new EnemyCreature objects for this round.</returns>
        static List<EnemyCreature> GenerateEnemiesForRound(int round = 1)
        {
            List<EnemyCreature> newEnemies = new List<EnemyCreature>();
            // List of possible enemy names (must match names in your JSON)
            string[] enemyTemplates = new string[] 
            { 
                "Slime", "Giant Bug", "Fire Elemental", 
                "Frost Wraith", "Goblin Shaman", "Dark Knight", "Hydra", 
                "Shadow Assassin", "Stone Golem", "Necromancer", "Dragon", 
                "Vampire Lord", "Thunder Titan" 
            };

            Random rnd = new Random();
            // For example, number of enemies increases with the round number.
            int numEnemies = Math.Min(round, enemyTemplates.Length);

            for (int i = 0; i < numEnemies; i++)
            {
                // Pick a random enemy template.
                string templateName = enemyTemplates[rnd.Next(enemyTemplates.Length)];
                try
                {
                    EnemyCreature enemy = EnemyLoader.GetEnemy(templateName);

                    // Scale enemy stats based on the round number.
                    // Increase health and shield for later rounds.
                    enemy.MaxHealth += round * 10;
                    enemy.Health = enemy.MaxHealth;  // Reset health to new max.
                    enemy.Shield += round * 2;
                    // Optionally scale other stats or modify abilities.
                    
                    newEnemies.Add(enemy);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Error generating enemy: {ex.Message}");
                }
            }
            return newEnemies;
        }

    }
}