namespace Teams.Business.Models
{
    public class MemberWorkingDaysBusiness
    {
        public int Id { get; set; }
        public virtual SprintBusiness Sprint { get; set; }
        public virtual TeamMemberBusiness TeamMember { get; set; }
        public int WorkingDays { get; set; }
    }
}
