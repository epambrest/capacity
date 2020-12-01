using System.Collections.Generic;
using Teams.Data.Models;

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
        public List<Data.Models.TeamMember> TeamMembers { get; set; }
        public List<Data.Models.Sprint> Sprints { get; set; }
    }
}
