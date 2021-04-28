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

        private MemberWorkingDays(int memberId, 
            int sprintId, 
            Sprint sprint, 
            int workingDays,
            int id = 0,
            TeamMember teamMember = null)
        {
            MemberId = memberId;
            SprintId = sprintId;
            Sprint = sprint;
            WorkingDays = workingDays;
            Id = id;
            TeamMember = teamMember;
        }

        public MemberWorkingDays() { }

        public static MemberWorkingDays Create(int id, int memberId, int sprintId, Sprint sprint, int workingDays)
        {
            return new MemberWorkingDays(memberId, sprintId, sprint, workingDays, id);
        }

        public static MemberWorkingDays Create(int memberId, int sprintId, Sprint sprint, int workingDays)
        {
            return new MemberWorkingDays(memberId, sprintId, sprint, workingDays);
        }
    }
}
