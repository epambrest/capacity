using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;

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
        public async Task<bool> RemoveAsync(int team_id, string member_id)
        {
            Team team = _teamRepository.GetByIdAsync(team_id).Result;
            if (team.TeamOwner == _currentUser.Current.Id() && MemberInTeam(team_id,member_id))
            {       
                return await _memberRepository.DeleteAsync(await _memberRepository.GetAll().FirstAsync(t => t.TeamId == team_id && t.MemberId == member_id));
            }
            return false;
        }
        private bool MemberInTeam(int team_id, string member_id)
        {
            return _memberRepository.GetAll().AnyAsync(t => t.TeamId == team_id && t.MemberId == member_id).Result;
        }
    }
}