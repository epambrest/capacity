using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Annotations;

namespace Teams.Business.Services
{
    public interface IManageTasksService
    {
        Task<Models.Task> GetTaskByIdAsync(int id);
        Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options);
        Task<bool> RemoveAsync(int taskId);
        Task<bool> EditTaskAsync(Models.Task task);
        Task<bool> AddTaskAsync(Models.Task task);
        Task<Dictionary<OtherNamesTaskParams, double>> GetTasksAllParamsForMember(int teamMemberId, int sprintId);
    }
}