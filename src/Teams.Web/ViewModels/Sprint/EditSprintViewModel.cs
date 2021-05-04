using System.ComponentModel.DataAnnotations;

namespace Teams.Web.ViewModels.Sprint
{
    public class EditSprintViewModel
    {
        [Required]
        [StringLength(255)]
        public string SprintName { get; set; }
        public int SprintId { get; set; }
        [Range(1, 14)]
        public int SprintDaysInSprint { get; set; }
        [Range(1, 56)]
        public int SprintStorePointInHours { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }

        private EditSprintViewModel(Business.Models.Sprint sprint, string errorMessage, Business.Models.Team team)
        {
            if (team != null)
            {
                TeamId = team.Id;
                TeamName = team.TeamName;
            }

            if (sprint != null)
            {
                SprintId = sprint.Id;
                SprintName = sprint.Name;
                SprintDaysInSprint = sprint.DaysInSprint;
                SprintStorePointInHours = sprint.StoryPointInHours;
                Status = sprint.Status;
            }

            ErrorMessage = errorMessage;
        }

        public EditSprintViewModel() 
        { 
        }

        public static EditSprintViewModel Create(Business.Models.Sprint sprint, string errorMessage, Business.Models.Team team) => 
            new EditSprintViewModel(sprint, errorMessage, team);
    }
}
