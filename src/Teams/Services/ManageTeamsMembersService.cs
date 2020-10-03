using Teams.Models;
using Teams.Data;
using System.Linq;
using Teams.Services;
using Teams.Security;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Teams.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team, int> _teamRepository;
        private readonly IRepository<TeamMember, int> _teamMemberRepository;

        public ManageTeamsMembersService(IRepository<Team, int> teamRepository, IRepository<TeamMember, int> memberRepository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _teamMemberRepository = memberRepository;
        }

        public async Task<bool> AddAsync(int team_id, string member_id)
        {
            var team = await _teamRepository.GetAll().Where(t => t.Id == team_id).Include(t => t.TeamMembers).FirstOrDefaultAsync();
            if (team != null && team.TeamOwner == _currentUser.Current.Id()
                && member_id != _currentUser.Current.Id() && !AlreadyInTeam(team, member_id))
            {
                return await _teamMemberRepository.InsertAsync(new TeamMember { TeamId = team_id, MemberId = member_id }); 
            }
            return false;
        }

        private bool AlreadyInTeam(Team team, string member_id)
        {
            return team.TeamMembers.Any(t => t.MemberId == member_id);
        }
    }
}