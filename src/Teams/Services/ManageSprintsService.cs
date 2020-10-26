using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;

namespace Teams.Services
{
    public class ManageSprintsService : IManageSprintsService
    {
        private readonly IRepository<Sprint, int> _sprintRepository;

        public ManageSprintsService(IRepository<Sprint, int> sprintRepository)
        {
            _sprintRepository = sprintRepository;
        }

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(int team_id, DisplayOptions options)
        {
            var sprints = _sprintRepository.GetAll().Include(x => x.Team).Where(x => x.TeamId == team_id);
            if (options.SortDirection == SortDirection.Ascending)
                return await sprints.OrderBy(x => x.Name).ToListAsync();
            else
                return await sprints.OrderByDescending(x => x.Name).ToListAsync();
        }
    }
}
