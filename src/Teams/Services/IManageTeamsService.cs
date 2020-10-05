using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

   public interface IManageTeamsService
    {
        IEnumerable<Team> GetMyTeams();
        Task<Team> GetTeamAsync(int team_id);
    }

