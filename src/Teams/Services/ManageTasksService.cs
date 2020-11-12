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
            var task = await _taskRepository.GetAll().Include(t => t.TeamMember)
                .Include(t => t.TeamMember.Member).Where(t => t.Id == id).FirstOrDefaultAsync();

            return task;
        }
    }
}
