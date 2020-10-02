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

        [Authorize, NonAction]
        public async Task<TeamMember> GetMemberAsync(int team_id, string member_id)
        {
            if (await _accessCheckService.OwnerOrMemberAsync(team_id))
            {
                return await _manageTeamsMembersService.GetMemberAsync(team_id, member_id);
            }
            else return null;
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