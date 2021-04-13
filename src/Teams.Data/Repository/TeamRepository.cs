using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Data.Repository
{
    public class TeamRepository : Repository<Models.Team, Business.Models.Team, int>
    {
        DbSet<Models.Team> _dbSet;

        public TeamRepository(ApplicationDbContext dbcontext, IMapper mapper) 
            : base(dbcontext, mapper)
        {
            _dbSet = dbcontext.Set<Models.Team>();
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            Models.Team deletedItem = await _dbSet.FindAsync(id);
            var tasks = await _dbContext.Task.Where(t => t.TeamId == id).ToListAsync();

            foreach (var task in tasks)
            {
                _dbContext.Task.Remove(task);
            }

            var teamMembers = await _dbContext.TeamMembers.Where(t => t.TeamId == id).ToListAsync();

            foreach (var teamMember in teamMembers)
            {
                _dbContext.TeamMembers.Remove(teamMember);
            }
            _dbSet.Remove(deletedItem);
            return await _dbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
