using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Repository;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace Teams.Services
{
    public class ManageTeamsService : IManageTeamsService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Team,int> _teamRepository;
        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;
            _teamRepository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            if (Regex.IsMatch(teamName, "^[a-zA-Z0-9-_,.]+$") && !_teamRepository.GetAll().Any(t => t.TeamName.Equals(teamName)))
            {
                await _teamRepository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
                return true;
            }

            return false;
        }
    }
}
