using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTasksService
    {
        Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int team_id, DisplayOptions options);
    }
}
