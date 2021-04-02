namespace Teams.Business.Models
{
    public class MemberWorkingDays
    {
        public int Id { get; set; }
        public virtual Sprint Sprint { get; set; }
        public virtual TeamMember TeamMember { get; set; }
        public int WorkingDays { get; set; }
    }
}
