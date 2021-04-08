using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Models;

namespace Teams.Data.Repository
{
    public class TeamRepository : IRepository<TeamBusiness, int>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public TeamRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(TeamBusiness entity)
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

            var deletedTeam = await _dbContext.Team.FirstOrDefaultAsync(t => t.Id == entity.Id);
            _dbContext.Team.Remove(deletedTeam);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<IEnumerable<TeamBusiness>> GetAllAsync()
        {
            var allTeams = await _dbContext.Team.Include(m => m.TeamMembers).Include(t => t.Owner).ToListAsync();
            return _mapper.Map<List<Team>, IEnumerable<TeamBusiness>>(allTeams);
        }

        public async Task<TeamBusiness> GetByIdAsync(int Id)
        {
            var dataModel = await _dbContext.Team.FindAsync(Id);
            return _mapper.Map<TeamBusiness>(dataModel);
        }

        public async Task<bool> InsertAsync(TeamBusiness entity)
        {
            Team dataModel = _mapper.Map<Team>(entity);
            await _dbContext.Team.AddAsync(dataModel);
            var result = await _dbContext.SaveChangesAsync() > 0 ? true : false;
            return result;
        }

        public async Task<bool> UpdateAsync(TeamBusiness entity)
        {
            var team = await _dbContext.Team.FindAsync(entity.Id);
            if (team == null) return false;
            
            team.TeamName = entity.TeamName;
            team.TeamOwner = entity.TeamOwner;
            var result = _dbContext.Entry(team).State == EntityState.Modified ? true : false;
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}

