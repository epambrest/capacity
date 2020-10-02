using System.Collections.Generic;
using System.Linq;
using Teams.Models;

   public interface IManageTeamsService
    {
        IEnumerable<Team> GetMyTeams();
    }

