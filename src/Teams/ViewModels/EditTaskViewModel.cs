
using System.Collections.Generic;

namespace Teams.Models
{
    public class EditTaskViewModel
    {
        public int TaskId { get; set; }
        public int TaskStoryPoints { get; set; }
        public int TaskMemberId { get; set; }
        public int TeamId { get; set; }
        public int TaskSprintId { get; set; }
        public string TaskLink { get; set; }
        public string TeamName { get; set; }
        public string TaskName { get; set; }
        public string ErrorMessage { get; set; }
        public List<TeamMember> TeamMembers { get; set; }
    }
}
