namespace RPGCombatProject.Models
{
    public class EnemyCreature : Creature
    {
        public EnemyCreature(string name, int maxHealth = 100, int health = 100, int shield = 0, List<Effect>? effects = null)
            : base(name, maxHealth, health, shield, effects)
        {
        }

        public override void Attack(Creature target)
        {
            Console.WriteLine($"{Name} attacks {target.Name} for 5 damage!");
            target.ApplyDamage(5);
        }
    }
}
