using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class TaskFormViewModel
    {
        public int TaskId { get; set; }
        [Range(1, 100)]
        public int TaskStoryPoints { get; set; }
        public int? TaskMemberId { get; set; }
        public string TaskMemberName { get; set; }
        public int TeamId { get; set; }
        [Required]
        public int TaskSprintId { get; set; }
        [Required]
        public string TaskLink { get; set; }
        public string LinkValidation { get; set; }
        public string TeamName { get; set; }
        [Required]
        public string TaskName { get; set; }
        public string ErrorMessage { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
        public List<SprintViewModel> Sprints { get; set; }

        private TaskFormViewModel(Business.Models.Task task, 
            string taskMemberName,
            string errorMessage, 
            List<TeamMemberViewModel> teamMemberViewModels)
        {
            TeamId = task.TeamId;
            TaskId = task.Id;
            TaskSprintId = task.SprintId.GetValueOrDefault();
            TeamName = task.Team.TeamName;
            TaskName = task.Name;
            TaskLink = task.Link;
            TaskStoryPoints = task.StoryPoints;
            TaskMemberId = task.MemberId;
            ErrorMessage = errorMessage;
            TaskMemberName = taskMemberName;
            TeamMembers = teamMemberViewModels;
        }

        public TaskFormViewModel() 
        { 
        }

        public static TaskFormViewModel Create(Business.Models.Task task, 
            string errorMessage, 
            List<Business.Models.TeamMember> teamMembers,
            List<Business.Models.Sprint> sprints)
        {
            var teamMember = teamMembers.FirstOrDefault(t => t.Id == task.MemberId);
            var teamMemberViewModels = new List<TeamMemberViewModel>();
            string taskMemberName;

            if (teamMember != null) taskMemberName = teamMember.Member.UserName;
            else taskMemberName = "";

            foreach(var member in teamMembers)
            {
                if (member != null)
                {
                    var teamMemberViewModel = TeamMemberViewModel.Create(member);
                    teamMemberViewModels.Add(teamMemberViewModel);
                }
            }

            var sprintViewModels = new List<SprintViewModel>();
            foreach(var sprint in sprints)
            {
                var sprintViewModel = SprintViewModel.Create(sprint, false, 0);
                sprintViewModels.Add(sprintViewModel);
            }

            return new TaskFormViewModel(task, taskMemberName, errorMessage, teamMemberViewModels);
        }
    }
}
