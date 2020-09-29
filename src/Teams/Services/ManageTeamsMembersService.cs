using Teams.Repository;
using Teams.Models;
using Teams.Data;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Teams.Services
{
    public class ManageTeamsMembersService : IManageTeamsMembersService
    {
        private readonly IRepository<TeamMember, int> _teamMemberRepository;

        public ManageTeamsMembersService(IRepository<TeamMember, int> teamMemberRepository)
        {
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<TeamMember> GetMemberAsync(int team_id, string member_id)
        {
             return await _teamMemberRepository.GetAll()
                    .Where(x => x.MemberId == member_id && x.TeamId == team_id)
                    .FirstOrDefaultAsync();
        }
    }
}