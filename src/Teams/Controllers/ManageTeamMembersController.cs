using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Teams.Models;
using Teams.Services;
namespace Teams.Controllers
{
    public class ManageTeamMembersController : Controller
    {
        private readonly IManageTeamsMembersService _manageTeamsMembersService;

        public ManageTeamMembersController(IManageTeamsMembersService manageTeamsMembersService)
        {
            _manageTeamsMembersService = manageTeamsMembersService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize, NonAction]
        public TeamMember GetMember(int team_id, string member_id)
        {
            return _manageTeamsMembersService.GetMember(team_id, member_id);
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