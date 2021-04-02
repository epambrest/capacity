using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

namespace Teams.Business.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team, int> _teamRepository;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            if (await _teamRepository.GetAll().AnyAsync(t => t.TeamName.ToUpper() == teamName.ToUpper()) || !Regex.IsMatch(teamName, ("^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$")))
            {
                return false;
            }
            return await _teamRepository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
        }

        public async Task<IEnumerable<Team>> GetMyTeamsAsync() =>await _teamRepository.GetAll().Include(m => m.TeamMembers).Include(t => t.Owner)
                .Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
                .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id()).ToListAsync();

        public async Task<Team> GetTeamAsync(int teamId) => await _teamRepository.GetByIdAsync(teamId);

        public async Task<bool> EditTeamNameAsync(int teamId, string teamName)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);

            if (team != null && team.TeamOwner == _currentUser.Current.Id() && !_teamRepository.GetAll().Any(x => x.TeamName.ToUpper() == teamName.ToUpper()) && Regex.IsMatch(teamName, ("^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$")))
            {
                return await _teamRepository.UpdateAsync(new Team { Id = teamId, TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
            }
            else return false;
        }   
            
        public async Task<bool> RemoveAsync(int teamId)
        {
            var team = await _teamRepository.GetAll().FirstOrDefaultAsync(i => i.TeamOwner == _currentUser.Current.Id() && i.Id == teamId);
            if (team == null)
                return false;
            var result = await _teamRepository.DeleteAsync(team);
            return result;
        }
    }
}
