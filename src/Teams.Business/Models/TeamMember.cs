namespace Teams.Business.Models
{
    public class TeamMember {
        public int Id { get; set; }
        public virtual Team Team { get; set; }
        public virtual User Member { get; set; }
    }
}
