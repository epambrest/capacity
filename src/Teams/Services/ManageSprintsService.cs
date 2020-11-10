using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Task = Teams.Models.Task;

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

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(int team_id, DisplayOptions options)
        {
            var sprints = _sprintRepository.GetAll().Include(x => x.Team).Include(x => x.Tasks).Where(x => x.TeamId == team_id);
            if (options.SortDirection == SortDirection.Ascending)
                return await sprints.OrderBy(x => x.Name).ToListAsync();
            else
                return await sprints.OrderByDescending(x => x.Name).ToListAsync();
        }

        public async Task<Team> GetTeam(int team_id)
        {
            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(i => i.Id == team_id);
            return team;
        }

        public async Task<Sprint> GetSprintAsync(int sprint_id, bool includeTaskAndTeamMember)
        {
            if (includeTaskAndTeamMember)
            {
                return await _sprintRepository.GetAll().Where(t => t.Id == sprint_id)
                    .Include(t => t.Tasks)
                    .ThenInclude(x => x.TeamMember.Member)
                    .FirstOrDefaultAsync(t => t.Id == sprint_id);
            }
            return await _sprintRepository.GetByIdAsync(sprint_id);
        }
    }
}
