namespace Teams.Business.Models
{
    public class Task
    {
        public int Id { get; set; }
        public virtual Team Team { get; set; }
        public string Name { get; set; }
        public virtual Sprint Sprint { get; set; }
        public virtual TeamMember TeamMember { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }
    }
}
