using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Web.ViewModels.Shared;
using Teams.Web.ViewModels.Team;

namespace Teams.Web.Controllers
{
    public class ManageTeamsController : Controller
    {
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IStringLocalizer<ManageTeamsController> _localizer;

        public ManageTeamsController(IManageTeamsService manageTeamsService, IAccessCheckService accessCheckService, IStringLocalizer<ManageTeamsController> localizer)
        {
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
            _localizer = localizer;
        }

        public IActionResult Index() => View();

        [Authorize]
        public async Task<IActionResult> GetMyTeamsAsync()
        {
            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var teamViewsModels = new List<TeamViewModel>();

            foreach (var team in  teams)
            {
                var teamViewModel = TeamViewModel.Create(team, false, new List<TeamMember>());
                teamViewsModels.Add(teamViewModel);
            }

            return View(teamViewsModels);
        }

        [Authorize, NonAction]
        private async Task<Team> GetTeamAsync(int teamId)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
                return await _manageTeamsService.GetTeamAsync(teamId);
            else return null;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(TeamViewModel teamViewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.resultOfEditing = await _manageTeamsService.EditTeamNameAsync(teamViewModel.Id, teamViewModel.TeamName);
                return RedirectToAction("GetMyTeams");
            }

            return RedirectToAction("GetMyTeams");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = ErrorViewModel.Create(Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(errorViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTeam(TeamViewModel teamViewModel)
        {
            if (ModelState.IsValid)
            {
                await _manageTeamsService.AddTeamAsync(teamViewModel.TeamName);
                return RedirectToAction("GetMyTeams");
            }

            return RedirectToAction("GetMyTeams");
        }

        public IActionResult NameError()
        {
            ViewData["Error"] = _localizer["Error"];
            ViewData["Cause"] = _localizer["NameEmpty"];
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Remove(int teamId)
        {
            var result = await _manageTeamsService.RemoveAsync(teamId);
            if (result) return RedirectToAction("GetMyTeams");
            return RedirectToAction("ErrorRemove");
        }

        public IActionResult ErrorRemoveAsync() => View();
    }
}