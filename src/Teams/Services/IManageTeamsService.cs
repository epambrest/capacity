using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        public Task<bool> AddTeamAsync(string teamName);
    }
}
