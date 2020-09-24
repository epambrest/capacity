using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        bool Remove(int team_id, string member_id);
    }
}
