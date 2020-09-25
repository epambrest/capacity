using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Repository;
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

        public async Task<bool> Remove(int team_id)
        {
            var team = _teamRepository.GetAll().FirstOrDefault(i => i.TeamOwner == _currentUser.Current.Id() && i.Id == team_id);
            if (team == null)
                return false;
            
            var result = await _teamRepository.DeleteAsync(team);
            return result;
        }

    }
}
