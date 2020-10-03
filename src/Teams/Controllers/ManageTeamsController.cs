using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;
using System.Threading.Tasks;

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
        public IActionResult GetMyTeams()
        {
            return View(_manageTeamsService.GetMyTeams());
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}