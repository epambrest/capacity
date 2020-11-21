using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teams.Models;
using Teams.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;

namespace Teams.Controllers
{
    public class ManageSprintsController : Controller
    {
        private readonly IManageSprintsService _manageSprintsService;
        private readonly IAccessCheckService _accessCheckService;
        private readonly IManageTeamsService _manageTeamsService;
        private readonly IManageTeamsMembersService _manageTeamsMembersService;
        private readonly IStringLocalizer<ManageSprintsController> _localizer;


        public ManageSprintsController(IManageSprintsService manageSprintsService, IAccessCheckService accessCheckService, IManageTeamsService manageTeamsService, IManageTeamsMembersService manageTeamsMembersService, IStringLocalizer<ManageSprintsController> localizer)
        {
            _manageSprintsService = manageSprintsService;
            _manageTeamsService = manageTeamsService;
            _accessCheckService = accessCheckService;
            _manageTeamsMembersService = manageTeamsMembersService;
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
                return View("ErrorGetAllSprints");

            var team = await _manageSprintsService.GetTeam(teamId);

            if (await _accessCheckService.IsOwnerAsync(teamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            ViewData["DaysInSprint"] = _localizer["DaysInSprint"];
            ViewData["StoryPointInHours"] = _localizer["StoryPointInHours"];
            ViewData["NameOfSprint"] = _localizer["NameOfSprint"];
            ViewData["MemberEmail"] = _localizer["MemberEmail"];
            ViewData["Owner"] = _localizer["Owner"];
            ViewData["Member"] = _localizer["Member"];
            ViewData["AddMember"] = _localizer["AddMember"];
            ViewData["RemoveMember"] = _localizer["RemoveMember"];

            ViewBag.TeamId = teamId;
            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamOwner = team.Owner.Email;
            List<TeamMember> teamMembers = await GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            CombinedModel combinedModel = new CombinedModel { Sprints = sprints, TeamMembers = teamMembers };
            return View(combinedModel);
        }

        [Authorize, NonAction]
        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int teamId, DisplayOptions options)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(teamId))
            {
                RedirectToAction("Error");
            }
            return await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, options);
        }

        [Authorize]
        public async Task<IActionResult> GetSprintById(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId, true);

            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            if (sprint == null)
                return View("ErrorGetAllSprints");
            return View(sprint);
        }

        [Authorize]
        public async Task<IActionResult> EditSprintAsync(int teamId, int sprintId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId,false);

            EditSprintViewModel model = new EditSprintViewModel {TeamId = teamId, TeamName = team.TeamName, SprintId = sprint.Id, SprintName = sprint.Name,
                SprintDaysInSprint = sprint.DaysInSprint, SprintStorePointInHours = sprint.StoryPointInHours, ErrorMessage=errorMessage, IsActive = sprint.IsActive };

            if (sprint.IsActive)
            {
                ViewBag.SprintActive = "checked";
                ViewBag.SprintNotActive = "";
            }
            else
            {
                ViewBag.SprintActive = "";
                ViewBag.SprintNotActive = "checked";
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> AddSprintAsync(int teamId, string errorMessage)
        {
            var team = await _manageSprintsService.GetTeam(teamId);

            ViewBag.ErrorMessage = errorMessage;
            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamId = teamId;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditSprintAsync(int teamId, int sprintId, string sprintName, int daysInSprint, int storePointsInHours, bool isActive)
            {
            if (string.IsNullOrEmpty(sprintName))
            {
                return RedirectToAction("EditSprint", new { teamId = teamId, sprintId = sprintId, errorMessage = _localizer["NameFieldError"] });
            }
            if (daysInSprint <= 0)
            {
                return RedirectToAction("EditSprint", new { teamId = teamId, sprintId = sprintId, errorMessage = _localizer["DaysFieldError"] });
            }
            if (storePointsInHours <= 0)
            {
                return RedirectToAction("EditSprint", new { teamId = teamId, sprintId=sprintId, errorMessage = _localizer["PointsFieldError"] });
            }

            var sprint = new Sprint { Id = sprintId, TeamId = teamId, Name = sprintName, DaysInSprint = daysInSprint, StoryPointInHours = storePointsInHours, IsActive = isActive };
            var result = await EditSprintAsync(sprint);

            if (result) return RedirectToAction("AllSprints", new { teamId = teamId });
            else return RedirectToAction("AddError", new { teamId = teamId });

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSprintAsync(int teamId, string sprintName, int daysInSprint, int storePointsInHours, bool isActive)
        {
            var sprint = new Sprint { TeamId = teamId, Name = sprintName, DaysInSprint = daysInSprint, StoryPointInHours = storePointsInHours, IsActive = isActive };

            if (string.IsNullOrEmpty(sprintName))
            {
                return RedirectToAction("AddSprint", new { teamId = teamId, errorMessage = _localizer["NameFieldError"] });
            }
            if (daysInSprint <= 0)
            {
                return RedirectToAction("AddSprint", new { teamId = teamId, errorMessage = _localizer["DaysFieldError"] });
            }
            if (storePointsInHours <= 0)
            {
                return RedirectToAction("AddSprint", new { teamId = teamId, errorMessage = _localizer["PointsFieldError"] });
            }

            var result = await AddSprintAsync(sprint);

                if (result) return RedirectToAction("AllSprints", new { teamId = teamId});
                else return RedirectToAction("AddError", new { teamId = teamId });
            
        }

        public IActionResult AddError(int teamId)
        {
            ViewBag.TeamId = teamId;
            return View();
        }

        public IActionResult Error()
        {
            ViewData["Error"] = _localizer["Error"];
            return View();
        }

        [Authorize, NonAction]
        public async Task<bool> AddSprintAsync(Sprint sprint)
        {
            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                return await _manageSprintsService.AddSprintAsync(sprint);
            }
            else return false;
        }

        [Authorize, NonAction]
        public async Task<bool> EditSprintAsync(Sprint sprint)
        {
            if (await _accessCheckService.IsOwnerAsync(sprint.TeamId))
            {
                return await _manageSprintsService.EditSprintAsync(sprint);
            }
            else return false;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Remove(int sprintId)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprintId,false);
            var teamId = sprint.TeamId;
            var result = await _manageSprintsService.RemoveAsync(sprintId);
            if (result)
                return RedirectToAction("AllSprints",new { team_id = teamId});
            return RedirectToAction("ErrorRemove");
        }

        public IActionResult ErrorRemove()
        {
            return View();
        }
    }
}
