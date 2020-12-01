using System.Collections.Generic;
using Teams.Data.Models;

namespace Teams.Web.ViewModels.Task
{
    public class AllTasksForTeamViewModel
    {
        public IEnumerable<Data.Models.Task> Tasks { get; set; }
        public string TeamName { get; set; }
        public int TeamId { get; set; }
    }
}