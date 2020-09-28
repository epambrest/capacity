using System.Threading.Tasks;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> AddAsync(int team_id, string member_id);
    }
}
