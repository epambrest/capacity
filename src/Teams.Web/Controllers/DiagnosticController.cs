using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Web.ViewModels.Home;

namespace Teams.Web.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<DiagnosticController> _logger;

        public DiagnosticController(IConfiguration config, ApplicationDbContext dbContext, ILogger<DiagnosticController> logger)
        {
            _config = config;
            _dbContext = dbContext;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Health()
        {
            HealthViewModel healthViewModel = new HealthViewModel()
            {
                Version = GetVersion(),
                IsDbConnected = await CheckDbConnection(),
                DataServerTime = GetServerDataTime()
            };
            return View(healthViewModel);
        }

        private async Task<bool> CheckDbConnection()
        {
            try
            {
                return await _dbContext.Database.CanConnectAsync();
            }
            catch(Exception e)
            {
                _logger.LogError($"Error Db Connection: {e.Message}");
                return false;
            }
        }

        private string GetVersion()
        {
            var result = _config.GetValue<string>("version"); // "Information"
            return result.ToString();
        }
        private DateTime GetServerDataTime()
        {
            return DateTime.Now;
        }
    }
}
