using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        Task<bool> AddTeamAsync(string teamName); 
        public IEnumerable<Team> GetMyTeams();
        IEnumerable<Team> GetMyTeams();
        Task<Team> GetTeamAsync(int team_id);
    }
}
