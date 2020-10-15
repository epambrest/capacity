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
        public async Task<IActionResult> TeamMembersAsync(int team_id)
        {
            List <TeamMember> members = await GetAllTeamMembersAsync(team_id, new DisplayOptions { });

            if (members == null) return View("MembersError");

            var teams =await  _manageTeamsService.GetMyTeamsAsync();
            var team = teams.Where(x => x.Id == team_id).FirstOrDefault();

            if (await _accessCheckService.IsOwnerAsync(team_id)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            ViewBag.Members = members;
            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamId = team.Id;
            ViewBag.TeamOwner = team.Owner.Email;
            return View();
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int team_id)
        {
            Team team = await _manageTeamsService.GetTeamAsync(team_id);
            var users = await _userManager.Users.ToListAsync();

            ViewBag.TeamId = team.Id;
            ViewBag.TeamName = team.TeamName;
            ViewBag.Users = users;

            return View(await TeamMembersAsync(team_id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int team_id, string member_id)
        {
            if (member_id == null) return RedirectToAction("AddError", new { error_message = "Field is empty"});
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Users = users;
            bool result = await _manageTeamsMembersService.AddAsync(team_id, member_id);
            

            if (result) return RedirectToAction("TeamMembers", new { team_id = team_id});
            else return RedirectToAction("AddError", new { error_message = "Current user already in team" });
        }

        public IActionResult AddError(string error_message)
        {
            ViewBag.ErrorMessage = error_message;
            return View();
        }
    }
}
