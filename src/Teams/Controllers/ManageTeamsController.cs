using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
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

        [Authorize]
        public async Task<IActionResult> Remove(int team_id)
        {
            var result = await _teamsService.RemoveAsync(team_id);
            if (result) 
               return RedirectToAction("Index", "Home");
            return RedirectToAction("ErorRemove");
        }

        public IActionResult ErrorRemoveAsync()
        {
            return View();
        }
    }
}