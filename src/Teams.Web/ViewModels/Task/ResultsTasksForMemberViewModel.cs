using System.Collections.Generic;
using System.Linq;
using Teams.Business.Annotations;
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

        private ResultsTasksForMemberViewModel(Business.Models.Sprint completedSprint,
            Business.Models.TeamMember member, 
            List<TaskViewModel> tasks,
            Dictionary<OtherNamesTaskParams, double> tasksAllParams,
            List<TeamMemberViewModel> teamMemberViewModels)
        {
            TeamMemberId = member.Id;
            TeamId = completedSprint.TeamId;
            CompletedSprintId = completedSprint.Id;
            TeamMemberEmail = member.Member.UserName;
            SprintName = completedSprint.Name;
            Tasks = tasks;
            TeamMembers = teamMemberViewModels;
            TotalStoryPoints = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.TotalStoryPoints);
            QuantityСompletedTasks = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.QuantityСompletedTasks);
            QuantityUnСompletedTasks = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.QuantityUnСompletedTasks);
            SpСompletedTasks = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.SpCompletedTasks);
            SpUnСompletedTasks = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.SpUnCompletedTasks);
            StoryPointsInDay = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.StoryPointsInDay);
        }

        public static ResultsTasksForMemberViewModel Create(Business.Models.Sprint completedSprint,
            Business.Models.TeamMember currentMember,
            Dictionary<OtherNamesTaskParams, double> tasksAllParams)
        {
            var allMemberTasks = completedSprint.Tasks.Where(t => t.MemberId == currentMember.Id).ToList();
            var allSprintTasks = completedSprint.Tasks.ToList();

            List<TaskViewModel> taskViewModelsForMember = new List<TaskViewModel>();
            foreach(var taskMember in allMemberTasks)
            {
                TaskViewModel taskViewModelForMember = TaskViewModel.Create(taskMember);
                taskViewModelsForMember.Add(taskViewModelForMember);
            }

            List<TeamMemberViewModel> teamMemberViewModels = new List<TeamMemberViewModel>();
            foreach(var task in allSprintTasks)
            {
                if (task.TeamMember != null)
                {
                    TeamMemberViewModel teamMemberViewModel = TeamMemberViewModel.Create(task.TeamMember);
                    teamMemberViewModels.Add(teamMemberViewModel);
                }
            }

            return new ResultsTasksForMemberViewModel(completedSprint, currentMember, taskViewModelsForMember, tasksAllParams, teamMemberViewModels);
        }
    }
}
