using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class TeamMemberRepository : IRepository<TeamMember, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamMemberRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(TeamMember entity)
        {
            _dbContext.TeamMembers.Remove(entity);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public IQueryable<TeamMember> GetAll()
        {
            return _dbContext.TeamMembers;
        }

        public async Task<TeamMember> GetByIdAsync(int Id) => await _dbContext.TeamMembers.FindAsync(Id);

        public async Task<bool> InsertAsync(TeamMember entity)
        {
            await _dbContext.TeamMembers.AddAsync(entity);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<bool> UpdateAsync(TeamMember entity)
        {
            var member = await _dbContext.TeamMembers.FindAsync(entity.Id);
            if (member == null)
                return false;
            member.MemberId = entity.MemberId;
            member.TeamId = entity.TeamId;
            var result = _dbContext.Entry(member).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}
