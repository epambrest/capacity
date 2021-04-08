using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class MemberWorkingDaysRepository : IRepository<MemberWorkingDaysBusiness, int>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public MemberWorkingDaysRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(MemberWorkingDaysBusiness entity)
        {
            var deletedMemberWorkingDays = await _dbContext.MemberWorkingDays.FirstOrDefaultAsync(t => t.Id == entity.Id);
            _dbContext.MemberWorkingDays.Remove(deletedMemberWorkingDays);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<MemberWorkingDaysBusiness>> GetAllAsync() /*=> await _mapper.ProjectTo<MemberWorkingDaysBusiness>(_dbContext.MemberWorkingDays).ToListAsync();*/
        {
            var allWorkingDays = await _dbContext.MemberWorkingDays.Include(x => x.Sprint)
                .Include(x => x.TeamMember).ToListAsync();
            return _mapper.Map<List<MemberWorkingDays>, IEnumerable<MemberWorkingDaysBusiness>>(allWorkingDays);
        }

        public async Task<MemberWorkingDaysBusiness> GetByIdAsync(int id)
        {
            var dataModel = await _dbContext.MemberWorkingDays.FindAsync(id);
            return _mapper.Map<MemberWorkingDaysBusiness>(dataModel);
        }
        public async Task<bool> InsertAsync(MemberWorkingDaysBusiness entity)
        {
            MemberWorkingDays memberWorkingDaysEntity = _mapper.Map<MemberWorkingDays>(entity);
            await _dbContext.MemberWorkingDays.AddAsync(memberWorkingDaysEntity);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(MemberWorkingDaysBusiness entity)
        {
            var workingDays = await _dbContext.MemberWorkingDays.FindAsync(entity.Id);
            if (workingDays == null) return false;
            
            workingDays.WorkingDays = entity.WorkingDays;
            var result = _dbContext.Entry(workingDays).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
