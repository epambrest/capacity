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
            var tasks = await _manageTasksService.GetAllTasksForTeamAsync(teamId, options);
            var team = await _manageTeamsService.GetTeamAsync(teamId);
            var sprints = await _manageSprintsService.GetAllSprintsAsync(teamId, options);
            var members = await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            var tasksForTeamViewModel = new AllTasksForTeamViewModel()
            {
                TeamId = team.Id,
                TeamName = team.TeamName,
                Tasks = new List<TaskViewModel>(),
                Sprints = new List<SprintViewModel>(),
                Members = new List<TeamMemberViewModel>()
            };

            sprints.ToList().ForEach(t => tasksForTeamViewModel.Sprints.Add(new SprintViewModel()
            {
                Id = t.Id,
                DaysInSprint = t.DaysInSprint,
                Status = t.Status,
                Name = t.Name,
                StoryPointInHours = t.StoryPointInHours,
                TeamId = t.TeamId
            }));

            if (tasksForTeamViewModel.Sprints.Count > 1 && tasksForTeamViewModel.Sprints[1].Status == PossibleStatuses.ActiveStatus)
            {
                var swapElem = tasksForTeamViewModel.Sprints[0];
                tasksForTeamViewModel.Sprints[0] = tasksForTeamViewModel.Sprints[1];
                tasksForTeamViewModel.Sprints[1] = swapElem;
            }

            tasks.ToList().ForEach(t => tasksForTeamViewModel.Tasks.Add(new TaskViewModel()
            {
                Id = t.Id,
                Link = t.Link,
                Name = t.Name,
                StoryPoints = t.StoryPoints,
                SprintId = t.SprintId,
                TeamMember = t.MemberId != null ? new TeamMemberViewModel() { Member = t.TeamMember.Member } : null,
                MemberId = t.MemberId,
                Completed = t.Completed
            }));

            members.ForEach(t => tasksForTeamViewModel.Members.Add(new TeamMemberViewModel()
            {
                MemberId = t.MemberId,
                Member = t.Member
            }));

            if (await _accessCheckService.IsOwnerAsync(teamId))
            {
                tasksForTeamViewModel.IsOwner = true;
            }
            else
            {
                tasksForTeamViewModel.IsOwner = false;
            }

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
            var taskViewModel = new TaskViewModel()
            {
                Id=task.Id,
                Link = task.Link,
                Name = task.Name,
                TeamId = task.TeamId,
                StoryPoints = task.StoryPoints,
                TeamMember = task.MemberId != null ? new TeamMemberViewModel() { Member = task.TeamMember.Member } : null
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
        public async Task<IActionResult> EditTaskAsync(int taskId, string errorMessage)
        {
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var team = await _manageSprintsService.GetTeam(task.TeamId);
            var teamMembers = await GetAllTeamMembersAsync(task.TeamId);
            var taskMemberName = teamMembers.FirstOrDefault(t => t.Id == task.MemberId).Member.Email;

            TaskFormViewModel model = new TaskFormViewModel
            {
                TeamId = task.TeamId,
                TaskId = task.Id,
                TaskSprintId = task.SprintId.GetValueOrDefault(),
                TeamName = team.TeamName,
                TaskName = task.Name,
                TaskLink = task.Link,
                TaskStoryPoints = task.StoryPoints,
                TaskMemberId = task.MemberId,
                ErrorMessage = errorMessage,
                TaskMemberName = taskMemberName,
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
        public async Task<IActionResult> EditTaskAsync(TaskFormViewModel taskViewModel)
        {
            if (ModelState.IsValid)
            {
                var task = new Data.Models.Task
                {
                    Id = taskViewModel.TaskId,
                    TeamId = taskViewModel.TeamId,
                    Name = taskViewModel.TaskName,
                    StoryPoints = taskViewModel.TaskStoryPoints,
                    Link = taskViewModel.TaskLink,
                    SprintId = taskViewModel.TaskSprintId,
                    MemberId = taskViewModel.TaskMemberId
                };

                var currentTask = await _manageTasksService.GetTaskByIdAsync(taskViewModel.TaskId);

                if (currentTask.Name== taskViewModel.TaskName && 
                    currentTask.Link == taskViewModel.TaskLink &&
                    currentTask.StoryPoints == taskViewModel.TaskStoryPoints&&
                    currentTask.MemberId== taskViewModel.TaskMemberId
                    )
                {
                    return RedirectToAction("EditTask", new { teamId = taskViewModel.TeamId, taskId = taskViewModel.TaskId, errorMessage = _localizer["HasntAnyChange"] });
                }

                var result = await EditTaskAsync(task);

                if (result) return RedirectToAction("AllTasksForTeam", new { teamId = taskViewModel.TeamId });
                else return RedirectToAction("NotOwnerError", new { teamId = taskViewModel.TaskId });
            }

            var teamMembers = await GetAllTeamMembersAsync(taskViewModel.TeamId);

            taskViewModel.TeamMembers = new List<TeamMemberViewModel>();
            teamMembers.ForEach(t => taskViewModel.TeamMembers.Add(new TeamMemberViewModel
            {
                Member = t.Member,
                Id = t.Id
            }));

            return View(taskViewModel);
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
        public async Task<IActionResult> GetResultTeamMember(int sprintId, int teamMemberId = 1)
        {
            var completedSprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            var members = await GetAllTeamMembersAsync(completedSprint.TeamId);
            var currentMember = members.FirstOrDefault(member => member.Id == teamMemberId);
            
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

            var allWorkingDaysForSprint = await _manageMemberWorkingDaysService.GetAllWorkingDaysForSprintAsync(sprintId);

            var memberWorkingDays = allWorkingDaysForSprint.Where(i => i.MemberId == teamMemberId).FirstOrDefault();

            if (memberWorkingDays == null)
            {
                RedirectToAction("GetResultError", new { errorMessage = "Can't get count of working days in the current team member" });
            }

            var teamMemberTotalSp = spCompletedTasks + spUnCompletedTasks;
            var storyPointsInDay = Convert.ToDouble(teamMemberTotalSp) / Convert.ToDouble(memberWorkingDays.WorkingDays);


            var resultsTasksForMemberViewModel = new ResultsTasksForMemberViewModel()
            {
                TeamMemberId = currentMember.Id,
                TeamId = completedSprint.TeamId,
                CompletedSprintId = completedSprint.Id,
                TeamMemberEmail = currentMember.Member.Email,
                SprintName = completedSprint.Name,
                Tasks = new List<TaskViewModel>(),
                TeamMembers = new List<TeamMemberViewModel>(),
                TotalStoryPoints = totalStoryPoints,
                QuantityСompletedTasks = quantityСompletedTasks,
                QuantityUnСompletedTasks = quantityUnСompletedTasks,
                SpСompletedTasks = spCompletedTasks,
                SpUnСompletedTasks = spUnCompletedTasks,
                StoryPointsInDay = storyPointsInDay
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
                TeamId = t.TeamMember.TeamId.GetValueOrDefault(),
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
            if (ModelState.IsValid)
            {
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
            teamMembers.ForEach(t => taskFormViewModel.TeamMembers.Add(new TeamMemberViewModel()
            {
                Member = t.Member,
                Id = t.Id
            }));

            return View(taskFormViewModel);
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
            if (ModelState.IsValid)
            {
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
            taskFormViewModel.Sprints = new List<SprintViewModel>().ToList();

            teamMembers.ForEach(t => taskFormViewModel.TeamMembers.Add(new TeamMemberViewModel()
            {
                Member = t.Member,
                Id = t.Id
            }));

            teamSprints.ToList().ForEach(t => taskFormViewModel.Sprints.Add(new SprintViewModel()
            {
                Name = t.Name,
                Id = t.Id
            }));

            return View(taskFormViewModel);

        }
    }
}