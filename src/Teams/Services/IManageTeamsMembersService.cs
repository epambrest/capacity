using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        public TeamMember GetMember(int team_id, string member_id);
    }
}
