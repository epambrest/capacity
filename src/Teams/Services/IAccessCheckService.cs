using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IAccessCheckService
    {
        Task<bool> OwnerOrMemberAsync(int teamId);
        Task<bool> IsOwnerAsync(int teamId);
    }
}
