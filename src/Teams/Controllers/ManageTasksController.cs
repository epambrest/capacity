using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Teams.Controllers
{
    public class ManageTasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
