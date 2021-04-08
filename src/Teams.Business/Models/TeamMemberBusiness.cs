namespace Teams.Business.Models
{
    public class TeamMemberBusiness 
    {
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public TeamBusiness Team { get; set; }
        public string MemberId { get; set; }
        public UserBusiness Member { get; set; }
    }
}
