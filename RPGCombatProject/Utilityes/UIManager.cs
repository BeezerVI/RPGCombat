using System;
using System.Collections.Generic;
using RPGCombatProject.Models;
using RPGCombatProject.GameLogic;

namespace RPGCombatProject.Utilityes
{
    /// <summary>
    /// Manages all UI-related functionalities such as displaying game state, messages, and creature details.
    /// </summary>
    public static class UIManager
    {
        private static GameState? gameState;

        /// <summary>
        /// Sets the current game state for UIManager to reference.
        /// </summary>
        /// <param name="state">The game state to set.</param>
        public static void SetGameState(GameState state)
        {
            gameState = state;
        }

        /// <summary>
        /// Writes a message to the console with optional clearing and waiting for input.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="waitForInput">If true, waits for user input before continuing.</param>
        /// <param name="clearConsole">If 1, clears the console before displaying text. If 2, clears console and then calls DisplayGameState, if 3 does not claer console at all</param>
        public static void Write(string text, bool waitForInput = true, int clearConsole = 2)
        {
            if (clearConsole == 1)
            {
                Console.Clear();
            }
            else if (clearConsole == 2)
            {
                Console.Clear();
                DisplayGameState();
            }
            else if (clearConsole == 3)
            {
                // Do nothing
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
        /// Displays the current game state, including enemies and player team information.
        /// </summary>
        public static void DisplayGameState()
        {
            Console.Clear();
            Console.WriteLine(CreateCenteredText("Combat Table", 60, ' '));
            if (gameState == null)
            {
                Console.WriteLine("Error: Game state is not set.");
                return;
            }

            // Iterate over all teams and display their members
            foreach (var team in gameState.Teams)
            {
                PrintCreatureList(team.Name, team.Members);
            }
        }

        /// <summary>
        /// Prints a list of creatures, displaying their health, shields, and effects.
        /// </summary>
        /// <param name="title">The title of the creature list (e.g., "Enemies" or "Your Team").</param>
        /// <param name="creatures">The list of creatures to display.</param>
        public static void PrintCreatureList(string title, List<Creature> creatures)
        {
            Console.WriteLine(CreateCenteredText(title, 60, '='));

            foreach (var creature in creatures)
            {
                string status = creature.IsDead ? " [DEAD]" : "";
                Console.WriteLine($"   {creature.Name}{status}");
                Console.WriteLine($"   - HP: {creature.Health} / {creature.MaxHealth}" +
                                $"{(creature.Shield > 0 ? $" | Shield: {creature.Shield}" : "")}");
                Console.WriteLine($"   - Effects: {EffectList(creature.Effects)}\n");
            }
        }

        /// <summary>
        /// Converts a list of effects into a formatted string.
        /// </summary>
        /// <param name="effects">The list of effects applied to a creature.</param>
        /// <returns>A formatted string displaying the effects.</returns>
        public static string EffectList(List<Effect> effects)
        {
            if (effects.Count == 0) return "None";
            return string.Join(", ", effects.Select(e => $"{e.EffectName}{new string('|', e.Duration)}{new string('*', e.Strength)}"));
        }

        /// <summary>
        /// Creates a centered text banner with a specified width and fill character.
        /// </summary>
        /// <param name="text">The text to center.</param>
        /// <param name="width">The width of the banner.</param>
        /// <param name="fillChar">The character used to fill empty space.</param>
        /// <returns>A formatted string with centered text.</returns>
        public static string CreateCenteredText(string text = "Example", int width = 50, char fillChar = '-')
        {
            if (text.Length >= width) return text;
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(fillChar, leftPadding) + text + new string(fillChar, rightPadding);
        }

        /// <summary>
        /// Displays the available combat options based on the player's hand and actions remaining.
        /// </summary>
        /// <param name="currentPlayer">The current player's object.</param>
        public static void CombatOptions(Creature currentPlayer)
        {
            var playersHand = currentPlayer.Hand;
            Console.WriteLine(CreateCenteredText("Combat Options", 60, '-'));
            Console.WriteLine($"[{currentPlayer.Stamina} Actions Remaining]\n");
            for (int i = 0; i < playersHand.Count; i++)
            {
                var card = playersHand[i];
                Console.WriteLine($"{i + 1}. {card.Name}    [Cost: {card.Cost} Action(s)]");
                //Console.WriteLine($"   - {card.CardAbilitys}\n");
            }
        }

        public static bool PlayerCombatOptions(Creature currentPlayer)
        {
            if (currentPlayer is PlayerCreature)
            {
                if (currentPlayer == null)
                {
                    Write("Current player is null. Ending turn.");
                    return false;
                }
                if (currentPlayer.Hand.Count == 0)
                {
                    Write("No cards in hand. Ending turn.");
                    return false;
                }
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
                Ability selectedCard = currentPlayer.Hand[cardNumber - 1];
                Write($"{currentPlayer.Name} played: {selectedCard.Name}", waitForInput: false);

                if (selectedCard.Cost > currentPlayer.Stamina)
                {
                    Write("Not enough actions to play this card. Please select another card.");
                    return true;
                }

                // Execute ability
                if (gameState != null)
                {
                    selectedCard.Execute(gameState, currentPlayer);
                }
                else
                {
                    Write("Error: Game state is not set.");
                }

                return true;
            }
            else
            {
                Console.WriteLine($"{currentPlayer.Name} is doing there turn.");
                return true;
            }
        }

        public static void SetUpPlayers(){
            // Ask how many players are participating.
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
                UIManager.Write(UIManager.CreateCenteredText($"Setting up Player {i + 1}", 10, '-'), clearConsole: 1);

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
                        UIManager.Write("Invalid choice; defaulting to Warrior.", clearConsole: 1);
                        chosenClass = "Warrior";
                        break;
                }

                // Create a new player creature using the factory method in PlayerCreature.
                PlayerCreature newPlayer = PlayerCreature.CreatePlayer(name, chosenClass);
                players.Add(newPlayer);
                UIManager.Write($"Player {name} has joined the adventure as a {chosenClass}.", clearConsole: 1); // Display the player's name and class
            }

            // Add players to the player team
            if (gameState == null)
            {
                throw new InvalidOperationException("Game state must be initialized.");
            }

            var playerTeam = gameState.Teams.FirstOrDefault(team => team.Name == "Player Team");
            if (playerTeam != null)
            {
                playerTeam.Members.AddRange(players);
            }
            else
            {
                throw new InvalidOperationException("Player team not found in game state.");
            }

        }

        public static void InitializeEnemies()
        {
            if (gameState == null)
            {
                throw new InvalidOperationException("Game state must be initialized.");
            }
            var enemyTeam = gameState.Teams.FirstOrDefault(team => team.Name == "Enemy Team");
            if (enemyTeam != null)
            {
                enemyTeam.Members.Add(new EnemyCreature("Slime"));
                enemyTeam.Members.Add(new EnemyCreature("Giant Bug"));
                // Add more enemies here as needed
            }
            else
            {
                throw new InvalidOperationException("Enemy team not found in game state.");
            }
        }
    }
}
