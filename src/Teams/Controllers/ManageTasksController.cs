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
    public class ManageTasksController : Controller
    {
        private readonly IManageTasksService _manageTasksService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IManageSprintsService _manageSprintsService;

        public ManageTasksController(IManageTasksService manageTasksService, IAccessCheckService accessCheckService, 
            IManageTeamsService manageTeamsService,IManageSprintsService manageSprintsService)
        {
            _manageTasksService = manageTasksService;
            _accessCheckService = accessCheckService;
            _manageTeamsService = manageTeamsService;
            _manageSprintsService = manageSprintsService;
        }

        [Authorize]
        public async Task<IActionResult> AllTasksInTeam(int team_id, DisplayOptions options)
        {

            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                var tasks = await _manageTasksService.GetMyTaskInTeamAsync(team_id, options);
                var team = await _manageTeamsService.GetTeamAsync(team_id);
                ViewBag.TeamName = team.TeamName;
                return View(tasks);
            }
            else
                return View("ErrorGetAllTasks");
        }
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
