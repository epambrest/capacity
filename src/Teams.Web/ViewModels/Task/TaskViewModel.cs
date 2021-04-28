using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int? SprintId { get; set; }
        public int? MemberId { get; set; }
        public string Link { get; set; }
        public string LinkValidation { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }
        public TeamMemberViewModel TeamMember { get; set; }

        private TaskViewModel(Business.Models.Task task)
        {
            TeamMember = task.MemberId != null ? TeamMemberViewModel.Create(task.TeamMember) : null;
            TeamId = task.TeamId;
            Name = task.Name;
            SprintId = task.SprintId;
            MemberId = task.MemberId;
            StoryPoints = task.StoryPoints;
            Id = task.Id;
            Link = task.Link;
            Completed = task.Completed;
        }

        public static TaskViewModel Create(Business.Models.Task task)
        {
            if (task == null) return null;
            return new TaskViewModel(task);
        }

    }
}
