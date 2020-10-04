using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> RemoveAsync(int team_id, string member_id)
        {
            var team = await _teamRepository.GetAll().Where(t => t.Id == team_id).FirstOrDefaultAsync();
            if (team != null && team.TeamOwner == _currentUser.Current.Id() && MemberInTeam(team,member_id))
            {       
                return await _memberRepository.DeleteAsync(team.TeamMembers.FirstOrDefault(t => t.MemberId == member_id));
            }
            return false;
        }
        private bool MemberInTeam(Team team, string member_id)
        {
            return team.TeamMembers.Any(t => t.MemberId == member_id);
        }
    }
}