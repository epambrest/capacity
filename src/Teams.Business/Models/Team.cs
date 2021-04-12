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
    }
}
