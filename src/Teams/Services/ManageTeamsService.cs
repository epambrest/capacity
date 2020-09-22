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
        private readonly IRepository<Team,int> _repository;
        public ManageTeamsService(CurrentUser currentUser, TeamRepository teamRepository)
        {
            _currentUser = currentUser;
            _repository = teamRepository;
        }

        public async Task<bool> AddTeamAsync(string teamName)
        {
            if (teamName != "")
            {
                if (!_repository.GetAll().Result.Any(t => t.TeamName.Equals(teamName)))
                {
                    await _repository.InsertAsync(new Team { TeamOwner = _currentUser.Current.Id(), TeamName = teamName });
                    return true;
                }
            }

            return false;
        }
    }
}
