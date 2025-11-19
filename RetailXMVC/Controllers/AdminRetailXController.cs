using BusinessObjectRetailX.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoriesRetailX;

public class SystemStatisticViewModel
{
    public Statistic Today { get; set; }              // thống kê hôm nay
    public List<Statistic> RecentDays { get; set; } = new();  // list các ngày trước (kể cả hôm nay cũng được)
}
namespace RetailXMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRetailXController : Controller
    {
        private readonly ITenantRepository tenantRepo;
        private readonly IStatisticRepository statisticRepo;
        public AdminRetailXController(ITenantRepository tenantRepo, IStatisticRepository statisticRepo)
        {
            this.tenantRepo = tenantRepo;
            this.statisticRepo = statisticRepo;
        }
        [HttpGet]
        public IActionResult System()
        {
            string day = DateTime.Now.ToString("yyyy-MM-dd");
            if (!statisticRepo.CheckStatisticExists(day))
            {
                statisticRepo.EnsureStatisticForToday();
            }
            var statToDay = statisticRepo.GetStatistic(day);
            var listStat = statisticRepo.GetStatisticList();
            var model = new SystemStatisticViewModel
            {
                Today = statToDay,
                RecentDays = listStat
            };
            return View(model);
        }
    }
}
