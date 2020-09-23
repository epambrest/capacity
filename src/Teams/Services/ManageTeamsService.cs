using System.Collections.Generic;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;

        private IRepository<Team, int> _teamRepository;

        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;

            _teamRepository = teamRepository;
        }

        public IEnumerable<Team> GetMyTeams() => _teamRepository.GetAll().Include(m => m.TeamMembers).AsEnumerable()
                .Where(x => x.TeamOwner == _currentUser.Current.Id() || x.TeamMembers.Any(p => p.MemberId == _currentUser.Current.Id()))
                .OrderByDescending(y => y.TeamOwner == _currentUser.Current.Id());
    }
}
