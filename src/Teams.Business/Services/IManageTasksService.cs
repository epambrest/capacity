using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Structures;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Models.Task task);
        Task<bool> AddTaskAsync(Models.Task task);
        Task<TasksAllParams> GetTasksAllParamsForMember(int teamMemberId, int sprintId);
    }
}