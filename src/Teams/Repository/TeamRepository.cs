using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamRepository:IRepository<Team>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Team> DeleteAsync(Team entity)
        {
            _dbContext.Team.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public IQueryable<Team> GetAll()
        {
            return _dbContext.Team;
        }

        public IQueryable<Team> GetById(Team Id) //public async Task<Team> GetById(int Id) => await _dbContext.Team.FindAsync(Id);
        {
            throw new NotImplementedException();
        }

       

        public async Task<Team> InsertAsync(Team entity)
        {
            var obj = await _dbContext.Team.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }

        public async Task<Team> UpdateAsync(Team entity)
        {
            var team = await _dbContext.Team.FindAsync(entity.Id);
            team.TeamMembers = entity.TeamMembers;
            team.TeamName = entity.TeamName;
            team.TeamOwner = entity.TeamOwner;
            await _dbContext.SaveChangesAsync();
            return team;
        }
    }
}

