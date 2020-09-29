using Teams.Repository;
using Teams.Models;
using Teams.Data;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly ICurrentUser _currentUser;

        private IRepository<Team, int> _teamRepository;

        private IRepository<TeamMember, int> _teamMemberRepository;

        public ManageTeamsMembersService(ICurrentUser currentUser, IRepository<Team, int> teamRepository, IRepository<TeamMember, int> teamMemberRepository)
        {
            _currentUser = currentUser;

            _teamRepository = teamRepository;

            _teamMemberRepository = teamMemberRepository;
        }

        public  TeamMember GetMember(int team_id, string member_id)
        {
           return _teamMemberRepository.GetAll().Where(x => x.MemberId == member_id && x.TeamId == team_id && OwnerOrMemberOfTeam(team_id)).FirstOrDefault();
        }

        private bool OwnerOrMemberOfTeam(int team_id)
        {
           return _teamRepository.GetAll().Include(x => x.TeamMembers)
                .Where(t=>t.Id==team_id).Any(y=>y.TeamOwner==_currentUser.Current.Id() || y.TeamMembers.Any(z=>z.MemberId==_currentUser.Current.Id()));
        }
    }
}