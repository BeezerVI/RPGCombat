namespace RPGCombatProject.Models
{
    public class PlayerCreature : Creature
    {
        public int Stamina { get; set; }

        public PlayerCreature(string name, int maxHealth = 100, int health = 100, int shield = 0, int stamina = 50)
            : base(name, maxHealth, health, shield)
        {
            Stamina = stamina;
        }

        public override void Attack(Creature target)
        {
            Console.WriteLine($"{Name} attacks {target.Name} for 10 damage!");
            target.ApplyDamage(10);
        }
    }
}
