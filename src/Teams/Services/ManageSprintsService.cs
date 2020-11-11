using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;

namespace Teams.Services
{
    public class ManageSprintsService : IManageSprintsService
    {
        private readonly IRepository<Sprint, int> _sprintRepository;
        private readonly IManageTeamsService _manageTeamsService;

        public ManageSprintsService(IRepository<Sprint, int> sprintRepository, IManageTeamsService manageTeamsService)
        {
            _sprintRepository = sprintRepository;
            _manageTeamsService = manageTeamsService;
        }

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(int teamId, DisplayOptions options)
        {
            var sprints = _sprintRepository.GetAll().Include(x => x.Team).Where(x => x.TeamId == teamId);
            if (options.SortDirection == SortDirection.Ascending)
                return await sprints.OrderBy(x => x.Name).ToListAsync();
            else
                return await sprints.OrderByDescending(x => x.Name).ToListAsync();
        }

        public async Task<Team> GetTeam(int teamId)
        {
            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(i => i.Id == teamId);
            return team;
        }

        public async Task<Sprint> GetSprintAsync(int sprintId) => await _sprintRepository.GetByIdAsync(sprintId);

        public async Task<bool> AddSprintAsync(Sprint sprint)
        {
            if (_sprintRepository.GetAll()
                .Where(x=>x.TeamId == sprint.TeamId)
                .Any(x=>x.Name == sprint.Name) 
                || sprint.DaysInSprint<=0 || sprint.StorePointInHours<=0 || !Regex.IsMatch(sprint.Name, ("^[a-zA-Z0-9-_.]+$")))
            {
                return false;
            }
            return await _sprintRepository.InsertAsync(sprint);
        }

        public async Task<bool> EditSprintAsync(Sprint sprint)
        {
            var sprintForCheck = await _sprintRepository.GetByIdAsync(sprint.Id);
            bool nameCheck;
            if (sprintForCheck.Name == sprint.Name) nameCheck = false;
            else nameCheck = _sprintRepository.GetAll()
                .Where(x => x.TeamId == sprint.TeamId)
                .Any(x => x.Name == sprint.Name);
            if (nameCheck || sprint.DaysInSprint <= 0 || sprint.StorePointInHours <= 0 || !Regex.IsMatch(sprint.Name, ("^[a-zA-Z0-9-_.]+$")))
            {
                return false;
            }
            return await _sprintRepository.UpdateAsync(sprint);
        }
    }
}
