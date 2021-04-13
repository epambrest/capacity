namespace Teams.Business.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string Name { get; set; }
        public int? SprintId { get; set; }
        public Sprint Sprint { get; set; }
        public int? MemberId { get; set; }
        public TeamMember TeamMember { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }
    }
}
