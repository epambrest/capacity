using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;

namespace Teams.Services
{
    public class ManageTasksService
    {
        private readonly IRepository<Models.Task, int> _repository;

        public ManageTasksService(IRepository<Models.Task, int> repository)
        {
            _repository = repository;
        }

        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            var task = await _repository.GetAll().Include(t => t.TeamMember)
                .Include(t => t.TeamMember.Member).Where(t => t.Id == id).FirstOrDefaultAsync();

            return task;
        }
    }
}