using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageSprintsService
    {
        Task<IEnumerable<SprintBusiness>> GetAllSprintsAsync(int teamId, DisplayOptions options);
        Task<TeamBusiness> GetTeam(int teamId);
        Task<bool> AddSprintAsync(SprintBusiness sprint);
        Task<bool> RemoveAsync(int sprintId);
        Task<bool> EditSprintAsync(SprintBusiness sprint);
        Task<SprintBusiness> GetSprintAsync(int sprintId, bool includeTaskAndTeamMember);
        Task<double> GetAverageStoryPointAsync(SprintBusiness sprint);
    }
}
