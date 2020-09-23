using System.Collections.Generic;
using System.Linq;
using Teams.Models;

   public interface IManageTeamsService
    {
        public IEnumerable<Team> GetMyTeams();
    }

