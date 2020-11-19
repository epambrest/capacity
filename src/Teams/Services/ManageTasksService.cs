using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Task = Teams.Models.Task;
using Microsoft.EntityFrameworkCore;
using Teams.Security;
using System.Text.RegularExpressions;

namespace Teams.Services
{
    public class ManageTasksService : IManageTasksService
    {
        private readonly IRepository<Task, int> _taskRepository;
        private readonly ICurrentUser _currentUser;

        public ManageTasksService(IRepository<Task, int> taskRepository,ICurrentUser currentUser)
        {
            _taskRepository = taskRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options)
        {
            var tasks = _taskRepository.GetAll()
                .Where(x => x.TeamId == teamId)
                .Include(t => t.TeamMember.Member)
                .Include(t=>t.Team);
                
            if (options.SortDirection == SortDirection.Ascending)   
                return await tasks.OrderBy(x => x.Name).ToListAsync();
            else
                return await tasks.OrderByDescending(x => x.Name).ToListAsync();
        }

        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetAll().Include(t=>t.Team).Include(t => t.TeamMember)
                .Include(t => t.TeamMember.Member).Where(t => t.Id == id).FirstOrDefaultAsync();

            return task;
        }

        public async Task<bool> EditTaskAsync(Models.Task task)
        {
            var taskForCheck = await _taskRepository.GetByIdAsync(task.Id);
            bool nameCheck;
            if (taskForCheck.Name == task.Name) nameCheck = false;
            else nameCheck = _taskRepository.GetAll()
                .Where(x => x.TeamId == task.TeamId)
                .Any(x => x.Name == task.Name);
            if (nameCheck || task.StoryPoints <= 0 || !Regex.IsMatch(task.Link, (@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")) || !Regex.IsMatch(task.Name, ("^[a-zA-Z0-9-_.]+$")))
            {
                return false;
            }
            return await _taskRepository.UpdateAsync(task);
        }

        public async Task<bool> RemoveAsync(int taskId)
        {
            var task = await _taskRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == taskId 
                                          && x.Team.TeamOwner == _currentUser.Current.Id());
            if (task == null)
            {
                return false;
            }
            var result = await _taskRepository.DeleteAsync(task);
            return result;
        }
    }
}
