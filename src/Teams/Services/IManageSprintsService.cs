using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageSprintsService
    {
        Task<IEnumerable<Sprint>> GetAllSprintsAsync(int team_id, DisplayOptions options);
        Task<Team> GetTeam(int team_id);
        Task<Sprint> GetSprintAsync(int sprint_id);
        Task<bool> AddSprintAsync(Sprint sprint);
        Task<bool> EditSprintAsync(Sprint sprint);
    }
}
