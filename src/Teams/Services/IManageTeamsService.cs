using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        Task<bool> AddTeamAsync(string teamName);
    }
}
