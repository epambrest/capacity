using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class MemberWorkingDaysRepository : IRepository<MemberWorkingDays, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public MemberWorkingDaysRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(MemberWorkingDays entity)
        {
            _dbContext.MemberWorkingDays.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public IQueryable<MemberWorkingDays> GetAll() => _dbContext.MemberWorkingDays;

        public async Task<MemberWorkingDays> GetByIdAsync(int id) => await _dbContext.MemberWorkingDays.FindAsync(id);

        public async Task<bool> InsertAsync(MemberWorkingDays entity)
        {
            await _dbContext.MemberWorkingDays.AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(MemberWorkingDays entity)
        {
            var workingDays = await _dbContext.MemberWorkingDays.FindAsync(entity.Id);
            if (workingDays == null)
                return false;
            workingDays.WorkingDays = entity.WorkingDays;
            var result = _dbContext.Entry(workingDays).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
