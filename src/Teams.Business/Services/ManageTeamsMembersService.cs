using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Data.Repository;
using Teams.Security;

namespace Teams.Business.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Data.Models.Team, Business.Models.Team, int> _teamRepository;
        private readonly IRepository<Data.Models.TeamMember, Business.Models.TeamMember, int> _teamMemberRepository;

        public ManageTeamsMembersService(IRepository<Data.Models.Team, Business.Models.Team, int> teamRepository, 
            IRepository<Data.Models.TeamMember, Business.Models.TeamMember, int> memberRepository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _teamMemberRepository = memberRepository;
        }

        public async Task<bool> RemoveAsync(int teamId, string memberId)
        {
            var allMembes = await _teamMemberRepository.GetAllAsync();
            var member = allMembes.Where(x => x.MemberId == memberId && x.TeamId == teamId 
                && x.Team.TeamOwner == _currentUser.Current.Id() && x.Team.TeamOwner != memberId).FirstOrDefault();
            if (member != null) return await _teamMemberRepository.DeleteAsync(member.Id);
            return false;
        }

        public async Task<bool> AddAsync(int teamId, string memberId)
        {
            var allTeams = await _teamRepository.GetAllAsync();
            var alreadyInTeam = allTeams.Any(t => t.TeamOwner == _currentUser.Current.Id() && t.Id == teamId 
                && t.TeamMembers.Any(t => t.MemberId == memberId));
            if (!alreadyInTeam && memberId != _currentUser.Current.Id())
                return await _teamMemberRepository.InsertAsync(new Business.Models.TeamMember { TeamId = teamId, MemberId = memberId });
            return false;
        }

        public async Task<Business.Models.TeamMember> GetMemberAsync(int teamId, string memberId)
        {
            var allMembers = await _teamMemberRepository.GetAllAsync();
            var member = allMembers.Where(x => x.MemberId == memberId && x.TeamId == teamId).FirstOrDefault();
            return member;
        }

        public async Task<List<Business.Models.TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            var allMembers = await _teamMemberRepository.GetAllAsync();
            var teamMembers = allMembers.Where(x => x.TeamId == teamId);

            if (options.SortDirection == SortDirection.Ascending) return teamMembers.OrderBy(x => x.Member.UserName).ToList();
            else return teamMembers.OrderByDescending(x => x.Member.UserName).ToList();
        }
    }
}