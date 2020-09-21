using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Repository;
using Teams.Security;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private IRepository<Team, int>  _teamRepository;
        private IRepository<TeamMember, int> _teamMemberRepository;
        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository, IRepository<TeamMember, int> teamMemberRepository)
        { 
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
        }
        public List<Team> GetMyTeams()
        {
            List<Team> myteams = new List<Team>();
            myteams.AddRange(_teamRepository.GetAll().ToList<Team>().Where(team => team.TeamOwner == _currentUser.Current.Id()));
            myteams.AddRange(GetMyMemberTeams());
            return myteams;
        }
        public List<Team> GetMyMemberTeams()
        {
            List<Team> memberlist = new List<Team>();

            foreach (var id in _teamMemberRepository.GetAll().ToArray<TeamMember>().Where(id => id.MemberId == _currentUser.Current.Id()))
            {
                foreach (var team in _teamRepository.GetAll().ToList<Team>())
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