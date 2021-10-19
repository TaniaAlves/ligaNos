using ligaNos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ligaNos.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()        
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult ExistingClub()
        {
            return View();
        }
        public IActionResult InfoAggregatedClub()
        {
            return View();
        }
        public IActionResult ClubInManager()
        {
            return View();
        }

        public IActionResult ClubName()
        {
            return View();
        }
    }

}
