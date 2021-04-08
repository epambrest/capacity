using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class SprintRepository : IRepository<SprintBusiness, int>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public SprintRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(SprintBusiness entity)
        {
            var deletedSprint = await _dbContext.Sprint.FirstOrDefaultAsync(t => t.Id == entity.Id);
            _dbContext.Sprint.Remove(deletedSprint);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<IEnumerable<SprintBusiness>> GetAllAsync()
        {
            var allSprints = await _dbContext.Sprint.Include(t => t.Tasks).ThenInclude(x => x.TeamMember.Member)
                    .Include(x => x.Team).ToListAsync();
            return _mapper.Map<List<Sprint>, IEnumerable<SprintBusiness>>(allSprints);
        }

        public async Task<SprintBusiness> GetByIdAsync(int id)
        {
            var dataModel = await _dbContext.Sprint.FindAsync(id);
            return _mapper.Map<SprintBusiness>(dataModel);
        }
         
        public async Task<bool> InsertAsync(SprintBusiness entity)
        {
            Sprint dataModel = _mapper.Map<Sprint>(entity);
            await _dbContext.Sprint.AddAsync(dataModel);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> UpdateAsync(SprintBusiness entity)
        {
            var sprint = await _dbContext.Sprint.FindAsync(entity.Id);
            if (sprint == null) return false;
            
            sprint.Name = entity.Name;
            sprint.StoryPointInHours = entity.StoryPointInHours;
            sprint.DaysInSprint = entity.DaysInSprint;
            sprint.Status = entity.Status;
            var result = _dbContext.Entry(sprint).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
