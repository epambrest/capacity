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

        public ManageTeamsController(IManageTeamsService manageTeamsService)
        {
            _manageTeamsService = manageTeamsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult GetMyTeams()
        {
            return View(_manageTeamsService.GetMyTeams());
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
    }
}