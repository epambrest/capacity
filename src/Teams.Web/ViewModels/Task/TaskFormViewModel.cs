using System.Collections.Generic;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class TaskFormViewModel
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
        public List<TeamMemberViewModel> TeamMembers { get; set; }
        public List<SprintViewModel> Sprints { get; set; }
    }
}
