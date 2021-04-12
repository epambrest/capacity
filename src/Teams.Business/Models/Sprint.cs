using System.Collections.Generic;

namespace Teams.Business.Models
{
    public class Sprint
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string Name { get; set; }
        public int DaysInSprint { get; set; }
        public int StoryPointInHours { get; set; }
        public int Status { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        public List<MemberWorkingDays> MemberWorkingDays { get; set; } = new List<MemberWorkingDays>();
    }
}
