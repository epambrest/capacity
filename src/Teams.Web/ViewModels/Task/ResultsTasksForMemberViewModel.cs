using System.Collections.Generic;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class ResultsTasksForMemberViewModel
    {
        public int TeamMemberId { get; set; }

        public int TeamId { get; set; }
        public int CompletedSprintId { get; set; }
        public string SprintName { get; set; }
        public string TeamMemberEmail { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
        public int TotalStoryPoints { get; set; }
        public int QuantityСompletedTasks { get; set; }
        public int QuantityUnСompletedTasks { get; set; }

        public int SpСompletedTasks { get; set; }
        public int SpUnСompletedTasks { get; set; }

        public double StoryPointsInDay { get; set; }
    }
}
