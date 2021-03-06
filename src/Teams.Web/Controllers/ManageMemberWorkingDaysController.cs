﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Web.ViewModels.Sprint;

namespace Teams.Web.Controllers
{
    public class ManageMemberWorkingDaysController : Controller
    {
        private readonly IManageMemberWorkingDaysService _manageMemberWorkingDaysService;
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IAccessCheckService _accessCheckService;

        public ManageMemberWorkingDaysController(IManageMemberWorkingDaysService manageMemberWorkingDaysService,
            IManageSprintsService manageSprintsService, 
            IAccessCheckService accessCheckService)
        {
            _manageMemberWorkingDaysService = manageMemberWorkingDaysService;
            _manageSprintsService = manageSprintsService;
            _accessCheckService = accessCheckService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllWorkingDays(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            var workingDays = await _manageMemberWorkingDaysService.GetAllWorkingDaysForSprintAsync(sprintId);
            var team = await _manageSprintsService.GetTeam(sprint.TeamId);

            var isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(team.Id))
                isOwner = true;

            var sprintAndTeamViewModel = SprintAndTeamViewModel.Create(sprint, new List<Sprint>(), team, isOwner, workingDays.ToList());

            return PartialView("_WorkingDaysPartial", sprintAndTeamViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<bool> EditWorkingDays(int workingDaysId, int workingDays)
        {
            var memberWorkingDays = await _manageMemberWorkingDaysService.GetWorkingDaysByIdAsync(workingDaysId);
            if (memberWorkingDays == null) return false;
            
            var newMemberWorkingDays = MemberWorkingDays.Create(memberWorkingDays.Id, 
                memberWorkingDays.MemberId, 
                memberWorkingDays.SprintId, 
                memberWorkingDays.Sprint, 
                workingDays);

            if (newMemberWorkingDays.WorkingDays >= 0 && newMemberWorkingDays.WorkingDays <= newMemberWorkingDays.Sprint.DaysInSprint)
                return await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(newMemberWorkingDays);
            else return false;
        }

        [Authorize]
        [HttpGet]
        public async Task<int> AddWorkingDays(int sprintId, int memberId, int workingDays)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, false);
            if (TryValidateModel(MemberWorkingDays.Create(memberId, sprintId, sprint, workingDays)))
            {
                var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(MemberWorkingDays.Create(memberId, 
                    sprintId, 
                    null, 
                    workingDays));
                
                if (result)
                {
                    var memberWorkingDays = await _manageMemberWorkingDaysService.GetAllWorkingDaysForSprintAsync(sprintId);
                    return memberWorkingDays.FirstOrDefault(i => i.MemberId == memberId).Id;
                }
                else return -1;
            }
            else return -1;
        }
    }
}
