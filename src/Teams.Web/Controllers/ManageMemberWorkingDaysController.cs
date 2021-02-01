using Microsoft.AspNetCore.Mvc;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.MemberWorkingDays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Web.Controllers;
using Teams.Web.ViewModels.Task;
using Teams.Web.ViewModels.TeamMember;
using Microsoft.AspNetCore.Authorization;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.Controllers
{
    public class ManageMemberWorkingDaysController : Controller
    {
        private readonly IManageMemberWorkingDaysService _manageMemberWorkingDaysService;
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IManageSprintsService _manageSprintsService;


        public ManageMemberWorkingDaysController(IManageMemberWorkingDaysService manageMemberWorkingDaysService, IManageTeamsMembersService manageTeamsMembersService, IManageSprintsService manageSprintsService)
        {
            _manageMemberWorkingDaysService = manageMemberWorkingDaysService;
            _manageTeamsMembersService = manageTeamsMembersService;
            _manageSprintsService = manageSprintsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllWorkingDays(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            var workingDays = await _manageMemberWorkingDaysService.GetAllWorkingDaysForSprintAsync(sprintId);
            var team = await _manageSprintsService.GetTeam(sprint.TeamId);
            List<TeamMember> teamMembers = await _manageTeamsMembersService.GetAllTeamMembersAsync(sprint.TeamId, new DisplayOptions { });
            var model = new SprintAndTeamViewModel
            {
                Sprints = new List<SprintViewModel>(),
                memberWorkingDays = new List<MemberWorkingDaysViewModels>()
            };
            model.sprintId = sprintId;
            model.Team = new TeamViewModel() { Id = team.Id, Owner = team.Owner, TeamName = team.TeamName, TeamMembers = new List<TeamMemberViewModel>() };
            teamMembers.ForEach(t => model.Team.TeamMembers.Add(new TeamMemberViewModel() { Id = t.Id, Member = t.Member, MemberId = t.MemberId }));
            workingDays.ToList().ForEach(t => model.memberWorkingDays.Add(new MemberWorkingDaysViewModels() { Id = t.Id, SprintId = sprintId, MemberId = t.MemberId, WorkingDays = t.WorkingDays }));
            return PartialView("_WorkingDaysPartial", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<bool> EditWorkingDays(int workingDaysId, int workingDays)
        {
            var memberWorkingDays = await _manageMemberWorkingDaysService.GetWorkingDaysByIdAsync(workingDaysId);
            if (memberWorkingDays == null)
                return false;
            
            var newMemberWorkingDays = new MemberWorkingDays { Id = memberWorkingDays.Id, MemberId = memberWorkingDays.MemberId, SprintId = memberWorkingDays.SprintId, Sprint = memberWorkingDays.Sprint, WorkingDays = workingDays };
            if (newMemberWorkingDays.WorkingDays >= 0 && newMemberWorkingDays.WorkingDays <= newMemberWorkingDays.Sprint.DaysInSprint)
            {
                return await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(newMemberWorkingDays);
            }
            else
                return false;
        }

        [Authorize]
        [HttpGet]
        public async Task<int> AddWorkingDays(int sprintId, int memberId, int workingDays)
        {
            var sprint = _manageSprintsService.GetSprintAsync(sprintId, false).Result;
            if (TryValidateModel(new MemberWorkingDays { SprintId = sprintId, MemberId = memberId, WorkingDays = workingDays, Sprint = sprint }))
            {
                var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(new MemberWorkingDays { SprintId = sprintId, MemberId = memberId, WorkingDays = workingDays });
                if (result)
                {
                    var memberWorkingDays = await _manageMemberWorkingDaysService.GetAllWorkingDaysForSprintAsync(sprintId);
                    return memberWorkingDays.FirstOrDefault(i => i.MemberId == memberId).Id;
                }
                else
                    return -1;
            }
            else
                return -1;
        }

    }
}
