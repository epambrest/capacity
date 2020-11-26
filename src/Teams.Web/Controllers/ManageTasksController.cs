using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Web.Controllers;
using Teams.Web.ViewModels;

namespace Teams.Web.Controllers
{
    public class ManageTasksController : Controller
    {
        private readonly IManageTasksService _manageTasksService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IStringLocalizer<ManageSprintsController> _localizer;

        public ManageTasksController(IManageTasksService manageTasksService, IAccessCheckService accessCheckService,
            IManageTeamsService manageTeamsService, IManageTeamsMembersService manageTeamsMembersService,
            IManageSprintsService manageSprintsService, IStringLocalizer<ManageSprintsController> localizer)
        {
            _manageTasksService = manageTasksService;
            _accessCheckService = accessCheckService;
            _manageTeamsService = manageTeamsService;
            _manageSprintsService = manageSprintsService;
            _manageTeamsMembersService = manageTeamsMembersService;
            _localizer = localizer;
        }

        [Authorize]
        public async Task<IActionResult> AllTasksForTeam(int teamId, DisplayOptions options)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return View("ErrorGetAllTasks");
            }
            var tasks = await _manageTasksService.GetAllTasksForTeamAsync(teamId, options);

            if (await _accessCheckService.IsOwnerAsync(teamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            var tasksForTeamViewModel = new AllTasksForTeamViewModel();
            var team = await _manageTeamsService.GetTeamAsync(teamId);

            if (tasks == null || team == null)
            {
                return View("ErrorGetAllTasks");
            }

            tasksForTeamViewModel.TeamName = team.TeamName;
            tasksForTeamViewModel.Tasks = tasks;
            return View(tasksForTeamViewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTaskByIdAsync(int teamId, int taskId)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                return View("ErrorGetTaskById");
            }

            if (await _accessCheckService.IsOwnerAsync(teamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            return View(task);
        }


        [Authorize]
        public async Task<IActionResult> RemoveInSprint(int taskId)
        {
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var sprintId = task.SprintId;
            var result = await _manageTasksService.RemoveAsync(taskId);
            if (result)
                return RedirectToAction("GetSprintById", "ManageSprints", new { sprintId });
            return RedirectToAction("ErrorRemoveTask");
        }

        [Authorize]
        public async Task<IActionResult> RemoveInTeam(int taskId)
        {
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var teamId = task.TeamId;
            var result = await _manageTasksService.RemoveAsync(taskId);
            if (result)
                return RedirectToAction("AllTasksForTeam", new { teamId });
            return RedirectToAction("ErrorRemoveTask");
        }


        [Authorize]
        public async Task<IActionResult> EditTaskAsync(int teamId, int taskId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var teamMembers = await GetAllTeamMembersAsync(teamId);

            EditTaskViewModel model = new EditTaskViewModel
            {
                TeamId = teamId,
                TaskId = task.Id,
                TaskSprintId = task.SprintId,
                TeamName = team.TeamName,
                TaskName = task.Name,
                TaskLink = task.Link,
                TaskStoryPoints = task.StoryPoints,
                TaskMemberId = task.MemberId,
                ErrorMessage = errorMessage,
                TeamMembers = teamMembers
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTaskAsync(int teamId, int taskId, int taskMemberId, int taskSprintId, string taskName, string taskLink, int taskStoryPoints)
        {
            if (string.IsNullOrEmpty(taskName))
            {
                return RedirectToAction("EditTask", new { teamId = teamId, taskId = taskId, errorMessage = _localizer["NameFieldError"] });
            }
            if (string.IsNullOrEmpty(taskLink) || !Regex.IsMatch(taskLink, (@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")))
            {
                return RedirectToAction("EditTask", new { teamId = teamId, taskId = taskId, errorMessage = _localizer["LinkFieldError"] });
            }
            if (taskStoryPoints <= 0)
            {
                return RedirectToAction("EditTask", new { teamId = teamId, taskId = taskId, errorMessage = _localizer["PointsFieldError"] });
            }
            if (taskMemberId <= 0)
            {
                return RedirectToAction("EditTask", new { teamId = teamId, taskId = taskId, errorMessage = _localizer["MemberFieldError"] });
            }

            var task = new Data.Models.Task
            {
                Id = taskId,
                TeamId = teamId,
                Name = taskName,
                StoryPoints = taskStoryPoints,
                Link = taskLink,
                SprintId = taskSprintId,
                MemberId = taskMemberId
            };
            var result = await EditTaskAsync(task);

            if (result) return RedirectToAction("AllTasksForTeam", new { teamId = teamId });
            else return RedirectToAction("EditError", new { teamId = teamId });

        }

        [Authorize, NonAction]
        public async Task<bool> EditTaskAsync(Data.Models.Task task)
        {
            if (await _accessCheckService.IsOwnerAsync(task.TeamId))
            {
                return await _manageTasksService.EditTaskAsync(task);
            }
            else return false;
        }

        [Authorize, NonAction]
        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                RedirectToAction("Error");
            }
            return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });
        }

        public IActionResult EditError(int teamId)
        {
            ViewBag.TeamId = teamId;
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}