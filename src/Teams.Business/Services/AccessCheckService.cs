using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

namespace Teams.Business.Services
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

        public async Task<bool> OwnerOrMemberAsync(int teamId)
        {
             return await _teamRepository.GetAll().Include(x => x.TeamMembers)
                .Where(t => t.Id == teamId)
                .AnyAsync(y => y.TeamOwner == _currentUser.Current.Id() || y.TeamMembers.Any(z => z.MemberId == _currentUser.Current.Id()));
        }

        public async Task<bool> IsOwnerAsync(int teamId)
        {
            return await _teamRepository.GetAll()
               .Where(t => t.Id == teamId)
               .AnyAsync(y => y.TeamOwner == _currentUser.Current.Id());
        }
    }
}
