namespace Teams.Web.ViewModels.MemberWorkingDays
{
    public class MemberWorkingDaysViewModel
    {       
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SprintId { get; set; }
        public int WorkingDays { get; set; }
    }
}
