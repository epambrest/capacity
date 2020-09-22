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
        private readonly IRepository<Team, int> _teamRepository;
        private readonly IRepository<TeamMember, int> _memberRepository;

        public ManageTeamsMembersService(IRepository<Team, int> teamRepository, IRepository<TeamMember, int> memberRepository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _memberRepository = memberRepository;
        }

        [Authorize]
        public void Add(int team_id, string member_id)
        {
            Team team = _teamRepository.GetByIdAsync(team_id).Result;
            if (team.TeamOwner == _currentUser.Current.Id() && !AlreadyInTeam(team_id, member_id))
            {
                TeamMember member = new TeamMember { TeamId = team_id, MemberId = member_id };
                _memberRepository.InsertAsync(member);
            }
        }

        private bool AlreadyInTeam(int team_id, string member_id)
        {
            return _memberRepository.GetAll().Any(t => t.TeamId == team_id && t.MemberId == member_id);
        }
    }
}