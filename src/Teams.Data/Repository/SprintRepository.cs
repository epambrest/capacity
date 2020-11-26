
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class SprintRepository : IRepository<Sprint, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public SprintRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Sprint entity)
        {
            _dbContext.Sprint.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public IQueryable<Sprint> GetAll() => _dbContext.Sprint;

        public async Task<Sprint> GetByIdAsync(int id) => await _dbContext.Sprint.FindAsync(id);
         
        public async Task<bool> InsertAsync(Sprint entity)
        {
            await _dbContext.Sprint.AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(Sprint entity)
        {
            var sprint = await _dbContext.Sprint.FindAsync(entity.Id);
            if (sprint == null)
                return false;
            sprint.Name = entity.Name;
            sprint.StoryPointInHours = entity.StoryPointInHours;
            sprint.DaysInSprint = entity.DaysInSprint;
            sprint.IsActive = entity.IsActive;
            var result = _dbContext.Entry(sprint).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
