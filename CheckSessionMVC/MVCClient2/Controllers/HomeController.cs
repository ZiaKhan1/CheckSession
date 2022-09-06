using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCClient2.Models;


namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment  webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var rpiframehtml = System.IO.Path.Combine(
                _webHostEnvironment.WebRootPath, "rpiframe2.html"
                );

            ViewBag.IframeHtml = 
                System.IO.File.ReadAllText(rpiframehtml);
            return View();
        }

        [Authorize(Policy = "member-self-management")]
        public IActionResult Index2()
        {

            var rpiframehtml = System.IO.Path.Combine(
                _webHostEnvironment.WebRootPath, "rpiframe2.html"
                );

            ViewBag.IframeHtml = 
                System.IO.File.ReadAllText(rpiframehtml);
            return View("Index");
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

        public IActionResult Logout()
            {
            return SignOut("Cookies", "oidc");
           }

        public IActionResult LogoutFromAppOnlyButNotFromOP()
            {
                SignOut("Cookies");
                return RedirectToAction("Index");
            }    

    }
}
