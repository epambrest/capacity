using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Business.Services
{
    public interface IManageSprintsService
    {
        Task<IEnumerable<Sprint>> GetAllSprintsAsync(int teamId, DisplayOptions options);
        Task<Team> GetTeam(int teamId);
        Task<bool> AddSprintAsync(Sprint sprint);
        Task<bool> RemoveAsync(int sprintId);
        Task<bool> EditSprintAsync(Sprint sprint);
        Task<Sprint> GetSprintAsync(int sprintId, bool includeTaskAndTeamMember);
        Task<double> GetAverageStoryPointAsync(Sprint sprint);
    }
}
