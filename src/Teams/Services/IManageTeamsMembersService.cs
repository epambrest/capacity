using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> RemoveAsync(int team_id, string member_id);
        Task<TeamMember> GetMemberAsync(int team_id, string member_id);
    }
}
