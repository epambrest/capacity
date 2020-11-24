using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<Data.Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Data.Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Data.Models.Task task);
    }
}