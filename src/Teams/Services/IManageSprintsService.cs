using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageSprintsService
    {
        Task<IEnumerable<Sprint>> GetAllSprintsAsync(int teamId, DisplayOptions options);
        Task<Team> GetTeam(int teamId);
        Task<Sprint> GetSprintAsync(int sprintId, bool includeTaskAndTeamMember);
    }
}
