using System.Linq;
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
        public async Task<bool> Remove(int team_id, string member_id)
        {
            Team team = _teamRepository.GetByIdAsync(team_id).Result;
            TeamMember member;
            if (team.TeamOwner == _currentUser.Current.Id() && (member = MemberInTeam(team_id,member_id)) != null)
            {                
                return await _memberRepository.DeleteAsync(member);
            }
            return false;
        }
        private TeamMember MemberInTeam(int team_id, string member_id)
        {
            return _memberRepository.GetAll().First(t => t.TeamId == team_id && t.MemberId == member_id);
        }
    }
}