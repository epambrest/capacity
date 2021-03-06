﻿using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Security;

namespace Teams.Business.Services
{
    public class AccessCheckService: IAccessCheckService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team, int> _teamRepository;

        public AccessCheckService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
        }

        public async Task<bool> OwnerOrMemberAsync(int teamId)
        {
            var allTeams = await _teamRepository.GetAllAsync();
            return allTeams.Where(t => t.Id == teamId)
                .Any(y => y.TeamOwner == _currentUser.Current.Id() || y.TeamMembers.Any(z => z.MemberId == _currentUser.Current.Id()));
        }

        public async Task<bool> IsOwnerAsync(int teamId)
        {
            var allTeams = await _teamRepository.GetAllAsync();
            return allTeams.Where(t => t.Id == teamId).Any(y => y.TeamOwner == _currentUser.Current.Id());
        }
    }
}
