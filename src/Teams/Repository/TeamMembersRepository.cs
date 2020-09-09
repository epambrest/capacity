using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Repository
{
    public class TeamMembersRepository : IRepository<TeamMembers>
    {
        private readonly ApplicationDbContext dbContext;

        public TeamMembersRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool DeleteAsync(object id)
        {
            TeamMembers member = dbContext.TeamMembers.Find(id);
            if (member == null)
                return false;

            dbContext.TeamMembers.Remove(member);
            return true;
        }

        public IEnumerable<TeamMembers> GetAll()
        {
            return dbContext.TeamMembers;
        }

        public TeamMembers GetById(IQueryable<TeamMembers> id)
        {
            throw new NotImplementedException();
        }

        public bool InsertAsync(TeamMembers obj)
        {
            dbContext.TeamMembers.Add(obj);
            return true;
        }

        public bool SaveAsync()
        {
            dbContext.SaveChangesAsync();
            return true;
        }

        public bool UpdateAsync(TeamMembers obj)
        {
            TeamMembers member = dbContext.TeamMembers.Find(obj.Id);
            if (member == null)
                return false;
            member.Member = obj.Member;
            member.MemberId = obj.MemberId;
            member.Team = obj.Team;
            member.TeamId = obj.TeamId;
            return true;
        }
    }
}
