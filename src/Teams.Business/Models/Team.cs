using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }
}
