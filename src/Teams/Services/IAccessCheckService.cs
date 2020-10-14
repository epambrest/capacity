using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IAccessCheckService
    {
        Task<bool> OwnerOrMemberAsync(int team_id);
        Task<bool> IsOwnerAsync(int team_id);
    }
}
