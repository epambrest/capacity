using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public User Owner { get; set; }
        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        private Team(string teamOwner, string teamName, int id = 0, List<TeamMember> teamMembers = null , User owner = null)
        {
            TeamOwner = teamOwner;
            TeamName = teamName;
            Id = id;
            TeamMembers = teamMembers;
            Owner = owner;
        }

        public Team() 
        { 
        }

        public static Team Create(int id, string teamOwner, string teamName, List<TeamMember> teamMembers) => 
            new Team(teamOwner, teamName, id, teamMembers);

        public static Team Create(string teamOwner, string teamName) => new Team(teamOwner, teamName);

        public void SetTeamMembers(List<TeamMember> teamMembers) => TeamMembers = teamMembers;

    }
}
