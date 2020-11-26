using System.Collections.Generic;
using Teams.Data.Models;

namespace Teams.Web.ViewModels
{
    public class AllTasksForTeamViewModel
    {
        public IEnumerable<Task> Tasks { get; set; }
        public string TeamName { get; set; }
    }
}