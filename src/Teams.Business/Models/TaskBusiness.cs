namespace Teams.Business.Models
{
    public class TaskBusiness
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public TeamBusiness Team { get; set; }
        public string Name { get; set; }
        public int? SprintId { get; set; }
        public SprintBusiness Sprint { get; set; }
        public int? MemberId { get; set; }
        public TeamMemberBusiness TeamMember { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }
    }
}
