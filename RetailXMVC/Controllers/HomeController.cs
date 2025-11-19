using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;
using RetailXMVC.Models;
using System.Diagnostics;

namespace RetailXMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStatisticRepository statisticRepository;
        public HomeController(ILogger<HomeController> logger, IStatisticRepository statisticRepository)
        {
            this.statisticRepository = statisticRepository;
            this.statisticRepository.IncreaseCount();
            _logger = logger;

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
    }
}
