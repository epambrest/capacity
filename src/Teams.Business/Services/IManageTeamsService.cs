using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageTeamsService
    {
        Task<bool> AddTeamAsync(string teamName); 
        Task<TeamBusiness> GetTeamAsync(int teamId);
        Task<bool> EditTeamNameAsync(int teamId, string teamName);
        Task<IEnumerable<TeamBusiness>> GetMyTeamsAsync();
        Task<bool> RemoveAsync(int teamId);
    }
}
