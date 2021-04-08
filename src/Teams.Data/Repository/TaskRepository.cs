using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;

namespace Teams.Data.Repository
{
    public class TaskRepository : IRepository<TaskBusiness, int>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public TaskRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(TaskBusiness entity)
        {
            var deletedTask = await _dbContext.Task.FirstOrDefaultAsync(t => t.Id == entity.Id);
            _dbContext.Task.Remove(deletedTask);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<TaskBusiness>> GetAllAsync()
        {
            var allTasks = await _dbContext.Task.Include(t => t.TeamMember.Member).Include(t => t.Team).Include(t => t.TeamMember).ToListAsync();
            return _mapper.Map<List<Models.Task>, IEnumerable<TaskBusiness>>(allTasks);
        }

        public async Task<TaskBusiness> GetByIdAsync(int id) 
        {
            var dataModel = await _dbContext.Task.FindAsync(id);
            return _mapper.Map<TaskBusiness>(dataModel);
        } 

        public async Task<bool> InsertAsync(TaskBusiness entity)
        {
            Models.Task dataModel = _mapper.Map<Models.Task>(entity);
            await _dbContext.Task.AddAsync(dataModel);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(TaskBusiness entity)
        {
            var task = await _dbContext.Task.FindAsync(entity.Id);
            if (task == null) return false;

            task.Name = entity.Name;
            task.Link = entity.Link;
            task.StoryPoints = entity.StoryPoints;
            task.MemberId = entity.MemberId;
            task.SprintId = entity.SprintId;
            var result = _dbContext.Entry(task).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}