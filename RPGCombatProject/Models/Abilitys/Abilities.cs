using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using RPGCombatProject.GameLogic;
using RPGCombatProject.Utilityes;

namespace RPGCombatProject.Models
{
    public static class Abilities
    {
        private static readonly Dictionary<string, Ability> abilityRegistry = new Dictionary<string, Ability>();

        // Static constructor loads abilities when the game starts.
        static Abilities()
        {
            LoadAbilitiesFromJson("abilities.json");
        }

        private static void LoadAbilitiesFromJson(string fileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "Abilitys", fileName);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: {filePath} not found.");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var abilities = JsonSerializer.Deserialize<List<AbilityData>>(json);

                if (abilities != null)
                {
                    foreach (var abilityData in abilities)
                    {
                        var effects = new List<Effect>();
                        foreach (var effect in abilityData.Effects)
                        {
                            effects.Add(new Effect(effect.EffectName, effect.Duration, effect.Strength));
                        }

                        var ability = new Ability(
                            abilityData.Name, 
                            abilityData.Cost,
                            Enum.Parse<AbilityType>(abilityData.Type),
                            Enum.Parse<TargetingMethod>(abilityData.Targeting),
                            Enum.Parse<TargetTeam>(abilityData.TeamTarget),
                            abilityData.CanTargetSelf,
                            effects
                        );

                        abilityRegistry[abilityData.Name.ToLower()] = ability;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load abilities: {ex.Message}");
            }
        }


        // Retrieves an ability by name
        public static Ability GetAbility(string abilityName)
        {
        //    UIManager.Write($"Looking for ability: {abilityName}"); for debugging
            string formattedAbilityName = abilityName.Trim().ToLower();
            //Console.WriteLine($"Looking for ability: {formattedAbilityName}");
            if (abilityRegistry.TryGetValue(formattedAbilityName, out Ability? ability))
            {
                //Console.WriteLine($"Found ability: {ability.Name}");
                return new Ability(ability.Name, ability.Cost, ability.Type, ability.Targeting, ability.TeamTarget, ability.CanTargetSelf, new List<Effect>(ability.Effects), ability.ChainAction);
            }
            throw new ArgumentException($"Ability '{formattedAbilityName}' not found.");
        }
    }

    // A helper class for JSON deserialization
    public class AbilityData
    {
        public string Name { get; set; } = string.Empty;
        public int Cost { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Targeting { get; set; } = string.Empty;
        public string TeamTarget { get; set; } = string.Empty;
        public bool CanTargetSelf { get; set; }
        public List<EffectData> Effects { get; set; } = new List<EffectData>();
    }

    public class EffectData
    {
        public string EffectName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int Strength { get; set; }
    }
}
