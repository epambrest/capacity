namespace Teams.Business.Models
{
    public class MemberWorkingDaysBusiness
    {
        public int Id { get; set; }
        public int SprintId { get; set; }
        public SprintBusiness Sprint { get; set; }
        public int MemberId { get; set; }
        public TeamMemberBusiness TeamMember { get; set; }
        public int WorkingDays { get; set; }
    }
}
