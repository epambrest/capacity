using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class TeamBusiness
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public virtual UserBusiness Owner { get; set; }
        public virtual ICollection<TeamMemberBusiness> TeamMembers { get; set; }
    }
}
