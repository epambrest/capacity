using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<TeamMember> GetMemberAsync(int team_id, string member_id);
        Task<bool> AddAsync(int team_id, string member_id);
        Task<bool> DeleteAsync(int team_id, string member_id);
        Task<List<TeamMember>> GetAllTeamMembersAsync(int team_id, DisplayOptions options);
    }
}

