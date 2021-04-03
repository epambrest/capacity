using System.Collections.Generic;
using System.Threading.Tasks;
using Teams.Data.Annotations;
using Teams.Data.Models;

namespace Teams.Business.Services
{
    public interface IManageTeamsMembersService
    {
        Task<bool> RemoveAsync(int teamId, string memberId);
        Task<TeamMember> GetMemberAsync(int teamId, string memberId);
        Task<bool> AddAsync(int teamId, string memberId);
        Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options);
    }
}

