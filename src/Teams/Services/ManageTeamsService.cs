using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly IRepository<Team, int> _teamRepository;
        private readonly ICurrentUser _currentUser;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _teamRepository = teamRepository;
            _currentUser = currentUser;
        }

        public async Task<bool> RemoveAsync(int team_id)
        {
            var team = await _teamRepository.GetAll().FirstOrDefaultAsync(i => i.TeamOwner == _currentUser.Current.Id() && i.Id == team_id);
            if (team == null)
                return false;
            
            var result = await _teamRepository.DeleteAsync(team);
            return result;
        }

    }
}
