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
        Task<Sprint> GetSprintAsync(int sprintId);
        Task<bool> AddSprintAsync(Sprint sprint);
        Task<bool> RemoveAsync(int sprintId);
        Task<bool> EditSprintAsync(Sprint sprint);

    }
}
