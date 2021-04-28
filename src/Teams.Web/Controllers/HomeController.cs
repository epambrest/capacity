using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Teams.Web.ViewModels.Shared;

namespace Teams.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()    
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("GetMyTeams", "ManageTeams");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            string requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            ErrorViewModel errorViewModel = ErrorViewModel.Create(requestId);
            return View(errorViewModel);
        }
    }
}
