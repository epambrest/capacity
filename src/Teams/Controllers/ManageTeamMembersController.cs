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
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Controllers
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
        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
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
            var team = teams.Where(x => x.Id == teamId).FirstOrDefault();

            if (await _accessCheckService.IsOwnerAsync(teamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamId = team.Id;
            ViewBag.TeamOwner = team.Owner.Email;
            return View(members);
        }

        public IActionResult MembersError()
        {
            return View("Index");
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

            ViewBag.TeamId = team.Id;
            ViewBag.TeamName = team.TeamName;
            ViewBag.Users = users;

            return View(await TeamMembersAsync(teamId));
        }


        [Authorize]
        public async Task<IActionResult> RemoveAsync(int teamId, string memberId, string ownerName, string teamName)
        {
            var result = await _manageTeamsMembersService.RemoveAsync(teamId, memberId);
            if (result)
            {
                return RedirectToAction("TeamMembers", new { teamId, teamName, ownerName });
            }
            return RedirectToAction("ErrorRemoveMember");
        }

        [Authorize]
        public IActionResult ErrorRemoveMember()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int teamId, string memberId)
        {
            if (memberId == null) return RedirectToAction("AddError", new { errorMessage = "Field is empty" });
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users;
            bool result = await _manageTeamsMembersService.AddAsync(teamId, memberId);


            if (result) return RedirectToAction("TeamMembers", new { teamId });
            else return RedirectToAction("AddError", new { errorMessage = "Current user already in team" });
        }

        public IActionResult AddError(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
    }
}
