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
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IStringLocalizer<ManageTasksController> _localizer;

        public ManageTasksController(IManageTasksService manageTasksService, IAccessCheckService accessCheckService,
            IManageTeamsService manageTeamsService, IManageTeamsMembersService manageTeamsMembersService,
            IManageSprintsService manageSprintsService, IStringLocalizer<ManageTasksController> localizer)
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
            tasksForTeamViewModel.Tasks = new List<TaskViewModel>();
            tasks.ToList().ForEach(t=> tasksForTeamViewModel.Tasks.Add(new TaskViewModel()
            {
                Id = t.Id,
                Link = t.Link,
                Name = t.Name,
                TeamMember = new TeamMemberViewModel(){Member = t.TeamMember.Member}
            }));
            tasksForTeamViewModel.TeamId = team.Id;
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
            var taskViewModel = new TaskViewModel()
            {
                Id=task.Id,
                Link = task.Link,
                Name = task.Name,
                TeamId = task.TeamId,
                TeamMember = new TeamMemberViewModel(){Member = task.TeamMember.Member}
            };

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
        public async Task<IActionResult> EditTaskAsync(int teamId, int taskId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var teamMembers = await GetAllTeamMembersAsync(teamId);

            TaskFormViewModel model = new TaskFormViewModel
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
                TeamMembers = new List<TeamMemberViewModel>()
            };

            teamMembers.ForEach(t=>model.TeamMembers.Add(new TeamMemberViewModel()
            {
                Member = t.Member,
                Id = t.Id
            }));

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
            else return RedirectToAction("NotOwnerError", new { teamId = teamId });

        }

        [Authorize, NonAction]
        private async Task<bool> EditTaskAsync(Data.Models.Task task)
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
        private Dictionary<string, int> GetTasksStoryPoints(List<Teams.Data.Models.Task> tasks)
        {
            Dictionary<string, int> tasksSp = new Dictionary<string, int>();
            int spCompletedTasks = 0;
            int spUnCompletedTasks = 0;
            foreach (var task in tasks)
            {
                if(task.Completed == true)
                {
                    spCompletedTasks += task.StoryPoints;
                }
                else
                {
                    spUnCompletedTasks += task.StoryPoints;
                }
            }
            tasksSp.Add("spCompletedTasks", spCompletedTasks);
            tasksSp.Add("spUnCompletedTasks", spUnCompletedTasks);
            return tasksSp;
        }

        [Authorize]
        public IActionResult GetResultError(string errorMessage)
        {
                        
            GetResultErrorViewModel getResultErrorViewModel = new GetResultErrorViewModel
            {
                ErrorMessage = errorMessage,
            };
            return View(getResultErrorViewModel);
        }

        [Authorize]
        public async Task<IActionResult> GetResultTeamMember(int sprintId, int teamId, int teamMemberId = 1)
        {
            var members = await GetAllTeamMembersAsync(teamId);
            var currentMember = members.FirstOrDefault(member => member.Id == teamMemberId);
            var completedSprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            
            if (completedSprint == null || currentMember == null || completedSprint == null)
                return RedirectToAction("GetResultError", new { errorMessage = _localizer["CouldntGetData"] });
            if (completedSprint.Status != 2)
                return RedirectToAction("GetResultError", new { errorMessage = _localizer["StatusIsNotComplete"] });

            var allMemberTasks = completedSprint.Tasks.Where(t => t.MemberId == teamMemberId).ToList();
            var allSprintTasks = completedSprint.Tasks.ToList();

            if(allMemberTasks == null)
                return RedirectToAction("GetResultError", new { errorMessage = _localizer["TasksNotExists"] });

            Dictionary<string, int> tasksSp = GetTasksStoryPoints(allMemberTasks);
            int spCompletedTasks = tasksSp.GetValueOrDefault("spCompletedTasks");
            int spUnCompletedTasks = tasksSp.GetValueOrDefault("spUnCompletedTasks");
            int totalStoryPoints = 0;
            int quantityСompletedTasks = allMemberTasks.Count(t => t.Completed == true);
            int quantityUnСompletedTasks = allMemberTasks.Count(t => t.Completed == false);

            allMemberTasks.ForEach(t => totalStoryPoints += t.StoryPoints);

            var resultsTasksForMemberViewModel = new ResultsTasksForMemberViewModel()
            {
                TeamMemberId = currentMember.Id,
                TeamId = teamId,
                CompletedSprintId = completedSprint.Id,
                TeamMemberEmail = currentMember.Member.Email,
                SprintName = completedSprint.Name,
                Tasks = new List<TaskViewModel>(),
                TeamMembers = new List<TeamMemberViewModel>(),
                TotalStoryPoints = totalStoryPoints,
                QuantityСompletedTasks = quantityСompletedTasks,
                QuantityUnСompletedTasks = quantityUnСompletedTasks,
                SpСompletedTasks = spCompletedTasks,
                SpUnСompletedTasks = spUnCompletedTasks
            };

            allMemberTasks.ForEach(t => resultsTasksForMemberViewModel.Tasks.Add(new TaskViewModel()
            {
                TeamMember = new TeamMemberViewModel() { Member = t.TeamMember.Member },
                Name = t.Name,
                StoryPoints = t.StoryPoints,
                Id = t.Id,
                Link = t.Link,
                Completed = t.Completed
            }
            ));

            allSprintTasks.ForEach(t => resultsTasksForMemberViewModel.TeamMembers.Add(new TeamMemberViewModel()
            {
                Id = t.TeamMember.Id,
                TeamId = t.TeamMember.TeamId,
                MemberId = t.TeamMember.Id.ToString(),
                Member = t.TeamMember.Member
            }
            ));

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

            TaskFormViewModel model = new TaskFormViewModel
            {
                TeamId = teamId,
                TaskSprintId = sprintId,
                TeamName = team.TeamName,
                ErrorMessage = errorMessage,
                TeamMembers = new List<TeamMemberViewModel>()
            };
            teamMembers.ForEach(t=>model.TeamMembers.Add(new TeamMemberViewModel()
            {
                Member = t.Member,
                Id = t.Id
            }));
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTaskAsync(TaskFormViewModel taskFormViewModel)
        {
            if (string.IsNullOrEmpty(taskFormViewModel.TaskName))
            {
                return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, taskId = taskFormViewModel.TaskId, errorMessage = _localizer["NameFieldError"] });
            }
            if (string.IsNullOrEmpty(taskFormViewModel.TaskLink) || !Regex.IsMatch(taskFormViewModel.TaskLink, (@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")))
            {
                return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, taskId = taskFormViewModel.TaskId, errorMessage = _localizer["LinkFieldError"] });
            }
            if (taskFormViewModel.TaskStoryPoints <= 0)
            {
                return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, taskId = taskFormViewModel.TaskId, errorMessage = _localizer["PointsFieldError"] });
            }
            if (taskFormViewModel.TaskMemberId <= 0)
            {
                return RedirectToAction("AddTask", new { teamId = taskFormViewModel.TeamId, taskId = taskFormViewModel.TaskId, errorMessage = _localizer["MemberFieldError"] });
            }

            var task = new Data.Models.Task
            {
                Id = taskFormViewModel.TaskId,
                TeamId = taskFormViewModel.TeamId,
                Name = taskFormViewModel.TaskName,
                StoryPoints = taskFormViewModel.TaskStoryPoints,
                Link = taskFormViewModel.TaskLink,
                SprintId = taskFormViewModel.TaskSprintId,
                MemberId = taskFormViewModel.TaskMemberId
            };
            var result = await AddTaskAsync(task);

            if (result) return RedirectToAction("GetSprintById", "ManageSprints", new { sprintId = taskFormViewModel.TaskSprintId });
            else return RedirectToAction("NotOwnerError", new { teamId = taskFormViewModel.TeamId });

        }

        [Authorize, NonAction]
        private async Task<bool> AddTaskAsync(Data.Models.Task task)
        {
            if (await _accessCheckService.IsOwnerAsync(task.TeamId))
            {
                return await _manageTasksService.AddTaskAsync(task);
            }
            else return false;
        }

        [Authorize]
        public async Task<IActionResult> AddTaskIntoTeamAsync(int teamId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var teamMembers = await GetAllTeamMembersAsync(teamId);
            var sprints = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(teamId, new DisplayOptions()));

            TaskFormViewModel model = new TaskFormViewModel
            {
                TeamId = teamId,
                Sprints = new List<SprintViewModel>(),
                TeamName = team.TeamName,
                ErrorMessage = errorMessage,
                TeamMembers = new List<TeamMemberViewModel>()
            };
            sprints.ForEach(t=>model.Sprints.Add(new SprintViewModel()
            {
                Id = t.Id,
                Name = t.Name
            }));
            teamMembers.ForEach(t=>model.TeamMembers.Add(new TeamMemberViewModel()
            {
                Id = t.Id,
                Member = t.Member
            }));
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTaskIntoTeamAsync(TaskFormViewModel taskFormViewModel)
        {
            if (string.IsNullOrEmpty(taskFormViewModel.TaskName))
            {
                return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId, errorMessage = _localizer["NameFieldError"] });
            }
            if (string.IsNullOrEmpty(taskFormViewModel.TaskLink) || !Regex.IsMatch(taskFormViewModel.TaskLink, (@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")))
            {
                return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId,errorMessage = _localizer["LinkFieldError"] });
            }
            if (taskFormViewModel.TaskStoryPoints <= 0)
            {
                return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId,errorMessage = _localizer["PointsFieldError"] });
            }
            if (taskFormViewModel.TaskMemberId <= 0)
            {
                return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId, errorMessage = _localizer["MemberFieldError"] });
            }
            if (taskFormViewModel.TaskSprintId <= 0)
            {
                return RedirectToAction("AddTaskIntoTeam", new { teamId = taskFormViewModel.TeamId, errorMessage = _localizer["SprintFieldError"] });
            }

            var task = new Data.Models.Task
            {
                Id = taskFormViewModel.TaskId,
                TeamId = taskFormViewModel.TeamId,
                Name = taskFormViewModel.TaskName,
                StoryPoints = taskFormViewModel.TaskStoryPoints,
                Link = taskFormViewModel.TaskLink,
                SprintId = taskFormViewModel.TaskSprintId,
                MemberId = taskFormViewModel.TaskMemberId
            };
            var result = await AddTaskAsync(task);

            if (result) return RedirectToAction( "AllTasksForTeam", new { teamId = taskFormViewModel.TeamId });
            else return RedirectToAction("NotOwnerError", new { teamId = taskFormViewModel.TeamId });

        }
    }
}