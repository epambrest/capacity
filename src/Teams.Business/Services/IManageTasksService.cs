using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<TaskBusiness> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskBusiness>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(TaskBusiness task);
        Task<bool> AddTaskAsync(TaskBusiness task);
    }
}