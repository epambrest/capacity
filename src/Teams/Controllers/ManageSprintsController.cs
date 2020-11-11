using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Teams.Models;
using Teams.Services;
using Microsoft.Extensions.Localization;

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
        public async Task<IActionResult> AllSprints(int team_id, DisplayOptions options)
        {
            List<Sprint> sprints;
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                sprints = (List<Sprint>)await _manageSprintsService.GetAllSprintsAsync(team_id, options);
            }
            else 
                return View("ErrorGetAllSprints");

            var team = await _manageSprintsService.GetTeam(team_id);

            if (await _accessCheckService.IsOwnerAsync(team_id)) ViewBag.AddVision = "visible";
            else ViewBag.AddVision = "collapse";

            ViewData["DaysInSprint"] = _localizer["DaysInSprint"];
            ViewData["StoryPointInHours"] = _localizer["StoryPointInHours"];
            ViewData["NameOfSprint"] = _localizer["NameOfSprint"];
            ViewData["MemberEmail"] = _localizer["MemberEmail"];
            ViewData["Owner"] = _localizer["Owner"];
            ViewData["Member"] = _localizer["Member"];
            ViewData["AddMember"] = _localizer["AddMember"];
            ViewData["RemoveMember"] = _localizer["RemoveMember"];

            ViewBag.TeamId = team_id;
            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamOwner = team.Owner.Email;
            List<TeamMember> teamMembers = await GetAllTeamMembersAsync(team_id, new DisplayOptions { });

            CombinedModel combinedModel = new CombinedModel { Sprints = sprints, TeamMembers = teamMembers };
            return View(combinedModel);
        }

        [Authorize, NonAction]
        public async Task<List<TeamMember>> GetAllTeamMembersAsync(int team_id, DisplayOptions options)
        {
            if (!await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                RedirectToAction("Error");
            }
            return await _manageTeamsMembersService.GetAllTeamMembersAsync(team_id, options);
        }

        [Authorize]
        public async Task<IActionResult> GetSprintById(int sprint_id)
        {
            var sprint = await _manageSprintsService.GetSprintAsync(sprint_id);

            if (sprint == null)
                return View("ErrorGetAllSprints");
            return View(sprint);
        }

        [Authorize]
        public async Task<IActionResult> AddSprintAsync(int team_id, string error_message)
        {
            var team = await _manageSprintsService.GetTeam(team_id);

            ViewData["Name"] = _localizer["Name"];
            ViewData["Active"] = _localizer["Active"];
            ViewData["NotActive"] = _localizer["NotActive"];
            ViewData["AddSprint"] = _localizer["AddSprint"];
            ViewData["ReturnToSprint"] = _localizer["ReturnToSprint"];
            ViewData["DaysInSprint"] = _localizer["DaysInSprint"];
            ViewData["StoryPointInHours"] = _localizer["StoryPointInHours"];

            ViewBag.ErrorMessage = error_message;
            ViewBag.TeamName = team.TeamName;
            ViewBag.TeamId = team_id;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSprintAsync(int team_id, string sprint_name, int days_in_sprint, int story_points_in_hours, bool is_active)
        {
            var sprint = new Sprint { TeamId = team_id, Name = sprint_name, DaysInSprint = days_in_sprint, StoryPointInHours = story_points_in_hours, IsActive = is_active };
          
            if(string.IsNullOrEmpty(sprint_name))
            {
                return RedirectToAction("AddSprint", new { team_id = team_id, error_message = _localizer["NameFieldError"] });
            }
            if (days_in_sprint <= 0)
            {
                return RedirectToAction("AddSprint", new { team_id = team_id, error_message = _localizer["DaysFieldError"] });
                }
            if (story_points_in_hours <= 0)
            {
                return RedirectToAction("AddSprint", new { team_id = team_id, error_message = _localizer["PointsFieldError"] });
            }

           var result = await AddSprintAsync(sprint);

                if (result) return RedirectToAction("AllSprints", new { team_id = team_id});
                else return RedirectToAction("AddError", new { team_id = team_id });
            
        }

        public IActionResult AddError(int team_id)
        {
            ViewBag.TeamId = team_id;
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
        public IActionResult Index()
        {
            return View();
        }
    }
}
