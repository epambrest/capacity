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

        private AllTasksForTeamViewModel(Business.Models.Team team, 
            bool isOwner, 
            List<TeamMemberViewModel> teamMemberViewModels,
            List<SprintViewModel> sprintViewModels,
            List<TaskViewModel> taskViewModels)
        {
            if (team != null)
            {
                TeamId = team.Id;
                TeamName = team.TeamName;
            }

            Tasks = taskViewModels;
            Sprints = sprintViewModels;
            Members = teamMemberViewModels;
            IsOwner = isOwner;
        }

        public static AllTasksForTeamViewModel Create(Business.Models.Team team, 
            bool isOwner, 
            List<Business.Models.Sprint> sprints, 
            List<Business.Models.Task> tasks)
        {
            List<TeamMemberViewModel> teamMemberViewModels = new List<TeamMemberViewModel>();
            List<SprintViewModel> sprintViewModels = new List<SprintViewModel>();
            List<TaskViewModel> taskViewModels = new List<TaskViewModel>();

            foreach(var teamMember in team.TeamMembers)
            {
                TeamMemberViewModel teamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                teamMemberViewModels.Add(teamMemberViewModel);
            }

            foreach(var sprint in sprints)
            {
                SprintViewModel sprintViewModel = SprintViewModel.Create(sprint, isOwner, 0);
                sprintViewModels.Add(sprintViewModel);
            }

            foreach(var task in tasks)
            {
                TaskViewModel taskViewModel = TaskViewModel.Create(task);
                taskViewModels.Add(taskViewModel);
            }

            return new AllTasksForTeamViewModel(team, isOwner, teamMemberViewModels, sprintViewModels, taskViewModels);
        }
    }
}