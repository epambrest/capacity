using System.Threading.Tasks;

namespace Teams.Business.Services
{
    public interface IAccessCheckService
    {
        Task<bool> OwnerOrMemberAsync(int teamId);
        Task<bool> IsOwnerAsync(int teamId);
    }
}
