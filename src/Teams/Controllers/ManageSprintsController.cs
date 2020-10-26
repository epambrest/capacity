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

        [Authorize, NonAction]
        public async Task<List<Sprint>> GetAllSprints(int team_id, DisplayOptions options)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                return (List<Sprint>)await _manageSprintsService.GetAllSprintsAsync(team_id, options);
            }
            else return null;
        }

        [Authorize]
        public async Task<IActionResult> AllSprints(int team_id)
        {
            List<Sprint> sprints = await GetAllSprints(team_id, new DisplayOptions { });

            if (sprints == null) 
                return View("ErrorGetAllSprints");

            var teams = await _manageTeamsService.GetMyTeamsAsync();
            var team = teams.FirstOrDefault(i => i.Id == team_id);

            if (await _accessCheckService.IsOwnerAsync(team_id)) 
                ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            ViewBag.TeamName = team.TeamName;
            return View(sprints);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
