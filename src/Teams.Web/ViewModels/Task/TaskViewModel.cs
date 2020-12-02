using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.ViewModels.Task
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int SprintId { get; set; }
        public int MemberId { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public TeamMemberViewModel TeamMember { get; set; }
    }
}
