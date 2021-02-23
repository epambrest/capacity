using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Web.ViewModels.Home;

namespace Teams.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDiagnosticService _diagnosticService;

        public HomeController(ILogger<HomeController> logger, IDiagnosticService diagnosticService)
        {
            _logger = logger;
            _diagnosticService = diagnosticService;
        }

        public IActionResult Index()    
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("GetMyTeams", "ManageTeams");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Health()
        {
            HealthViewModel healthViewModel = new HealthViewModel()
            {
                Version = _diagnosticService.GetCurrentVersion(),
                IsDbConnected = await _diagnosticService.CheckDbConnection(),
                DataServerTime = _diagnosticService.GetServerDataTime()
            };
            return View(healthViewModel);
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
