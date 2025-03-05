// Purpose: Contains the GameState class, which is used to store the current state of the game. This includes the player and enemy teams.
using System.Collections.Generic;
using RPGCombatProject.Models;

namespace RPGCombatProject.GameLogic
{
    public class GameState
    {
        public List<Creature> EnemyTeam { get; set; }
        public List<Creature> PlayerTeam { get; set; }

        public GameState(List<Creature> enemyTeam, List<Creature> playerTeam)
        {
            EnemyTeam = enemyTeam;
            PlayerTeam = playerTeam;
        }
    }

}
