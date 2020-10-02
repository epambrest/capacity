using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<TeamMember> GetMemberAsync(int team_id, string member_id);
    }
}
