using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;

        private IRepository<Team, int> _teamRepository;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;

            _teamRepository = teamRepository;
        }

        public async Task<IEnumerable<Team>> GetMyTeamsAsync() =>await _teamRepository.GetAll().Include(m => m.TeamMembers)
                .Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
                .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id())
                .ToListAsync();

        public async Task<Team> GetTeamAsync(int team_id) => await _teamRepository.GetByIdAsync(team_id);

        public async Task<bool> EditTeamNameAsync(int team_id, string team_name)
        {
            var team = await _teamRepository.GetByIdAsync(team_id);

            if (team != null && team.TeamOwner == _currentUser.Current.Id() && !_teamRepository.GetAll().Any(x => x.TeamName.ToUpper() == team_name.ToUpper()) && Regex.IsMatch(team_name, ("^[a-zA-Z0-9-_.]+$")))
            {
                return await _teamRepository.UpdateAsync(new Team { Id = team_id, TeamOwner = _currentUser.Current.Id(), TeamName = team_name });
            }
            else return false;
        }
    }
}
