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
        private readonly TeamRepository _teamRepository;
        private readonly TeamMemberRepository _teamMemberRepository;
        public ManageTeamsService(ICurrentUser currentUser, TeamRepository teamRepository, TeamMemberRepository teamMemberRepository)
        { 
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
        }
        public List<Team> GetMyTeams()
        {
            List<Team> myteams = new List<Team>();
            myteams.AddRange(_teamRepository.GetAll().Result.Where(team => team.TeamOwner == _currentUser.Current.Id()));
            myteams.AddRange(GetMyMemberTeams());
            return myteams;
        }
        public List<Team> GetMyMemberTeams()
        {
            List<Team> memberlist = new List<Team>();

            foreach (var id in _teamMemberRepository.GetAll().Result.Where(id => id.MemberId == _currentUser.Current.Id()))
            {
                foreach (var team in _teamRepository.GetAll().Result)
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
