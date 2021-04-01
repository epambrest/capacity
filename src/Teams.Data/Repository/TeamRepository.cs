using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class TeamRepository : IRepository<Team, int>
    {
        private readonly ApplicationDbContext _dbContext;

        public TeamRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Team entity)
        {
            var tasks = _dbContext.Task.Where(t => t.TeamId == entity.Id).ToList();
            
            foreach(var task in tasks)
            {
                _dbContext.Task.Remove(task);
            }
            
            var teamMembers = _dbContext.TeamMembers.Where(t => t.TeamId == entity.Id);

            foreach(var teamMember in teamMembers)
            {
                _dbContext.TeamMembers.Remove(teamMember);
            }

            _dbContext.Team.Remove(entity);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public IQueryable<Team> GetAll()
        {
            return _dbContext.Team;
        }

        public async Task<Team> GetByIdAsync(int Id) => await _dbContext.Team.FindAsync(Id);

        public async Task<bool> InsertAsync(Team entity)
        {
            await _dbContext.Team.AddAsync(entity);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<bool> UpdateAsync(Team entity)
        {
            var team = await _dbContext.Team.FindAsync(entity.Id);
            if (team == null)
                return false;
            team.TeamName = entity.TeamName;
            team.TeamOwner = entity.TeamOwner;
            var result = _dbContext.Entry(team).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}

