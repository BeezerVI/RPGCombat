// using System;
// using System.Collections.Generic;
// using System.Linq;
// using RPGCombatProject.Models;
// using RPGCombatProject.GameLogic;

// namespace RPGCombatProject.Utilities
// {
//     public static class UIHelper
//     {
//         /// <summary>
//         /// Writes a formatted message to the console with optional input wait and clearing.
//         /// </summary>
//         public static void Write(string text, bool waitForInput = true, bool clearConsole = true)
//         {
//             if (clearConsole)
//             {
//                 Console.Clear();
//             }
//             Console.WriteLine(text);
//             if (waitForInput)
//             {
//                 Console.WriteLine("\nPress Enter to continue...");
//                 Console.ReadLine();
//             }
//         }

//         /// <summary>
//         /// Displays the current game state with players, enemies, and available actions.
//         /// </summary>
//         public static void DisplayGameState(GameState gameState)
//         {
//             Console.Clear();
//             PrintCreatureList("Enemies", gameState.EnemyTeam, gameState.EnemyTargeted);
//             PrintCreatureList("Your Team", gameState.PlayerTeam, gameState.PlayerTargeted);
//             CombatOptions(gameState.ActionsRemaining, gameState.PlayerHand);
//         }

//         /// <summary>
//         /// Prints a list of creatures, highlighting the currently targeted one.
//         /// </summary>
//         private static void PrintCreatureList(string title, List<Creature> creatures, int targetedCreature = 0)
//         {
//             Console.WriteLine(CreateCenteredText(title, 60, '='));

//             for (int i = 0; i < creatures.Count; i++)
//             {
//                 var creature = creatures[i];
//                 string marker = i == targetedCreature ? ">> " : "   ";

//                 Console.WriteLine($"{marker}{creature.Name}");
//                 Console.WriteLine($"   - HP: {creature.Health} / {creature.MaxHealth}" +
//                                 $"{(creature.Shield > 0 ? $" | Shield: {creature.Shield}" : "")}");
//                 Console.WriteLine($"   - Effects: {EffectList(creature.Effects)}\n");
//             }
//         }

//         /// <summary>
//         /// Formats a list of effects for display.
//         /// </summary>
//         private static string EffectList(List<Effect> effects)
//         {
//             if (effects.Count == 0) return "None";
//             return string.Join(", ", effects.Select(e => $"{e.EffectName}{new string('|', e.Duration)}{new string('*', e.Strength)}"));
//         }

//         /// <summary>
//         /// Displays the available combat options.
//         /// </summary>
//         private static void CombatOptions(int actionsRemaining, List<Card> playersHand)
//         {
//             Console.WriteLine(CreateCenteredText("Combat Options", 60, '-'));
//             Console.WriteLine($"[{actionsRemaining} Actions Remaining]\n");
//             for (int i = 0; i < playersHand.Count; i++)
//             {
//                 var card = playersHand[i];
//                 Console.WriteLine($"{i + 1}. {card.Name}    [Cost: {card.Actions} Action(s)]");
//                 Console.WriteLine($"   - {card.CardAbilitys}\n");
//             }
//         }

//         /// <summary>
//         /// Centers text within a specified width, using a given fill character.
//         /// </summary>
//         private static string CreateCenteredText(string text, int width, char fillChar = '-')
//         {
//             if (text.Length >= width) return text;
//             int leftPadding = (width - text.Length) / 2;
//             int rightPadding = width - text.Length - leftPadding;
//             return new string(fillChar, leftPadding) + text + new string(fillChar, rightPadding);
//         }
//     }
// }
