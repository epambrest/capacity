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
        public ICurrentUser _currentUser;
        private readonly IRepository<Team,int> _repository;
        public ManageTeamsService(ICurrentUser currentUser, IRepository<Team, int> teamRepository)
        {
            _currentUser = currentUser;
            _repository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            if (Regex.IsMatch(teamName, "^[a-zA-Z0-9-_,.]+$") && !_repository.GetAll().Result.Any(t => t.TeamName.Equals(teamName)))
            {
                await _repository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
                return true;
            }

            return false;
        }
    }
}
