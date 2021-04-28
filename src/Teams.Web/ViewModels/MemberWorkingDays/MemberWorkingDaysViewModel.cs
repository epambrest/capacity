namespace Teams.Web.ViewModels.MemberWorkingDays
{
    public class MemberWorkingDaysViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SprintId { get; set; }
        public int WorkingDays { get; set; }

        private MemberWorkingDaysViewModel(Business.Models.MemberWorkingDays memberWorkingDays)
        {
            Id = memberWorkingDays.Id;
            SprintId = memberWorkingDays.SprintId;
            MemberId = memberWorkingDays.MemberId;
            WorkingDays = memberWorkingDays.WorkingDays;
        }

        public static MemberWorkingDaysViewModel Create(Business.Models.MemberWorkingDays memberWorkingDays)
        {
            return new MemberWorkingDaysViewModel(memberWorkingDays);
        }
    }
}
