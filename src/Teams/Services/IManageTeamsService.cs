using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        Task<bool> AddTeamAsync(string teamName); 
        IEnumerable<Team> GetMyTeams();
        Task<Team> GetTeamAsync(int team_id);
        Task<bool> EditTeamNameAsync(int team_id, string team_name);
        Task<IEnumerable<Team>> GetMyTeamsAsync();
        Task<bool> RemoveAsync(int team_id);
    }
}
