using Teams.Models;
using Teams.Data;
using System.Linq;
using Teams.Services;
using Teams.Security;
using Microsoft.AspNetCore.Authorization;

namespace Teams.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly ICurrentUser _currentUser;
        private readonly ApplicationDbContext db;
        public ManageTeamsMembersService(ApplicationDbContext context, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            db = context;
        }

        [Authorize]
        public void Add(int team_id, string member_id)
        {
            Team team = db.Team.First(t => t.Id == team_id);
            if (team.TeamOwner == _currentUser.Current.Id() && !alreadyInTeam(team_id, member_id))
            {
                TeamMember member = new TeamMember { TeamId = team_id, MemberId = member_id };
                db.TeamMembers.Add(member);
                db.SaveChanges();
            }
        }

        private bool alreadyInTeam(int team_id, string member_id)
        {
            return db.TeamMembers.Any(t => t.TeamId == team_id && t.MemberId == member_id);
        }
    }
}
