using System.Collections.Generic;

namespace Teams.Data.Models
{
    public class CombinedModel
    {
        public List<Sprint> Sprints { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
