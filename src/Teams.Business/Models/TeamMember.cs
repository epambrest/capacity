namespace Teams.Business.Models
{
    public class TeamMember 
    {
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public Team Team { get; set; }
        public string MemberId { get; set; }
        public User Member { get; set; }

        private TeamMember(string memberId, int teamId, User member = null, Team team = null, int id = 0)
        {
            TeamId = teamId;
            MemberId = memberId;
            Member = member;
            Team = team;
            Id = id;
        }

        public TeamMember() { }

        public static TeamMember Create(string memberId, User member)
        {
            return new TeamMember(memberId, 0, member);
        }

        public static TeamMember Create(int teamId, string memberId)
        {
            return new TeamMember(memberId, teamId);
        }
        public static TeamMember Create(int id, int teamId, string memberId, User member)
        {
            return new TeamMember(memberId, teamId, member, null, id);
        }

        public static TeamMember Create(int id, int teamId, Team team, string memberId, User member)
        {
            return new TeamMember(memberId, teamId, member, team, id);
        }
    }
}
