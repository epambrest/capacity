using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teams.Models;
using Teams.Services;

namespace Teams.Controllers
{
    public class ManageSprintsController : Controller
    {
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;


        public ManageSprintsController(IManageSprintsService manageSprintsService, IAccessCheckService accessCheckService, IManageTeamsService manageTeamsService)
        {
            _manageSprintsService = manageSprintsService;
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
        }

        [Authorize]
        public async Task<IActionResult> AllSprints(int teamId, DisplayOptions options)
        {
            List<Sprint> sprints;
            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                sprints = (List<Sprint>)await _manageSprintsService.GetAllSprintsAsync(teamId, options);
            }
            else 
                return View("ErrorGetAllSprints");

            var team = await _manageSprintsService.GetTeam(teamId);
            ViewBag.TeamName = team.TeamName;
            return View(sprints);
        }

        [Authorize]
        public async Task<IActionResult> GetSprintById(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            if (sprint == null)
                return View("ErrorGetAllSprints");
            return View(sprint);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
