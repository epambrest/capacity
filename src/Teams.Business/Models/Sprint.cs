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

        private Sprint(int teamId, 
            string name = "", 
            int daysInSprint = 0, 
            int storyPointInHours = 0, 
            int status = 0,
            int id = 0,
            Team team = null,
            List<Task> tasks = null,
            List<MemberWorkingDays> memberWorkingDays = null)
        {
            TeamId = teamId;
            Name = name;
            DaysInSprint = daysInSprint;
            StoryPointInHours = storyPointInHours;
            Status = status;
            Id = id;
            Team = team;
            Tasks = tasks;
            MemberWorkingDays = memberWorkingDays;
        }

        public Sprint() 
        { 
        }

        public static Sprint Create(int teamId, string name, int daysInSprint, int storyPointInHours, int status) => 
            new Sprint(teamId, name, daysInSprint, storyPointInHours, status);

        public static Sprint Create(int id, 
            int teamId, 
            Team team, 
            string name, 
            int daysInSprint, 
            int storyPointInHours, 
            int status, 
            List<Task> tasks = null) => 
            new Sprint(teamId, name, daysInSprint, storyPointInHours, status, id, team, tasks);

        public static Sprint Create(int teamId, List<Task> tasks) => 
            new Sprint(teamId, "", 0, 0, 0, 0, null, tasks);

        public void SetStatus(int status) => Status = status;

    }
}
