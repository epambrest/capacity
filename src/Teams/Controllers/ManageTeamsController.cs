using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using Teams.Models;
using System.Linq;
using Teams.Data;
using Teams.Security;
using Teams.Services;

namespace Teams.Controllers
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
            return View(await _manageTeamsService.GetMyTeamsAsync());
        }

        [Authorize, NonAction]
        public async Task<Team> GetTeamAsync(int team_id)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                return await _manageTeamsService.GetTeamAsync(team_id);
            }
            else return null;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(int team_id)
        {
            return View(await GetTeamAsync(team_id));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(int team_id, string team_name)
        {
            ViewBag.resultOfEditing = await _manageTeamsService.EditTeamNameAsync(team_id, team_name);
            return View(await GetTeamAsync(team_id));
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
                return RedirectToAction(nameof(Index));
            }
            return View(teamName);
        }

        [Authorize]
        public async Task<IActionResult> Remove(int team_id)
        {
            var result = await _manageTeamsService.RemoveAsync(team_id);
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