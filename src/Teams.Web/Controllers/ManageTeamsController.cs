using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Teams.Business.Services;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

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
            return View(await _manageTeamsService.GetMyTeamsAsync());
        }

        [Authorize, NonAction]
        public async Task<Team> GetTeamAsync(int teamId)
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
            return View(await GetTeamAsync(teamId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTeamNameAsync(int teamId, string teamName)
        {
            ViewBag.resultOfEditing = await _manageTeamsService.EditTeamNameAsync(teamId, teamName);
            return View(await GetTeamAsync(teamId));
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