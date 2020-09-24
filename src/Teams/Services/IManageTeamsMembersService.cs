using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> Remove(int team_id, string member_id);
    }
}
