using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Data.Repository;
using Teams.Security;


namespace Teams.Business.Services
{
    public class ManageTasksService : IManageTasksService
    {
        private readonly IRepository<Data.Models.Task, Business.Models.Task, int> _taskRepository;
        private readonly ICurrentUser _currentUser;

        public ManageTasksService(IRepository<Data.Models.Task, Business.Models.Task, int> taskRepository, ICurrentUser currentUser)
        {
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Business.Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            var allTasksForTean = allTasks.Where(x => x.TeamId == teamId);

            if (options.SortDirection == SortDirection.Ascending)
                return allTasksForTean.OrderBy(x => x.Name);
            else
                return allTasksForTean.OrderByDescending(x => x.Name);
        }

        public async System.Threading.Tasks.Task<Business.Models.Task> GetTaskByIdAsync(int id)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            //
            var task = allTasks.Where(t => t.Id == id).FirstOrDefault();
            return task;
        }

        public async Task<bool> EditTaskAsync(Business.Models.Task task)
        {
            var taskForCheck = await _taskRepository.GetByIdAsync(task.Id);
            bool nameCheck;
            
            if (taskForCheck.Name == task.Name)
            {
                nameCheck = false;
            }
            else
            {
                var allTasks = await _taskRepository.GetAllAsync();
                nameCheck = allTasks.Where(x => x.TeamId == task.TeamId).Any(x => x.Name == task.Name);
            }

            if (nameCheck || task.StoryPoints <= 0 || !Regex.IsMatch(task.Link, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$") 
                || !Regex.IsMatch(task.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }
            return await _taskRepository.UpdateAsync(task);
        }

        public async Task<bool> RemoveAsync(int taskId)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            var task = allTasks.FirstOrDefault(x => x.Id == taskId && x.Team.TeamOwner == _currentUser.Current.Id());
            
            if (task == null) return false;
            
            var result = await _taskRepository.DeleteAsync(taskId);
            return result;
        }

        public async Task<bool> AddTaskAsync(Business.Models.Task task)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            if (allTasks.Where(t => t.SprintId == task.SprintId).Any(t => t.Name == task.Name || t.Link == task.Link) || task.StoryPoints <= 0
                || !Regex.IsMatch(task.Link, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")
                || !Regex.IsMatch(task.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }
            return await _taskRepository.InsertAsync(task);
        }
    }
}
