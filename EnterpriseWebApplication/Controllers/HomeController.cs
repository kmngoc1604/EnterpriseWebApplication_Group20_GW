using EnterpriseWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return ViewComponent("MessagePage", new XTLASPNET.MessagePage.Message
            {
                title = "Private",
                htmlcontent = "Why are you here?",
                secondwait = 1,
                urlredirect = "/"
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
