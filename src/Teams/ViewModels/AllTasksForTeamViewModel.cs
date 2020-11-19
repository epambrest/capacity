using System.Collections.Generic;
using Teams.Models;

namespace Teams.ViewModels
{
    public class AllTasksForTeamViewModel
    {
        public IEnumerable<Task> Tasks { get; set; }
        public string TeamName { get; set; }
    }
}