using Microsoft.AspNetCore.Mvc;
using Mock1Airline.Domains;
using System.IO;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


namespace MockAirline2Ident.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Master master;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            master = new Master();
        }

        public async Task<IActionResult> Index()
        {
            /*await master.GenerateFlights();*/

            /*await master.CreateFlightClasses();*/

            return View();
        }

       
        

        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Debug()
        {
            return Content("Hello World - Login Redirect Working");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
