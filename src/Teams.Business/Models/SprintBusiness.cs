using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class SprintBusiness
    {
        public int Id { get; set; }
        public virtual TeamBusiness Team { get; set; }
        public string Name { get; set; }
        public int DaysInSprint { get; set; }
        public int StoryPointInHours { get; set; }
        public int Status { get; set; }
        public virtual ICollection<TaskBusiness> Tasks { get; set; }
        public virtual ICollection<MemberWorkingDaysBusiness> MemberWorkingDays { get; set; }
    }
}
