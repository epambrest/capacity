using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class ResultsTasksForMemberViewModel
    {
        public int teamMemberId { get; set; }

        public int teamId { get; set; }
        public int completedSprintId { get; set; }
        public string sprintName { get; set; }
        public string teamMemberEmail { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
        public int TotalStoryPoints { get; set; }
    }
}
