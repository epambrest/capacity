using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;

namespace Teams.Repository
{
    public class TaskRepository : IRepository<Models.Task, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Models.Task entity)
        {
            _dbContext.Task.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public IQueryable<Models.Task> GetAll() => _dbContext.Task;

        public async Task<Models.Task> GetByIdAsync(int id) => await _dbContext.Task.FindAsync(id);


        public async Task<bool> InsertAsync(Models.Task entity)
        {
            await _dbContext.Task.AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(Models.Task entity)
        {
            var task = await _dbContext.Task.FindAsync(entity.Id);
            if (task == null)
                return false;
            task.Name = entity.Name;
            task.Link = entity.Link;
            task.StoryPoints = entity.StoryPoints;
            task.MemberId = entity.MemberId;
            var result = _dbContext.Entry(task).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}