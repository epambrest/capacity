using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageTeamsService
    {
        Task<bool> AddTeamAsync(string teamName); 
        Task<Team> GetTeamAsync(int teamId);
        Task<bool> EditTeamNameAsync(int teamId, string teamName);
        Task<IEnumerable<Team>> GetMyTeamsAsync();
        Task<bool> RemoveAsync(int teamId);
    }
}
