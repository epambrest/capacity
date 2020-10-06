using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team,int> _teamRepository;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            if (await _teamRepository.GetAll().AnyAsync(t => t.TeamName.ToUpper() == teamName.ToUpper()) || !Regex.IsMatch(teamName, ("^[a-zA-Z0-9-_.]+$")))
            {
                return false;
            }
            var result = await _teamRepository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
            return await _teamRepository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
        }

        public IEnumerable<Team> GetMyTeams() => _teamRepository.GetAll().Include(m => m.TeamMembers);
        public async Task<IEnumerable<Team>> GetMyTeamsAsync() =>await _teamRepository.GetAll().Include(m => m.TeamMembers)
                .Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
                .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id()).ToListAsync();

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
