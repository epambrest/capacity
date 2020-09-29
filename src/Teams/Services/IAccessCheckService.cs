using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IAccessCheckService
    {
        public  Task<bool> OwnerOrMemberAsync(int team_id);
    }
}
