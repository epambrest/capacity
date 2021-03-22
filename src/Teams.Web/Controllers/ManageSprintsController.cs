using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Web.ViewModels;
using Teams.Web.ViewModels.Sprint;
using Teams.Web.ViewModels.Task;
using Teams.Web.ViewModels.Team;
using Teams.Web.ViewModels.TeamMember;

namespace Teams.Web.Controllers
{
    public class ManageSprintsController : Controller
    {
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IManageTasksService _manageTasksService;
        private readonly IStringLocalizer<ManageSprintsController> _localizer;


        public ManageSprintsController(IManageSprintsService manageSprintsService, IAccessCheckService accessCheckService,
            IManageTeamsService manageTeamsService, IManageTeamsMembersService manageTeamsMembersService,
            IManageTasksService manageTasksService, IStringLocalizer<ManageSprintsController> localizer)
        {
            _manageSprintsService = manageSprintsService;
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
            _manageTeamsMembersService = manageTeamsMembersService;
            _manageTasksService = manageTasksService;
            _localizer = localizer;
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
            {
                return View("ErrorGetAllSprints");
            }

            List<TeamMember> teamMembers = await GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            var sprintViewModel = new SprintAndTeamViewModel
            {
                Sprints = new List<SprintViewModel>()
            };

            if (await _accessCheckService.IsOwnerAsync(teamId))
            {
                sprintViewModel.IsOwner = true;
            }
            else
            {
                sprintViewModel.IsOwner = false;
            }
            sprints.OrderBy(s => s.Status).ToList().ForEach(t => sprintViewModel.Sprints.Add(new SprintViewModel()
            {
                Id = t.Id,
                DaysInSprint = t.DaysInSprint,
                Status = t.Status,
                Name = t.Name,
                StoryPointInHours = t.StoryPointInHours,
                TeamId = t.TeamId
            }
            ));

            if (sprintViewModel.Sprints.Count > 1 && sprintViewModel.Sprints[1].Status == PossibleStatuses.ActiveStatus)
            {
                var swapElem = sprintViewModel.Sprints[0];
                sprintViewModel.Sprints[0] = sprintViewModel.Sprints[1];
                sprintViewModel.Sprints[1] = swapElem;
            }

            var team = await _manageSprintsService.GetTeam(teamId);

            sprintViewModel.Team = new TeamViewModel()
            {
                Id = team.Id,
                Owner = team.Owner,
                TeamName = team.TeamName,
                TeamMembers = new List<TeamMemberViewModel>()
            };

            teamMembers.ForEach(t => sprintViewModel.Team.TeamMembers.Add(new TeamMemberViewModel()
            {
                Member = t.Member,
                MemberId = t.MemberId
            }));

            return View(sprintViewModel);
        }

        [Authorize, NonAction]
        private async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                RedirectToAction("Error");
            }
            return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, options);
        }

        [Authorize]
        public async Task<IActionResult> CompleteTaskInSprint(int taskId, bool isCompleted, string redirectPath = "GetSprintById")
        {
            var task = await _manageTasksService.GetTaskByIdAsync(taskId);
            var sprint = await _manageSprintsService.GetSprintAsync(task.SprintId.GetValueOrDefault(), false);

            if (sprint == null)
            {
                return View("ErrorSprint");
            }
            if (sprint.Status != PossibleStatuses.ActiveStatus)
            {
                return RedirectToAction("ErrorTask", new { Cause = "SprintNotActive", task.SprintId });
            }
            if (task.MemberId == null)
                return RedirectToAction("ErrorTask", new { Cause = "NotAssigned", task.SprintId });

            task.Completed = isCompleted;

            var result = await _manageTasksService.EditTaskAsync(task);

            if (result)
            {
                if(redirectPath == "GetSprintById")
                    return RedirectToAction(redirectPath, new { sprintId = task.SprintId });
                else
                    return RedirectToAction(redirectPath, "ManageTasks", new { teamId = task.TeamId, options = new DisplayOptions { } });
            }
            else
            {
                task.Completed = false;
                return View("Error");
            }
        }

        [Authorize]
        public async Task<IActionResult> GetSprintById(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, true);

            if (sprint == null)
            {
                return View("ErrorGetAllSprints");
            }

            var sprintViewModel = new SprintViewModel()
            {
                DaysInSprint = sprint.DaysInSprint,
                Id = sprint.Id,
                Tasks = CreateTaskViewModels(sprint.Tasks),
                Status = sprint.Status,
                Name = sprint.Name,
                StoryPointInHours = sprint.StoryPointInHours,
                TeamId = sprint.TeamId
            };

            sprintViewModel.TotalStoryPoint = sprint.Tasks.Count > 0 ? sprint.Tasks.Sum(t => t.StoryPoints) : 0;
            sprintViewModel.AverageStoryPoint = await _manageSprintsService.GetAverageStoryPointAsync(sprint);

            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                sprintViewModel.IsOwner = true;
            }
            else
            {
                sprintViewModel.IsOwner = false;
            }


            return View(sprintViewModel);
        }

        [Authorize]
        public async Task<IActionResult> EditSprintAsync(int sprintId, string errorMessage)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, false);
            var team = await _manageSprintsService.GetTeam(sprint.TeamId);

            EditSprintViewModel model = new EditSprintViewModel
            {
                TeamId = team.Id,
                TeamName = team.TeamName,
                SprintId = sprint.Id,
                SprintName = sprint.Name,
                SprintDaysInSprint = sprint.DaysInSprint,
                SprintStorePointInHours = sprint.StoryPointInHours,
                ErrorMessage = errorMessage,
                Status = sprint.Status
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditSprintAsync(EditSprintViewModel editSprintViewModel)
        {
            if (!ModelState.IsValid)
                return View(editSprintViewModel);

            var sprints = await _manageSprintsService.GetAllSprintsAsync(editSprintViewModel.TeamId, new DisplayOptions());
            var currentSprint = sprints.FirstOrDefault(i => i.Id == editSprintViewModel.SprintId);

            if (currentSprint.Name == editSprintViewModel.SprintName &&
                currentSprint.DaysInSprint == editSprintViewModel.SprintDaysInSprint &&
                currentSprint.StoryPointInHours == editSprintViewModel.SprintStorePointInHours &&
                currentSprint.Status == editSprintViewModel.Status)
            {
                return RedirectToAction("EditSprint", new
                {
                    teamId = editSprintViewModel.TeamId,
                    sprintId = editSprintViewModel.SprintId,
                    errorMessage = _localizer["HasntAnyChange"]
                });
            }

            var createdSprint = sprints.FirstOrDefault(i => i.Status == PossibleStatuses.CreatedStatus);

            if (IsStatusCanBeChanged(currentSprint.Status, editSprintViewModel.Status))
            {
                var activeSprint = sprints.FirstOrDefault(i => i.Status == PossibleStatuses.ActiveStatus);

                if (activeSprint != null &&
                    editSprintViewModel.Status == PossibleStatuses.ActiveStatus &&
                    activeSprint.Id != currentSprint.Id)
                {
                    return RedirectToAction("EditSprint", new
                    {
                        teamId = editSprintViewModel.TeamId,
                        sprintId = editSprintViewModel.SprintId,
                        errorMessage = _localizer["ActiveFieldError"]
                    });
                }

                var newSprint = new Sprint
                {
                    Id = editSprintViewModel.SprintId,
                    TeamId = editSprintViewModel.TeamId,
                    Name = editSprintViewModel.SprintName,
                    DaysInSprint = editSprintViewModel.SprintDaysInSprint,
                    StoryPointInHours = editSprintViewModel.SprintStorePointInHours,
                    Status = editSprintViewModel.Status
                };
                
                if (await EditSprintAsync(newSprint))
                    return RedirectToAction("AllSprints", new { teamId = editSprintViewModel.TeamId });
                else
                    return RedirectToAction("NotOwnerError", new { teamId = editSprintViewModel.TeamId });
            }
            return RedirectToAction("EditSprint", new
            {
                teamId = editSprintViewModel.TeamId,
                sprintId = editSprintViewModel.SprintId,
                errorMessage = _localizer["CantChangeStatus"]
            });
        }

        [Authorize]
        public async Task<IActionResult> AddSprintAsync(int teamId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var sprintViewModel = new SprintViewModel() { Id = teamId, Name = team.TeamName, Tasks = new List<TaskViewModel>() };
            ViewBag.ErrorMessage = errorMessage;

            var tasks = (await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions())).Where(task => task.SprintId == null && task.Completed == false);
            tasks.ToList().ForEach(t => sprintViewModel.Tasks.Add(new TaskViewModel()
            {
                Name = t.Name,
                Id = t.Id
            }
            ));

            sprintViewModel.SelectTasks = await SelectTasks(teamId);

            return View(sprintViewModel);
        }

        [Authorize, NonAction]
        private async Task<List<SelectListItem>> SelectTasks(int teamId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var tasks = (await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions())).Where(task => task.SprintId == null && task.Completed == false);
            foreach (var task in tasks)
            {
                items.Add(new SelectListItem(task.Name, task.Id.ToString()));
            }

            return items;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSprintAsync(SprintViewModel sprintViewModel)
        {

            if (ModelState.IsValid)
            {
                sprintViewModel.Status = PossibleStatuses.CreatedStatus;
                var sprints = await _manageSprintsService.GetAllSprintsAsync(sprintViewModel.TeamId, new DisplayOptions());
                var activeSprint = sprints.FirstOrDefault(i => i.Status == PossibleStatuses.ActiveStatus);

                if (activeSprint != null && sprintViewModel.Status == PossibleStatuses.ActiveStatus)
                {
                    return RedirectToAction("AddSprint", new { teamId = sprintViewModel.TeamId, errorMessage = _localizer["ActiveFieldError"] });
                }

                var createdSprint = sprints.FirstOrDefault(i => i.Status == PossibleStatuses.CreatedStatus);

                if (createdSprint != null && sprintViewModel.Status == PossibleStatuses.CreatedStatus)
                    return RedirectToAction("AddSprint", new { teamId = sprintViewModel.TeamId, errorMessage = _localizer["СreatedSprintExist"] });

                var sameSprint = sprints.FirstOrDefault(i => i.Name == sprintViewModel.Name);

                if (sameSprint != null)
                    return RedirectToAction("AddSprint", new { teamId = sprintViewModel.TeamId, errorMessage = _localizer["SprintWithSameName"] });

                var newSprint = new Sprint
                {
                    TeamId = sprintViewModel.TeamId,
                    Name = sprintViewModel.Name,
                    DaysInSprint = sprintViewModel.DaysInSprint,
                    StoryPointInHours = sprintViewModel.StoryPointInHours,
                    Status = sprintViewModel.Status
                };

                var result = await AddSprintAsync(newSprint);


                if (result && sprintViewModel.SelectTasksIds != null &&
                    await UpdateTasks(sprintViewModel.SelectTasksIds, newSprint.Id))
                {
                    return RedirectToAction("AllSprints", new { teamId = sprintViewModel.TeamId });
                }
                else
                {
                    return RedirectToAction("NotOwnerError", new { teamId = sprintViewModel.TeamId });
                }
            }
            else
            {
                return View(sprintViewModel);
            }
        }

        public IActionResult NotOwnerError(int teamId)
        {
            ViewData["Error"] = _localizer["Error"];
            ViewData["Cause"] = _localizer["NotOwner"];
            return View(teamId);
        }

        public IActionResult Error()
        {
            ViewData["Error"] = _localizer["Error"];
            return View();
        }
        public IActionResult ErrorTask(string Cause, int sprintId)
        {
            ViewData["Error"] = _localizer["Error"];
            ViewData["Cause"] = _localizer[Cause];
            return View(sprintId);
        }
        public IActionResult ErrorSprint()
        {
            ViewData["Error"] = _localizer["Error"];
            ViewData["Sprint"] = _localizer["SprintNull"];
            return View();
        }

        [Authorize, NonAction]
        private async Task<bool> AddSprintAsync(Sprint sprint)
        {
            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                return await _manageSprintsService.AddSprintAsync(sprint);
            }
            else return false;
        }

        [Authorize, NonAction]
        private async Task<bool> EditSprintAsync(Sprint sprint)
        {
            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                return await _manageSprintsService.EditSprintAsync(sprint);
            }
            else return false;
        }

        [Authorize, NonAction]
        private async Task<bool> UpdateTasks(int[] selectedTasksId, int sprintId)
        {
            var currentSprint = await _manageSprintsService.GetSprintAsync(sprintId, true);
            var isOwner = await _accessCheckService.IsOwnerAsync(currentSprint.TeamId);
            if (isOwner)
            {
                foreach (var selectedTaskId in selectedTasksId)
                {
                    var currentTask = await _manageTasksService.GetTaskByIdAsync(selectedTaskId);
                    var task = new Data.Models.Task
                    {
                        Id = currentTask.Id,
                        TeamId = currentTask.TeamId,
                        Name = currentTask.Name,
                        StoryPoints = currentTask.StoryPoints,
                        Link = currentTask.Link,
                        SprintId = sprintId,
                        MemberId = currentTask.MemberId
                    };
                    await _manageTasksService.EditTaskAsync(task);
                }
                return true;
            }
            return false;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Remove(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, false);
            var teamId = sprint.TeamId;
            var result = await _manageSprintsService.RemoveAsync(sprintId);
            if (result)
                return RedirectToAction("AllSprints", new { teamId = teamId });
            return RedirectToAction("ErrorRemove");
        }

        private bool IsStatusCanBeChanged(int curStatus, int nextStatus)
        {
            List<int> statuses = new List<int>() { PossibleStatuses.CreatedStatus, PossibleStatuses.ActiveStatus, PossibleStatuses.CompletedStatus };

            if (!statuses.Contains(curStatus) || !statuses.Contains(nextStatus))
                return false;
            else if ((curStatus != PossibleStatuses.CompletedStatus && curStatus + 1 == nextStatus) || curStatus == nextStatus)
                return true;
            else
                return false;
        }

        [Authorize]
        public async Task<IActionResult> ChangeStatusSprint(int sprintId, int status)
        {
            var currentSprint = await _manageSprintsService.GetSprintAsync(sprintId, false);
            var currentSprintTeam = await _manageSprintsService.GetTeam(currentSprint.TeamId);
            var sprints = await _manageSprintsService.GetAllSprintsAsync(currentSprintTeam.Id, new DisplayOptions());

            if (IsStatusCanBeChanged(currentSprint.Status, status))
            {
                var activeSprint = sprints.FirstOrDefault(i => i.Status == PossibleStatuses.ActiveStatus);

                if (activeSprint != null &&
                    status == PossibleStatuses.ActiveStatus &&
                    activeSprint.Id != currentSprint.Id)
                {
                    return RedirectToAction("ErrorChangeStatus", new
                    {
                        SprintName = currentSprint.Name,
                        ErrorMessage = _localizer["ActiveFieldError"]
                    });
                }

                currentSprint.Status = status;
                var result = await EditSprintAsync(currentSprint);

                if (result)
                {
                    return RedirectToAction("AllSprints", new { teamId = currentSprintTeam.Id });
                }
                else
                {
                    return RedirectToAction("ErrorChangeStatus", new
                    {
                        SprintName = currentSprint.Name,
                        ErrorMessage = _localizer["ErrorWhileEdit"]
                    });
                }
            }
            else
            {
                return RedirectToAction("ErrorChangeStatus", new
                {
                    SprintName = currentSprint.Name,
                    ErrorMessage = _localizer["CantChangeStatus"]
                });
            }
        }

        public IActionResult ErrorChangeStatus(ErrorChangeStatusViewModel errorChangeStatusViewModel)
        {
            return View(errorChangeStatusViewModel);
        }

        public IActionResult ErrorRemove()
        {
            return View();
        }

        [Authorize, NonAction]
        private List<TaskViewModel> CreateTaskViewModels(ICollection<Data.Models.Task> tasks)
        {
            var tasksList = new List<TaskViewModel>();
            tasks.ToList().ForEach(t => tasksList.Add(new TaskViewModel()
            {
                TeamMember = t.MemberId != null ? new TeamMemberViewModel() { Member = t.TeamMember.Member, MemberId = t.MemberId.ToString() } : null,
                Name = t.Name,
                StoryPoints = t.StoryPoints,
                Id = t.Id,
                Link = t.Link,
                Completed = t.Completed
            }
            ));
            return tasksList;
        }
    }
}
