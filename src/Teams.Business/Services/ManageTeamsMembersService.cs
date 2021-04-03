using Microsoft.EntityFrameworkCore;
/*using Microsoft.AspNetCore.Mvc;*/
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Data.Annotations;
using Teams.Data.Models;
using Teams.Security;

namespace Teams.Business.Services
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

        public async Task<bool> RemoveAsync(int teamId, string memberId)
        {
            var member = await _teamMemberRepository.GetAll()
            .Where(x => x.MemberId == memberId && x.TeamId == teamId
            && x.Team.TeamOwner == _currentUser.Current.Id()
            && x.Team.TeamOwner != memberId)
            .FirstOrDefaultAsync();
            if (member != null)
            {
                return await _teamMemberRepository.DeleteAsync(member);
            }
            return false;
        }

        public async Task<bool> AddAsync(int teamId, string memberId)
        {
            var alreadyInTeam = await _teamRepository.GetAll().
                AnyAsync(t => t.TeamOwner == _currentUser.Current.Id() && t.Id == teamId && t.TeamMembers.Any(t => t.MemberId == memberId));
            if (!alreadyInTeam && memberId != _currentUser.Current.Id())
            {
                return await _teamMemberRepository.InsertAsync(new TeamMember { TeamId = teamId, MemberId = memberId });
            }
            return false;
        }

        public async Task<TeamMember> GetMemberAsync(int teamId, string memberId)
        {
             return await _teamMemberRepository.GetAll()
                    .Where(x => x.MemberId == memberId && x.TeamId == teamId)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            var members = _teamMemberRepository.GetAll().Include(x => x.Member).Include(x => x.Team).ThenInclude(x => x.Owner).Where(x => x.TeamId == teamId);

            if (options.SortDirection == SortDirection.Ascending) return await members.OrderBy(x => x.Member.UserName).ToListAsync();

            else return await members.OrderByDescending(x => x.Member.UserName).ToListAsync();
        }
    }
}