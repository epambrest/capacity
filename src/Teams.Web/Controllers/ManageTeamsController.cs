using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Teams.Business.Services;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;
using Teams.Web.ViewModels;
using Teams.Web.ViewModels.Team;
using Microsoft.Extensions.Localization;

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

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> GetMyTeamsAsync()
        {
            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var teamModelView = new List<TeamViewModel>();
            teams.ToList().ForEach(t=>teamModelView.Add(new TeamViewModel(){Owner = t.Owner,TeamName = t.TeamName,TeamOwner = t.TeamOwner,Id = t.Id}));
            return View(teamModelView);
        }

        [Authorize, NonAction]
        private async Task<Team> GetTeamAsync(int teamId)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return await _manageTeamsService.GetTeamAsync(teamId);
            }
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
        public IActionResult AddTeam()
        {
            return View();
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

            return View();
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
            if (result) 
               return RedirectToAction("GetMyTeams");
            return RedirectToAction("ErrorRemove");
        }

        public IActionResult ErrorRemoveAsync()
        {
            return View();
        }
    }
}