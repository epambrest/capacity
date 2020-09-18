using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Teams.Models;
using Teams.Services;

namespace Teams.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _membersService;

        public ManageTeamMembersController(IManageTeamsMembersService membersService)
        {
            _membersService = membersService;
        
        }

        public IActionResult Index()
        {
            _membersService.Add(16, "b5cc0f2f-29c9-4fac-83ef-182914f98610");
            return View();
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