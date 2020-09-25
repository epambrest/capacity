using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Teams.Models;
using Teams.Services;

namespace Teams.Controllers
{
    public class ManageTeamsController : Controller
    {
        private readonly IManageTeamsService _teamsService;
        public ManageTeamsController(IManageTeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        public IActionResult Index()
        {
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

        public void Remove(int team_id)
        {
            _teamsService.Remove(team_id);
        }
    }
}