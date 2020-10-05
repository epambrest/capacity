using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

   public interface IManageTeamsService
    {
        Task<IEnumerable<Team>> GetMyTeamsAsync();
        Task<Team> GetTeamAsync(int team_id);
        Task<bool> EditTeamNameAsync(int team_id, string team_name);
    }

