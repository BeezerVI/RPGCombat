using System.Collections.Generic;
using RPGCombatProject.Models;

namespace RPGCombatProject.GameLogic
{
    public class GameState
    {
        public List<Creature> EnemyTeam { get; set; }
        public List<Creature> PlayerTeam { get; set; }
        public int ActionsRemaining { get; set; }

        public GameState(List<Creature> enemyTeam, List<Creature> playerTeam, int actionsRemaining)
        {
            EnemyTeam = enemyTeam;
            PlayerTeam = playerTeam;
            ActionsRemaining = actionsRemaining;
        }
    }

}
