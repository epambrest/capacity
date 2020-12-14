using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Teams.Web.ViewModels.Task;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 14)]
        public int DaysInSprint { get; set; }
        [Range(1, 56)]
        public int StoryPointInHours { get; set; }
        public bool IsActive { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }
}
