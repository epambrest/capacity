using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Teams.Models;
using Teams.Services;

namespace Teams.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _manageTeamsMembersService;

        private readonly IAccessCheckService _accessCheckService;

        public ManageTeamMembersController(IManageTeamsMembersService manageTeamsMembersService, IAccessCheckService accessCheckService)
        {
            _manageTeamsMembersService = manageTeamsMembersService;

            _accessCheckService = accessCheckService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize, NonAction]
        public async Task<TeamMember> GetMemberAsync(int team_id, string member_id)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                return await _manageTeamsMembersService.GetMemberAsync(team_id, member_id);
            }
            else return null;
        }

        [Authorize, NonAction]
        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int team_id, DisplayOptions options)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                return await _manageTeamsMembersService.GetAllTeamMembersAsync(team_id, options);
            }
            else return null;
        }

        [Authorize]
        public async Task<IActionResult> TeamMembersAsync(int team_id, string team_name, string owner_name)
        {
            List<TeamMember> members = await GetAllTeamMembersAsync(team_id, new DisplayOptions { });
            if (members == null) return View("MembersEror");
            ViewBag.Members = members;
            ViewBag.TeamName = team_name;
            ViewBag.TeamId = team_id;
            ViewBag.TeamOwner = owner_name;
            return View();
        }

        public IActionResult MembersEror()
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
        public async Task<IActionResult> Remove(int team_id, string member_id, string owner_name, string team_name)
        {
            var result = await _manageTeamsMembersService.RemoveAsync(team_id, member_id);
            if (result)
            {
                return RedirectToAction("TeamMembers", new { team_id = team_id, team_name = team_name, owner_name = owner_name });
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
        public async Task<IActionResult> Add(int team_id, string member_id)
        {
            await _manageTeamsMembersService.AddAsync(team_id, member_id);
            return View();
        }
    }
}
