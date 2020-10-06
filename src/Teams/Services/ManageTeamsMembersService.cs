using Teams.Repository;
using Teams.Models;
using Teams.Data;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Teams.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team, int> _teamRepository;
        private readonly IRepository<TeamMember, int> _teamMemberRepository;

        public ManageTeamsMembersService(IRepository<TeamMember, int> teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public ManageTeamsMembersService(IRepository<Team, int> teamRepository, IRepository<TeamMember, int> memberRepository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _teamMemberRepository = memberRepository;
        }

        public async Task<bool> AddAsync(int team_id, string member_id)
        {
            var alreadyInTeam = await _teamRepository.GetAll().
                AnyAsync(t => t.TeamOwner == _currentUser.Current.Id() && t.Id == team_id && t.TeamMembers.Any(t => t.MemberId == member_id));
            if (!alreadyInTeam && member_id != _currentUser.Current.Id())
            {
                return await _teamMemberRepository.InsertAsync(new TeamMember { TeamId = team_id, MemberId = member_id });
            }
            return false;
        }

        public async Task<TeamMember> GetMemberAsync(int team_id, string member_id)
        {
            return await _teamMemberRepository.GetAll()
            .Where(x => x.MemberId == member_id && x.TeamId == team_id)
            .FirstOrDefaultAsync();
        }
    }
}