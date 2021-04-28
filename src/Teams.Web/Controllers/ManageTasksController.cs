using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.Task;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.Controllers
{
    public class ManageTasksController : Controller
    {
        private readonly IManageTasksService _manageTasksService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IManageMemberWorkingDaysService _manageMemberWorkingDaysService;
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IStringLocalizer<ManageTasksController> _localizer;

        public ManageTasksController(IManageTasksService manageTasksService, IAccessCheckService accessCheckService,
            IManageTeamsService manageTeamsService, IManageTeamsMembersService manageTeamsMembersService,
            IManageSprintsService manageSprintsService,
             IManageMemberWorkingDaysService manageMemberWorkingDaysService, IStringLocalizer<ManageTasksController> localizer)
        {
            _manageTasksService = manageTasksService;
            _accessCheckService = accessCheckService;
            _manageTeamsService = manageTeamsService;
            _manageSprintsService = manageSprintsService;
            _manageMemberWorkingDaysService = manageMemberWorkingDaysService;
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

            var tasksForTeamViewModel = await GetAllTasksForTeamViewModel(teamId, options);

            if (tasksForTeamViewModel.Tasks == null || tasksForTeamViewModel.Sprints == null || tasksForTeamViewModel.Members == null)
            {
                return View("ErrorGetAllTasks");
            }

            return View(tasksForTeamViewModel);
        }

        [Authorize, NonAction]
        private async Task<AllTasksForTeamViewModel> GetAllTasksForTeamViewModel(int teamId, DisplayOptions options)
        {
            options.SortDirection = SortDirection.ByStatus;
            var tasks = await _manageTasksService.GetAllTasksForTeamAsync(teamId, options);
            var team = await _manageTeamsService.GetTeamAsync(teamId);
            var sprints = await _manageSprintsService.GetAllSprintsAsync(teamId, options);

            bool isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(teamId))
            {
                isOwner = true;
            }

            AllTasksForTeamViewModel tasksForTeamViewModel = AllTasksForTeamViewModel.Create(team, isOwner, sprints.ToList(), tasks.ToList());

            return tasksForTeamViewModel;
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
            TaskViewModel taskViewModel = TaskViewModel.Create(task);

            return View(taskViewModel);
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
        public async Task<IActionResult> EditTaskAsync(int taskId, string errorMessage)
        {
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var team = await _manageSprintsService.GetTeam(task.TeamId);
            var teamMembers = await GetAllTeamMembersAsync(task.TeamId);
            var teamMember = teamMembers.FirstOrDefault(t => t.Id == task.MemberId);

            TaskFormViewModel taskFormViewModel = TaskFormViewModel.Create(task, errorMessage, teamMembers, new List<Sprint>());

            return View(taskFormViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditTaskAsync(TaskFormViewModel taskFormViewModel)
        {
            if (ModelState.IsValid)
            {
                if (taskFormViewModel.LinkValidation == null && !Regex.IsMatch(taskFormViewModel.TaskLink, 
                    @"^(?:http(s):\/\/)(github\.com\/)|(bitbucket\.org\/)[\w\d\S]+(\/[\w\d\S]+)*$"))
                {
                    return RedirectToAction("EditTask", new { teamId = taskFormViewModel.TeamId, 
                        taskId = taskFormViewModel.TaskId, errorMessage = _localizer["LinkFieldError"] });
                }
                else if (taskFormViewModel.LinkValidation != null && !Regex.IsMatch(taskFormViewModel.TaskLink,
                    @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$"))
                {
                    return RedirectToAction("EditTask", new { teamId = taskFormViewModel.TeamId, 
                        taskId = taskFormViewModel.TaskId, errorMessage = _localizer["LinkFieldError"] });
                }

                var task = Business.Models.Task.Create(taskFormViewModel.TaskId,
                    taskFormViewModel.TeamId,
                    null,
                    taskFormViewModel.TaskName,
                    taskFormViewModel.TaskStoryPoints,
                    taskFormViewModel.TaskLink,
                    taskFormViewModel.TaskSprintId,
                    taskFormViewModel.TaskMemberId);

                var currentTask = await _manageTasksService.GetTaskByIdAsync(taskFormViewModel.TaskId);

                if (currentTask.Name == taskFormViewModel.TaskName && 
                    currentTask.Link == taskFormViewModel.TaskLink &&
                    currentTask.StoryPoints == taskFormViewModel.TaskStoryPoints&&
                    currentTask.MemberId == taskFormViewModel.TaskMemberId
                    )
                {
                    return RedirectToAction("EditTask", new { teamId = taskFormViewModel.TeamId, 
                        taskId = taskFormViewModel.TaskId, errorMessage = _localizer["HasntAnyChange"] });
                }

                var result = await EditTaskAsync(task);

                if (result) return RedirectToAction("AllTasksForTeam", new { teamId = taskFormViewModel.TeamId });
                else return RedirectToAction("NotOwnerError", new { teamId = taskFormViewModel.TaskId });
            }

            var teamMembers = await GetAllTeamMembersAsync(taskFormViewModel.TeamId);
            taskFormViewModel.TeamMembers = new List<TeamMemberViewModel>();
            
            foreach(var teamMember in teamMembers)
            {
                TeamMemberViewModel teamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                taskFormViewModel.TeamMembers.Add(teamMemberViewModel);
            }

            return View(taskFormViewModel);
        }

        [Authorize, NonAction]
        private async Task<bool> EditTaskAsync(Business.Models.Task task)
        {
            if (await _accessCheckService.IsOwnerAsync(task.TeamId))
            {
                return await _manageTasksService.EditTaskAsync(task);
            }
            else return false;
        }

        [Authorize, NonAction]
        private async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                RedirectToAction("Error");
            }
            return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });
        }

        [Authorize]
        public IActionResult GetResultError(string errorMessage)
        {
            GetResultErrorViewModel getResultErrorViewModel = GetResultErrorViewModel.Create(errorMessage);
            return View(getResultErrorViewModel);
        }

        [Authorize]
        public async Task<IActionResult> GetResultTeamMember(int sprintId, int teamMemberId = 1)
        {
            var completedSprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            var members = await GetAllTeamMembersAsync(completedSprint.TeamId);
            var currentMember = members.FirstOrDefault(member => member.Id == teamMemberId);
            
            if (completedSprint == null || currentMember == null || completedSprint == null)
                return RedirectToAction("GetResultError", new { errorMessage = _localizer["CouldntGetData"] });
            if (completedSprint.Status != 2)
                return RedirectToAction("GetResultError", new { errorMessage = _localizer["StatusIsNotComplete"] });

            Dictionary<OtherNamesTaskParams, double> tasksAllParams = await _manageTasksService.GetTasksAllParamsForMember(teamMemberId, sprintId);
            int teamMemberTotalSp = (int)tasksAllParams.GetValueOrDefault(OtherNamesTaskParams.TeamMemberTotalSp);
            var storyPointsInDay = await _manageMemberWorkingDaysService.GetStoryPointsInDayForMember(sprintId, teamMemberId, teamMemberTotalSp);
            tasksAllParams.Add(OtherNamesTaskParams.StoryPointsInDay, storyPointsInDay);
            var resultsTasksForMemberViewModel = ResultsTasksForMemberViewModel.Create(completedSprint, currentMember, tasksAllParams);

            return View(resultsTasksForMemberViewModel);
        }

        public IActionResult NotOwnerError(int teamId)
        {
            ViewBag.TeamId = teamId;
            ViewData["Error"] = _localizer["Error"];
            ViewData["Cause"] = _localizer["NotOwner"];
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddTaskError(int teamId)
        {
            return View(teamId);
        }
        
        [Authorize]
        public async Task<IActionResult> AddTaskAsync(int teamId, int sprintId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var teamMembers = await GetAllTeamMembersAsync(teamId);
            Business.Models.Task task = Business.Models.Task.Create(teamId, sprintId, team);

            TaskFormViewModel model = TaskFormViewModel.Create(task, errorMessage, teamMembers, new List<Sprint>());

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTaskAsync(TaskFormViewModel taskFormViewModel)
        {
            if (ModelState.IsValid)
            {
                if (taskFormViewModel.LinkValidation == null && !Regex.IsMatch(taskFormViewModel.TaskLink, 
                    @"^(?:http(s):\/\/)(github\.com\/)|(bitbucket\.org\/)[\w\d\S]+(\/[\w\d\S]+)*$"))
                {
                    return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, 
                        sprintId = taskFormViewModel.TaskSprintId , errorMessage = _localizer["LinkFieldError"] });
                }
                else if (taskFormViewModel.LinkValidation != null && !Regex.IsMatch(taskFormViewModel.TaskLink, 
                    @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$"))
                {
                    return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, 
                        sprintId = taskFormViewModel.TaskSprintId, errorMessage = _localizer["LinkFieldError"] });
                }

                var task = Business.Models.Task.Create(taskFormViewModel.TaskId,
                    taskFormViewModel.TeamId,
                    null,
                    taskFormViewModel.TaskName,
                    taskFormViewModel.TaskStoryPoints,
                    taskFormViewModel.TaskLink,
                    taskFormViewModel.TaskSprintId,
                    taskFormViewModel.TaskMemberId);

                var isOwner = await _accessCheckService.IsOwnerAsync(task.TeamId);

                if (!isOwner)
                {
                    return RedirectToAction("NotOwnerError", new { teamId = taskFormViewModel.TeamId });
                }

                var result = await _manageTasksService.AddTaskAsync(task);

                if (result)
                {
                    return RedirectToAction("GetSprintById", "ManageSprints", new { sprintId = taskFormViewModel.TaskSprintId });
                }
                else
                {
                    return RedirectToAction("AddTaskError", new { teamId = taskFormViewModel.TeamId });
                }
            }

            var teamMembers = await GetAllTeamMembersAsync(taskFormViewModel.TeamId);
            taskFormViewModel.TeamMembers = new List<TeamMemberViewModel>();

            foreach (var teamMember in teamMembers)
            {
                TeamMemberViewModel teamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                taskFormViewModel.TeamMembers.Add(teamMemberViewModel);
            }

            return View(taskFormViewModel);
        }

        [Authorize, NonAction]
        private async Task<bool> AddTaskAsync(Business.Models.Task task)
        {
            if (await _accessCheckService.IsOwnerAsync(task.TeamId))
                return await _manageTasksService.AddTaskAsync(task);
            else return false;
        }

        [Authorize]
        public async Task<IActionResult> AddTaskIntoTeamAsync(int teamId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var teamMembers = await GetAllTeamMembersAsync(teamId);
            var sprints = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(teamId, new DisplayOptions()));

            Business.Models.Task task = Business.Models.Task.Create(teamId, 0, team);

            TaskFormViewModel model = TaskFormViewModel.Create(task, errorMessage, teamMembers, sprints);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTaskIntoTeamAsync(TaskFormViewModel taskFormViewModel)
        {
            if (ModelState.IsValid)
            {

                if (taskFormViewModel.LinkValidation == null && !Regex.IsMatch(taskFormViewModel.TaskLink, 
                    @"^(?:http(s):\/\/)(github\.com\/)|(bitbucket\.org\/)[\w\d\S]+(\/[\w\d\S]+)*$"))
                {
                    return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId, 
                        errorMessage = _localizer["LinkFieldError"] });
                }
                else if(taskFormViewModel.LinkValidation != null && !Regex.IsMatch(taskFormViewModel.TaskLink, 
                    @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$"))
                {
                    return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId, 
                        errorMessage = _localizer["LinkFieldError"] });
                }
                var task = Business.Models.Task.Create(taskFormViewModel.TaskId,
                    taskFormViewModel.TeamId,
                    null,
                    taskFormViewModel.TaskName,
                    taskFormViewModel.TaskStoryPoints,
                    taskFormViewModel.TaskLink,
                    taskFormViewModel.TaskSprintId,
                    taskFormViewModel.TaskMemberId);

                var isOwner = await _accessCheckService.IsOwnerAsync(task.TeamId);

                if (!isOwner)
                {
                    return RedirectToAction("NotOwnerError", new { teamId = taskFormViewModel.TeamId });
                }

                var result = await _manageTasksService.AddTaskAsync(task);

                if (result)
                {
                    return RedirectToAction("GetSprintById", "ManageSprints", new { sprintId = taskFormViewModel.TaskSprintId });
                }
                else
                {
                    return RedirectToAction("AddTaskError", new { teamId = taskFormViewModel.TeamId });
                }
            }
            
            var teamMembers = await GetAllTeamMembersAsync(taskFormViewModel.TeamId);
            var teamSprints = await _manageSprintsService.GetAllSprintsAsync(taskFormViewModel.TeamId, new DisplayOptions());

            taskFormViewModel.TeamMembers = new List<TeamMemberViewModel>();
            foreach (var teamMember in teamMembers)
            {
                TeamMemberViewModel teamMemberViewModel = TeamMemberViewModel.Create(teamMember);
                taskFormViewModel.TeamMembers.Add(teamMemberViewModel);
            }

            taskFormViewModel.Sprints = new List<SprintViewModel>();

            foreach (var teamSprint in teamSprints)
            {
                SprintViewModel sprintViewModel = SprintViewModel.Create(teamSprint, false, 0);
                taskFormViewModel.Sprints.Add(sprintViewModel);
            }

            return View(taskFormViewModel);
        }
    }
}