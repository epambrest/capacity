using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Security;
using Teams.Web.ViewModels.Team;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _manageTeamsMembersService;

        private readonly IManageTeamsService _manageTeamsService;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly IAccessCheckService _accessCheckService;

        public ManageTeamMembersController(IManageTeamsMembersService manageTeamsMembersService, IManageTeamsService manageTeamsService, IAccessCheckService accessCheckService, UserManager<IdentityUser> userManager)
        {
            _manageTeamsMembersService = manageTeamsMembersService;

            _manageTeamsService = manageTeamsService;

            _accessCheckService = accessCheckService;

            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize, NonAction]
        public async Task<TeamMember> GetMemberAsync(int teamId, string memberId)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return await _manageTeamsMembersService.GetMemberAsync(teamId, memberId);
            }
            else return null;
        }

        [Authorize, NonAction]
        private async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, options);
            }
            else return null;
        }

        [Authorize]
        public async Task<IActionResult> TeamMembersAsync(int teamId)
        {
            List<TeamMember> members = await GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            if (members == null) return View("MembersError");

            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(x => x.Id == teamId);
            if (team == null) return View("ErrorNotMember");

            if (await _accessCheckService.IsOwnerAsync(teamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";
            var teamViewModel = new TeamViewModel() { Id = team.Id, TeamName = team.TeamName, Owner = team.Owner,TeamMembers = new List<TeamMemberViewModel>()};
            members.ForEach(t=> teamViewModel.TeamMembers.Add(new TeamMemberViewModel(){MemberId = t.MemberId,Member = t.Member}));
            return View(teamViewModel);
        }

        public IActionResult MembersError()
        {
            return View("Index");
        }

        public IActionResult ErrorNotMember()
        {
            return View("ErrorNotMember");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int teamId)
        {
            Team team = await _manageTeamsService.GetTeamAsync(teamId);
            var users = await _userManager.Users.ToListAsync();

            var teamViewModel = new TeamViewModel() { Id = team.Id, TeamName = team.TeamName, TeamMembers = new List<TeamMemberViewModel>() };
            users.ForEach(t => teamViewModel.TeamMembers.Add(new TeamMemberViewModel() { MemberId = t.Id, Member = t}));

            return View(teamViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int teamId, string memberId)
        {
            if (memberId == null) return RedirectToAction("AddError", new { errorMessage = "Field is empty" });
            /*var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users;*/
            bool result = await _manageTeamsMembersService.AddAsync(teamId, memberId);

            if (result) return RedirectToAction("TeamMembers", new { teamId });
            else return RedirectToAction("AddError", new { errorMessage = "Current user already in team" });
        }
        public IActionResult AddError(string errorMessage)
        {
            ViewBag.errorMessage = errorMessage;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> RemoveAsync(int teamId, string memberId)
        {
            var result = await _manageTeamsMembersService.RemoveAsync(teamId, memberId);
            if (result)
            {
                return RedirectToAction("TeamMembers", new { teamId});
            }
            return RedirectToAction("ErrorRemoveMember");
        }

        [Authorize]
        public IActionResult ErrorRemoveMember()
        {
            return View();
        }
    }
}
