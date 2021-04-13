namespace Teams.Business.Models
{
    public class MemberWorkingDays
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public Sprint Sprint { get; set; }
        public int MemberId { get; set; }
        public TeamMember TeamMember { get; set; }
        public int WorkingDays { get; set; }
    }
}
