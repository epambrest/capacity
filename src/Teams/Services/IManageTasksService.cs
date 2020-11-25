using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;
using Task = Teams.Models.Task;

namespace Teams.Services
{
    public interface IManageTasksService
    {
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Models.Task task);
        Task<bool> AddTaskAsync(Task task);
    }
}