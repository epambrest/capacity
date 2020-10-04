using System.Collections.Generic;
using System.Linq;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        IEnumerable<Team> GetMyTeams();
    }
}
