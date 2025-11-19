using BusinessObjectRetailX.Models;
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
        private readonly ITenantRepository _tenantRepo;
        private readonly IUserRepository _userRepo;
        
        public HomeController(ILogger<HomeController> logger, IStatisticRepository statisticRepository, ITenantRepository tenantRepo, IUserRepository userRepo)
        {
            this.statisticRepository = statisticRepository;
            this.statisticRepository.IncreaseCount();
            _logger = logger;
            _tenantRepo = tenantRepo;
            _userRepo = userRepo;
        }

        public IActionResult Index()
        {
            List<Tenant> activeTenants = _tenantRepo.GetAllTenant();
            return View(activeTenants);
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
