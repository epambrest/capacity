using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Models;

namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> RemoveAsync(int teamId, string memberId);
        Task<TeamMember> GetMemberAsync(int teamId, string memberId);
        Task<bool> AddAsync(int teamId, string memberId);
        Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options);
    }
}

