using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Annotations;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<Business.Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Business.Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Business.Models.Task task);
        Task<bool> AddTaskAsync(Business.Models.Task task);
    }
}