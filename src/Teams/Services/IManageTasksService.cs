using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTasksService
    {
        Task<Models.Task> GetTaskByIdAsync(int id);
    }
}