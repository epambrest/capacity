using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Teams.Data.Models;

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
