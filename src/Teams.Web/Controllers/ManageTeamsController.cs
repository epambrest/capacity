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

namespace Teams.Web.Controllers
{
    public class ManageTeamsController : Controller
    {
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IAccessCheckService _accessCheckService;

        public ManageTeamsController(IManageTeamsService manageTeamsService, IAccessCheckService accessCheckService)
        {
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(int teamId)
        {
            var team = await GetTeamAsync(teamId);
            var teamModelView = new TeamViewModel(){Id = team.Id,TeamName =team.TeamName};
            return View(teamModelView);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(int teamId, string teamName)
        {
            ViewBag.resultOfEditing = await _manageTeamsService.EditTeamNameAsync(teamId, teamName);
            var team = await GetTeamAsync(teamId);
            var teamModelView = new TeamViewModel() { Id = team.Id, TeamName = team.TeamName };
            return View(teamModelView);
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
        public async Task<IActionResult> AddTeam(string teamName)
        {
            if (ModelState.IsValid)
            {
                await _manageTeamsService.AddTeamAsync(teamName);
                return RedirectToAction("GetMyTeams");
            }
            return View(teamName);
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