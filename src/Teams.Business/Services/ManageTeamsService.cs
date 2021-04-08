using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Security;

namespace Teams.Business.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<TeamBusiness, int> _teamRepository;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<TeamBusiness, int> teamRepository)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            var allTeams = await _teamRepository.GetAllAsync();
            if (allTeams.Any(t => t.TeamName.ToUpper() == teamName.ToUpper()) || !Regex.IsMatch(teamName, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
                return false;
            return await _teamRepository.InsertAsync(new TeamBusiness { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
        }

        public async Task<IEnumerable<TeamBusiness>> GetMyTeamsAsync()
        {
            var allTeams = await _teamRepository.GetAllAsync();
            var ownerTeams = allTeams.Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
               .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id());
            return ownerTeams;
        }
        public async Task<TeamBusiness> GetTeamAsync(int teamId) => await _teamRepository.GetByIdAsync(teamId);
        public async Task<bool> EditTeamNameAsync(int teamId, string teamName)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            var allTeams = await _teamRepository.GetAllAsync();
            bool isThisTeam = allTeams.Any(x => x.TeamName.ToUpper() == teamName.ToUpper());
            if (team != null && team.TeamOwner == _currentUser.Current.Id() && !isThisTeam && Regex.IsMatch(teamName, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
                return await _teamRepository.UpdateAsync(new TeamBusiness { Id = teamId, TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
            else return false;
        }   
            
        public async Task<bool> RemoveAsync(int teamId)
        {
            var allTeams = await _teamRepository.GetAllAsync();
            var team = allTeams.FirstOrDefault(i => i.TeamOwner == _currentUser.Current.Id() && i.Id == teamId);
            if (team == null) return false;
            
            var result = await _teamRepository.DeleteAsync(team);
            return result;
        }
    }
}
