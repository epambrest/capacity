using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public IEnumerable<Team> GetMyTeams() => _teamRepository.GetAll().Include(m => m.TeamMembers)
                .Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
                .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id());
    }
}
