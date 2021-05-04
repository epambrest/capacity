using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Web.ViewModels.Shared;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IStringLocalizer<ManageTeamMembersController> _localizer;
        private readonly UserManager<Data.Models.User> _userManager;

        public ManageTeamMembersController(IManageTeamsMembersService manageTeamsMembersService, IManageTeamsService manageTeamsService, 
            IAccessCheckService accessCheckService, IStringLocalizer<ManageTeamMembersController> localizer, UserManager<Data.Models.User> userManager)
        {
            _manageTeamsMembersService = manageTeamsMembersService;
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
            _localizer = localizer;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [Authorize, NonAction]
        public async Task<TeamMember> GetMemberAsync(int teamId, string memberId)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
                return await _manageTeamsMembersService.GetMemberAsync(teamId, memberId);
            else return null;
        }

        [Authorize, NonAction]
        private async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
                return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, options);
            else return null;
        }

        [Authorize]
        public async Task<IActionResult> TeamMembersAsync(int teamId)
        {
            var members = await GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            if (members == null) return View("MembersError");

            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(x => x.Id == teamId);

            if (team == null) return View("ErrorNotMember");

            var isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(teamId)) isOwner = true;

            var teamViewModel = TeamViewModel.Create(team, isOwner, new List<TeamMember>()); 

            return View(teamViewModel);
        }

        public IActionResult MembersError() => View("Index");

        public IActionResult ErrorNotMember() => View("ErrorNotMember");
     
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = ErrorViewModel.Create(Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(errorViewModel);
        }

        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int teamId)
        {
            var team = await _manageTeamsService.GetTeamAsync(teamId);

            var isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(teamId)) isOwner = true;

            var users = await _userManager.Users.ToListAsync();
            var allMembers = new List<TeamMember>();
            foreach (var user in users)
            {
                var member = Business.Models.User.Create(user.Id, user.UserName, user.FirstName, user.LastName);
                allMembers.Add(TeamMember.Create(user.Id, member));
            }

            var teamViewModel = TeamViewModel.Create(team, isOwner, allMembers);

            return View(teamViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddMemberAsync(int teamId, string memberId)
        {
            if (memberId == null) return RedirectToAction("AddError", new { errorMessage = _localizer["MemberFieldError"], teamId = teamId });

            var result = await _manageTeamsMembersService.AddAsync(teamId, memberId);

            if (result) return RedirectToAction("TeamMembers", new { teamId });
            else return RedirectToAction("AddError", new { errorMessage= _localizer["CurrentUser"], teamId = teamId });
        }
        public IActionResult AddError(string errorMessage, int teamId)
        {
            ViewData["Error"] = _localizer["Error"];
            ViewData["TeamId"] = teamId;
            ViewData["Cause"] = errorMessage;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> RemoveAsync(int teamId, string memberId)
        {
            var result = await _manageTeamsMembersService.RemoveAsync(teamId, memberId);
            if (result) return RedirectToAction("TeamMembers", new { teamId});
            return RedirectToAction("ErrorRemoveMember");
        }

        [Authorize]
        public IActionResult ErrorRemoveMember() => View();
    }
}
