using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int TeamId { get; set; }
        [Range(1, int.MaxValue)]
        public int TaskSprintId { get; set; }
        [Required]
        [RegularExpression(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")]
        public string TaskLink { get; set; }
        public string TeamName { get; set; }
        [Required]
        public string TaskName { get; set; }
        public string ErrorMessage { get; set; }
        public List<TeamMemberViewModel> TeamMembers { get; set; }
        public List<SprintViewModel> Sprints { get; set; }
    }
}
