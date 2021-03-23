using System.Collections.Generic;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class AllTasksForTeamViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
        public List<SprintViewModel> Sprints { get; set; }
        public List<TeamMemberViewModel> Members { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public bool IsOwner { get; set; }
    }
}