using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class Sprint
    {
        public int Id { get; set; }
        public virtual Team Team { get; set; }
        public string Name { get; set; }
        public int DaysInSprint { get; set; }
        public int StoryPointInHours { get; set; }
        public int Status { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<MemberWorkingDays> MemberWorkingDays { get; set; }
    }
}
