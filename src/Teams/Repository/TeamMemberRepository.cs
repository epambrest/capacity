using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamMemberRepository : IRepository<TeamMember,int>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamMemberRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(TeamMember entity)
        {
            var obj = _dbContext.TeamMembers.Remove(entity);
            var result=obj.State == EntityState.Deleted ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public IQueryable<TeamMember> GetAll()
        {
            return _dbContext.TeamMembers;
        }

        public async Task<TeamMember> GetByIdAsync(int Id) => await _dbContext.TeamMembers.FindAsync(Id);    

        public async Task<bool> InsertAsync(TeamMember entity)
        {
            var obj = await _dbContext.TeamMembers.AddAsync(entity);
            var result = obj.State == EntityState.Added ? true : false;
            await _dbContext.SaveChangesAsync();
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
