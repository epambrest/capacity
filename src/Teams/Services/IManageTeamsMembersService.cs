using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> Add(int team_id, string member_id);
    }
}
