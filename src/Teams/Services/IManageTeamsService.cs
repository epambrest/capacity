using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsService
    {
        Task<bool> Remove(int team_id);
    }
}
