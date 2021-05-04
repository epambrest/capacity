namespace Teams.Business.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public string Name { get; set; }
        public int? SprintId { get; set; }
        public Sprint Sprint { get; set; }
        public int? MemberId { get; set; }
        public TeamMember TeamMember { get; set; }
        public string Link { get; set; }
        public int StoryPoints { get; set; }
        public bool Completed { get; set; }

        private Task(int teamId,
            int sprintId,
            Team team, 
            string name = "", 
            int storyPoints = 0, 
            string link = "", 
            int? memberId = null, 
            bool isCompleted = false,
            int id = 0,
            Sprint sprint = null,
            TeamMember teamMember = null)
        {
            TeamId = teamId;
            Team = team;
            Name = name;
            StoryPoints = storyPoints;
            Link = link;
            SprintId = sprintId;
            MemberId = memberId;
            Completed = isCompleted;
            Sprint = sprint;
            TeamMember = teamMember;
            Id = id;
        }

        public Task() 
        { 
        }

        public static Task Create(int id, 
            int teamId, 
            Team team, 
            string name, 
            int storyPoints,
            string link,
            int sprintId, 
            int? memberId,
            bool isCompleted = false) => 
            new Task(teamId, sprintId, team, name, storyPoints, link, memberId, isCompleted, id);
        

        public static Task Create(int teamId, int sprintId, Team team) => 
            new Task(teamId, sprintId, team);

        public void SetCompleted() => Completed = true;
    }
}
