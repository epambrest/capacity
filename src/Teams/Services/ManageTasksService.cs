using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Task = Teams.Models.Task;
using Microsoft.EntityFrameworkCore;

namespace Teams.Services
{
    public class ManageTasksService : IManageTasksService
    {
        private readonly IRepository<Task, int> _taskRepository;

        public ManageTasksService(IRepository<Task, int> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<Task>> GetMyTaskInSprintAsync(int sprint_id, DisplayOptions options)
        {
           
            var tasks = _taskRepository.GetAll().Where(x => x.SprintId == sprint_id);
            if (options.SortDirection == SortDirection.Ascending)
                return await tasks.OrderBy(x => x.Name).ToListAsync();
            else
                return await tasks.OrderByDescending(x => x.Name).ToListAsync();
        }

        public async Task<IEnumerable<Task>> GetMyTaskInTeamAsync(int team_id, DisplayOptions options)
        {
            await _taskRepository.InsertAsync(new Task { Name = "task1", SprintId = 1, TeamId = 16, StoryPoints = 1, Link = "wwwww" });
            var tasks = _taskRepository.GetAll().Where(x => x.TeamId == team_id);
            if (options.SortDirection == SortDirection.Ascending)
                return await tasks.OrderBy(x => x.Name).ToListAsync();
            else
                return await tasks.OrderByDescending(x => x.Name).ToListAsync();
        }
    }
}
