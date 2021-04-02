using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Data.Annotations;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<Data.Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Data.Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Data.Models.Task task);
        Task<bool> AddTaskAsync(Data.Models.Task task);
    }
}