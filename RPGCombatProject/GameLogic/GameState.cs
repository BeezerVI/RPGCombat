// Purpose: Contains the GameState class, which is used to store the current state of the game. This includes the teams.
using System.Collections.Generic;
using RPGCombatProject.Models;


namespace RPGCombatProject.GameLogic
{
    public class GameState
    {
        public List<Team> Teams { get; set; }

        public GameState(List<Team> teams)
        {
            Teams = teams;
        }

        public void AddTeam(Team team)
        {
            Teams.Add(team);
        }
    }
}

namespace RPGCombatProject.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Creature> Members { get; set; }

        public Team(string name, List<Creature> members)
        {
            Name = name;
            Members = members;
        }
    }
}
