namespace Teams.Business.Models
{
    public class TaskBusiness
    {
        public int Id { get; set; }
        public virtual TeamBusiness Team { get; set; }
        public string Name { get; set; }
        public virtual SprintBusiness Sprint { get; set; }
        public virtual TeamMemberBusiness TeamMember { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }
    }
}
