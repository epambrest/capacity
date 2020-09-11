using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamMemberRepository:IRepository<TeamMember>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamMemberRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<TeamMember> DeleteAsync(TeamMember entity)
        {
            var obj=_dbContext.TeamMembers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }

        public IQueryable<TeamMember> GetAll()
        {
            return _dbContext.TeamMembers;
        }

        public IQueryable<TeamMember> GetById(TeamMember Id)//public async Task<TeamMember> GetById(int Id) => await _dbContext.TeamMember.FindAsync(Id);
        {
            throw new NotImplementedException();
        }

        

        public async Task<TeamMember> InsertAsync(TeamMember entity)
        {
            var obj=await _dbContext.TeamMembers.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }

        public async Task<TeamMember> UpdateAsync(TeamMember entity)
        {
            var member = await _dbContext.TeamMembers.FindAsync(entity.Id);
            member.MemberId = entity.MemberId;
            member.Team = entity.Team;
            await _dbContext.SaveChangesAsync();
            return member;
        }
    }
}
