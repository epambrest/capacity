using System.Collections.Generic;
using Teams.Data.Models;

namespace Teams.Web.ViewModels.Task
{
    public class AllTasksForTeamViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
        public bool IsOwner { get; set; }
    }
}