using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Web.ViewModels.Sprint;

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
            options.SortDirection = SortDirection.ByStatus;

            if (await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                var ienumarableSprints = await _manageSprintsService.GetAllSprintsAsync(teamId, options);
                sprints = ienumarableSprints.ToList();
            }
            else
            {
                return View("ErrorGetAllSprints");
            }

            bool isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(teamId))
            {
                isOwner = true;
            }

            var team = await _manageSprintsService.GetTeam(teamId);
            List<TeamMember> teamMembers = await GetAllTeamMembersAsync(teamId, new DisplayOptions { });
            team.SetTeamMembers(teamMembers);
            SprintAndTeamViewModel sprintViewModel = SprintAndTeamViewModel.Create(null, 
                sprints, team, isOwner, new List<MemberWorkingDays>());

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

            if (isCompleted) task.SetCompleted();

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

            bool isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                isOwner = true;
            }

            var averageStoryPoint = await _manageSprintsService.GetAverageStoryPointAsync(sprint);
            SprintViewModel sprintViewModel = SprintViewModel.Create(sprint, isOwner, averageStoryPoint);

            return View(sprintViewModel);
        }

        [Authorize]
        public async Task<IActionResult> EditSprintAsync(int sprintId, string errorMessage)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, false);
            var team = await _manageSprintsService.GetTeam(sprint.TeamId);
            EditSprintViewModel editSprintViewModel = EditSprintViewModel.Create(sprint, errorMessage, team);

            return View(editSprintViewModel);
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

                var newSprint = Sprint.Create(editSprintViewModel.SprintId,
                    editSprintViewModel.TeamId,
                    Team.Create(currentSprint.Team.TeamOwner, currentSprint.Team.TeamName),
                    editSprintViewModel.SprintName,
                    editSprintViewModel.SprintDaysInSprint,
                    editSprintViewModel.SprintStorePointInHours,
                    editSprintViewModel.Status);
                
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
            ViewBag.ErrorMessage = errorMessage;

            var tasks = (await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions()))
                .Where(task => task.SprintId == null && task.Completed == false);

            Sprint sprint = Sprint.Create(teamId, tasks.ToList());

            bool isOwner = false;
            if (await _accessCheckService.IsOwnerAsync(teamId))
            {
                isOwner = true;
            }

            SprintViewModel sprintViewModel = SprintViewModel.Create(sprint, isOwner, 0);

            return View(sprintViewModel);
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

                var newSprint = Sprint.Create(sprintViewModel.TeamId, sprintViewModel.Name,
                    sprintViewModel.DaysInSprint,
                    sprintViewModel.StoryPointInHours,
                    sprintViewModel.Status);

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
                    var task = Business.Models.Task.Create(currentTask.Id,
                        currentTask.TeamId,
                        Team.Create(currentSprint.Team.Id, 
                            currentSprint.Team.TeamOwner, 
                            currentSprint.Team.TeamName, 
                            new List<TeamMember>()),
                        currentTask.Name,
                        currentTask.StoryPoints,
                        currentTask.Link, sprintId,
                        currentTask.MemberId);

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
            if (result) return RedirectToAction("AllSprints", new { teamId = teamId });
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
                    ErrorChangeStatusViewModel errorChangeStatusViewModel = ErrorChangeStatusViewModel.Create(currentSprint.Name,
                        _localizer["ActiveFieldError"]);
                    return RedirectToAction("ErrorChangeStatus", errorChangeStatusViewModel);
                }

                currentSprint.SetStatus(status);
                var result = await EditSprintAsync(currentSprint);

                if (result)
                {
                    return RedirectToAction("AllSprints", new { teamId = currentSprintTeam.Id });
                }
                else
                {
                    ErrorChangeStatusViewModel errorChangeStatusViewModel = ErrorChangeStatusViewModel.Create(currentSprint.Name,
                    _localizer["ErrorWhileEdit"]);
                    return RedirectToAction("ErrorChangeStatus", errorChangeStatusViewModel);
                }
            }
            else
            {
                ErrorChangeStatusViewModel errorChangeStatusViewModel = ErrorChangeStatusViewModel.Create(currentSprint.Name,
                    _localizer["CantChangeStatus"]);
                return RedirectToAction("ErrorChangeStatus", errorChangeStatusViewModel);
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
    }
}
