using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamRepository : IRepository<Team,int>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Team entity)
        {
            var obj = _dbContext.Team.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return obj.State == EntityState.Deleted ? true : false; ;
        }

        public async Task<IQueryable<Team>> GetAll()
        {
            return _dbContext.Team;
        }

        public async Task<Team> GetByIdAsync(int Id) => await _dbContext.Team.FindAsync(Id);       

        public async Task<bool> InsertAsync(Team entity)
        {
            var obj = await _dbContext.Team.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return obj.State == EntityState.Added ? true : false; ;
        }

        public async Task<bool> UpdateAsync(Team entity)
        {
            var team = await _dbContext.Team.FindAsync(entity.Id);
            team.TeamName = entity.TeamName;
            team.TeamOwner = entity.TeamOwner;
            var result = _dbContext.Entry(team).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}

