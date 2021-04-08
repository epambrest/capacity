using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class SprintBusiness
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public TeamBusiness Team { get; set; }
        public string Name { get; set; }
        public int DaysInSprint { get; set; }
        public int StoryPointInHours { get; set; }
        public int Status { get; set; }
        public List<TaskBusiness> Tasks { get; set; } = new List<TaskBusiness>();
        public List<MemberWorkingDaysBusiness> MemberWorkingDays { get; set; } = new List<MemberWorkingDaysBusiness>();
    }
}
