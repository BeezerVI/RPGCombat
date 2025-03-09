namespace RPGCombatProject.Models
{
    public class Effect
    {
        public string EffectName { get; set; }
        public int Duration { get; set; }
        public int Strength { get; set; }

        public Effect(string effectName, int duration = 0, int strength = 0)
        {
            EffectName = effectName;
            Duration = duration;
            Strength = strength;
        }
    }
}
