using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTasksService
    {
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
    }
}