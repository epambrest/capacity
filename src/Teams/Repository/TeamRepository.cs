using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamRepository:IRepository<Team>
    {
        private readonly ApplicationDbContext dbContext;

        public TeamRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Team> GetAll()
        {
            return dbContext.Team;
        }

        public Team GetById(IQueryable<Team> id)
        {
            throw new NotImplementedException();
        }

        public bool InsertAsync(Team obj)
        {
            dbContext.Team.Add(obj);
            return true;
        }

        public bool UpdateAsync(Team obj)
        {
            Team team = dbContext.Team.Find(obj.Id);
            if (team == null)
                return false;
            team.TeamMembers = obj.TeamMembers;
            team.TeamName = obj.TeamName;
            team.TeamOwner = obj.TeamOwner;
            return true;
        }

        public bool DeleteAsync(object id)
        {
            Team team = dbContext.Team.Find(id);
            if (team == null)
                return false;

            dbContext.Team.Remove(team);
            return true;
        }

        public bool SaveAsync()
        {
            dbContext.SaveChangesAsync();
            return true;
        }
    }
}
