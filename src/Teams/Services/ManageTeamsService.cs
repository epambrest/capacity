using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private readonly ApplicationDbContext db;
        public ManageTeamsService(ICurrentUser currentUser, ApplicationDbContext context)
        {
            _currentUser = currentUser;
            db = context;
        }
        public List<Team> GetMyTeams()
        {
            List<Team> myteams = new List<Team>();
            myteams.AddRange(db.Team.ToList().Where(team => team.TeamOwner == _currentUser.Current.Id()));
            myteams.AddRange(GetMemberTeams());
            return myteams;
        }
        public List<Team> GetMemberTeams()
        {
            List<Team> memberlist = new List<Team>();

            foreach (var id in db.TeamMembers.ToArray().Where(id => id.MemberId == _currentUser.Current.Id()))
            {
                foreach (var team in db.Team.ToList())
                {
                    if (id.TeamId == team.Id)
                    {
                        memberlist.Add(team);
                    }
                }
            }
            return memberlist;
        }
    }
}
