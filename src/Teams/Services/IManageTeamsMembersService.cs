using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        public Task<TeamMember> GetMemberAsync(int team_id, string member_id);
    }
}
