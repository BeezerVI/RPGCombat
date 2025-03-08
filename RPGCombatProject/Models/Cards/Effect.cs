namespace RPGCombatProject.Models
{
    public class Effect
    {
        public string EffectName { get; set; }
        public int Duration { get; set; }
        public int Strength { get; set; }

        public bool IsBuff { get; set; }

        public Effect(string effectName, int duration = 1, int strength = 1, bool isBuff = false)
        {
            EffectName = effectName;
            Duration = duration;
            Strength = strength;
            IsBuff = isBuff;
        }
    }
}
