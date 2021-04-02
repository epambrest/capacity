namespace Teams.Business.Models
{
    public class TeamMemberBusiness {
        public int Id { get; set; }
        public virtual TeamBusiness Team { get; set; }
        public virtual UserBusiness Member { get; set; }
    }
}
