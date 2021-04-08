using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class TeamBusiness
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamOwner { get; set; }
        public UserBusiness Owner { get; set; }
        public List<TeamMemberBusiness> TeamMembers { get; set; } = new List<TeamMemberBusiness>();
    }
}
