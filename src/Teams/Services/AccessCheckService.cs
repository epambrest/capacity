using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;

namespace Teams.Services
{
    public class AccessCheckService : IAccessCheckService
    {
        private readonly ICurrentUser _currentUser;

        private readonly IRepository<Team, int> _teamRepository;

        public AccessCheckService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;

            _teamRepository = teamRepository;
        }

        public async Task<bool> OwnerOrMemberAsync(int team_id)
        {
             return await _teamRepository.GetAll().Include(x => x.TeamMembers)
                .Where(t => t.Id == team_id)
                .AnyAsync(y => y.TeamOwner == _currentUser.Current.Id() || y.TeamMembers.Any(z => z.MemberId == _currentUser.Current.Id()));
        }
    }
}
