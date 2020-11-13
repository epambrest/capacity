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
            IManageTeamsService manageTeamsService, IManageSprintsService manageSprintsService)
        {
            _manageTasksService = manageTasksService;
            _accessCheckService = accessCheckService;
            _manageTeamsService = manageTeamsService;
            _manageSprintsService = manageSprintsService;
        }

        [Authorize]
        public async Task<IActionResult> AllTasksForTeam(int teamId, DisplayOptions options)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return View("ErrorGetAllTasks");
            }
            var tasks = await _manageTasksService.GetAllTasksForTeamAsync(teamId, options);
            if (tasks == null)
            {
                return View("ErrorGetAllTasks");
            }
            return View(tasks);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTaskByIdAsync(int teamId, int taskId)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return View("ErrorGetTaskById");
            }

            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            return View(task);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
