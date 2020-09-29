using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Teams.Models;
using Teams.Services;
namespace Teams.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _manageTeamsMembersService;

        private readonly IAccessCheckService _accessCheckService;

        public ManageTeamMembersController(IManageTeamsMembersService manageTeamsMembersService, IAccessCheckService accessCheckService)
        {
            _manageTeamsMembersService = manageTeamsMembersService;

            _accessCheckService = accessCheckService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserSaw()
        {
            var memberlist = new List<TeamMember>();
            memberlist.Add(GetMemberAsync(8, "475122a9-d83c-45bd-a795-341ff4ddc721").Result);
            return View(memberlist);
        }

        [Authorize, NonAction]
        public async Task<TeamMember> GetMemberAsync(int team_id, string member_id)
        {
            if (!_accessCheckService.OwnerOrMemberAsync(team_id).Result)
            {
                return null;
            }
            else return await _manageTeamsMembersService.GetMemberAsync(team_id, member_id);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}