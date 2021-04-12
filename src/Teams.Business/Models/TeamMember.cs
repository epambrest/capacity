namespace Teams.Business.Models
{
    public class TeamMember 
    {
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public string MemberId { get; set; }
        public User Member { get; set; }
    }
}
