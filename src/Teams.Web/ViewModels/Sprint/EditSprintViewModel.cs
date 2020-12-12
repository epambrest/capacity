using System.ComponentModel.DataAnnotations;

namespace Teams.Web.ViewModels.Sprint
{
    public class EditSprintViewModel
    {
        [Required]
        public string SprintName { get; set; }
        public int SprintId { get; set; }
        [Range(1, 14)]
        public int SprintDaysInSprint { get; set; }
        [Range(1, 56)]
        public int SprintStorePointInHours { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsActive { get; set; }
    }
}
