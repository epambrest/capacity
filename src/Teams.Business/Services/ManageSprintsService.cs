﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Security;

namespace Teams.Business.Services
{
    public class ManageSprintsService: IManageSprintsService
    {
        private readonly IRepository<Sprint, int> _sprintRepository;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly ICurrentUser _currentUser;

        public ManageSprintsService(IRepository<Sprint, int> sprintRepository, 
            IManageTeamsService manageTeamsService, ICurrentUser currentUser)
        {
            _sprintRepository = sprintRepository;
            _manageTeamsService = manageTeamsService;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Sprint>> GetAllSprintsAsync(int teamId, DisplayOptions options)
        {
            var allSprints = await _sprintRepository.GetAllAsync(); 
            var allTeamSprints = allSprints.Where(x => x.TeamId == teamId);

            if (options.SortDirection == SortDirection.Ascending)
            {
                return allTeamSprints.OrderBy(x => x.Name);
            }
            else if (options.SortDirection == SortDirection.Descending)
            {
                return allTeamSprints.OrderByDescending(x => x.Name);
            }
            else if (options.SortDirection == SortDirection.ByStatus)
            {
                var allTeamSprintsByStatus = allTeamSprints.OrderBy(x => x.Status).ToList();

                if (allTeamSprintsByStatus.Count > 1 && allTeamSprintsByStatus[1].Status == PossibleStatuses.ActiveStatus)
                {
                    var swapElem = allTeamSprintsByStatus[0];
                    allTeamSprintsByStatus[0] = allTeamSprintsByStatus[1];
                    allTeamSprintsByStatus[1] = swapElem;
                    return allTeamSprintsByStatus;
                }

                return allTeamSprintsByStatus;
            }

            return allTeamSprints;
        }

        public async Task<Team> GetTeam(int teamId)
        {
            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(i => i.Id == teamId);
            return team;
        }

        public async Task<Sprint> GetSprintAsync(int sprintId, bool includeTaskAndTeamMember)
        {
            if (includeTaskAndTeamMember)
            {
                var allSprints = await _sprintRepository.GetAllAsync();
                var sprint = allSprints.Where(t => t.Id == sprintId).FirstOrDefault(t => t.Id == sprintId);
                return sprint;
            }
            return await _sprintRepository.GetByIdAsync(sprintId);
        }

        public async Task<bool> AddSprintAsync(Sprint sprint)
        {
            var allSprints = await _sprintRepository.GetAllAsync();
            bool isThisSprintName = allSprints.Where(x => x.TeamId == sprint.TeamId).Any(x => x.Name == sprint.Name);

            if (isThisSprintName || sprint.DaysInSprint <= 0 || sprint.StoryPointInHours <= 0 
                || !Regex.IsMatch(sprint.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }
            return await _sprintRepository.InsertAsync(sprint);
        }


        public async Task<bool> RemoveAsync(int sprintId)
        {
            var allSprints = await _sprintRepository.GetAllAsync();
            var sprint = allSprints.FirstOrDefault(i => i.Team.TeamOwner == _currentUser.Current.Id() && i.Id == sprintId);
            
            if (sprint == null) return false;

            var result = await _sprintRepository.DeleteAsync(sprintId);
            return result;
         }

        public async Task<bool> EditSprintAsync(Sprint sprint)
        {
            var oldSprint = await _sprintRepository.GetByIdAsync(sprint.Id);

            if (oldSprint == null || 
                sprint.DaysInSprint <= 0 || sprint.StoryPointInHours <= 0 || 
                !Regex.IsMatch(sprint.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }

            return await _sprintRepository.UpdateAsync(sprint);
        }
        
        public async Task<double> GetAverageStoryPointAsync(Sprint sprint)
        {
            var sprints = await GetAllSprintsAsync(sprint.TeamId, new DisplayOptions());
            sprints = sprints.Where(t => t.Status == PossibleStatuses.CompletedStatus);

            if (sprints.Count() > 0)
            {
                var averageSp = Math.Round((double)sprints.SelectMany(t => t.Tasks)
                    .Where(t => t.Completed == true).Sum(t => t.StoryPoints) / sprints.Count(), 1);
                return averageSp;
            }
            return 0;
        }
    }
}
