using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Teams.Web.ViewModels.Task;

namespace Teams.Web.ViewModels.Sprint
{
    public class SprintViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Range(1, 30)]
        public int DaysInSprint { get; set; }
        [Range(1, 56)]
        public int StoryPointInHours { get; set; }
        public int Status { get; set; }
        public bool IsOwner { get; set; }
        public List<TaskViewModel> Tasks { get; set; }

        public List<SelectListItem> SelectTasks { get; set; }
        public int[] SelectTasksIds { get; set; }

        public int TotalStoryPoint { get; set; }
        public double AverageStoryPoint { get; set; }

        private SprintViewModel(Business.Models.Sprint sprint, 
            bool isOwner, 
            double averageStoryPoint, 
            List<TaskViewModel> taskViewModels, 
            List<SelectListItem> selectTasks)
        {
            if (sprint != null)
            {
                DaysInSprint = sprint.DaysInSprint;
                Id = sprint.Id;
                Status = sprint.Status;
                Name = sprint.Name;
                StoryPointInHours = sprint.StoryPointInHours;
                TotalStoryPoint = sprint.Tasks.Count > 0 ? sprint.Tasks.Sum(t => t.StoryPoints) : 0;
                TeamId = sprint.TeamId;
            }

            Tasks = taskViewModels;
            SelectTasks = selectTasks;
            AverageStoryPoint = averageStoryPoint;
            IsOwner = isOwner;
        }

        public SprintViewModel() 
        { 
        }

        public static SprintViewModel Create(Business.Models.Sprint sprint, bool isOwner, double averageStoryPoint)
        {
            var taskViewModels = new List<TaskViewModel>();
            var selectTasks = new List<SelectListItem>();

            foreach (var task in sprint.Tasks)
            {
                var taskViewModel = TaskViewModel.Create(task);
                taskViewModels.Add(taskViewModel);
                selectTasks.Add(new SelectListItem(task.Name, task.Id.ToString()));
            }

            return new SprintViewModel(sprint, isOwner, averageStoryPoint, taskViewModels, selectTasks);
        }

    }
}
