using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class TeamMemberRepository : IRepository<TeamMemberBusiness, int>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public TeamMemberRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(TeamMemberBusiness entity)
        {
            var deletedTeamMember = await _dbContext.TeamMembers.FirstOrDefaultAsync(t => t.Id == entity.Id);
            _dbContext.TeamMembers.Remove(deletedTeamMember);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<IEnumerable<TeamMemberBusiness>> GetAllAsync() /*=> await _mapper.ProjectTo<TeamMemberBusiness>(_dbContext.TeamMembers).ToListAsync();*/
        {
            var allTeamMembers = await _dbContext.TeamMembers.Include(x => x.Member).Include(x => x.Team)
                .ThenInclude(x => x.Owner).ToListAsync();
            return _mapper.Map<List<TeamMember>, IEnumerable<TeamMemberBusiness>>(allTeamMembers);
        }
        public async Task<TeamMemberBusiness> GetByIdAsync(int Id)
        {
            var dataModel = await _dbContext.TeamMembers.FindAsync(Id);
            return _mapper.Map<TeamMemberBusiness>(dataModel);
        }

        public async Task<bool> InsertAsync(TeamMemberBusiness entity)
        {
            TeamMember dataModel = _mapper.Map<TeamMember>(entity);
            await _dbContext.TeamMembers.AddAsync(dataModel);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<bool> UpdateAsync(TeamMemberBusiness entity)
        {
            var member = await _dbContext.TeamMembers.FindAsync(entity.Id);  
            if (member == null) return false;
            
            member.MemberId = entity.MemberId;
            member.TeamId = entity.TeamId;
            var result = _dbContext.Entry(member).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
