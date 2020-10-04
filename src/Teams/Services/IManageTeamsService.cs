using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        Task<bool> RemoveAsync(int team_id);
    }
}
