using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class SellerDashboardController : Controller
    {
        private readonly ISellerDashboardRepository _repo;

        public SellerDashboardController(ISellerDashboardRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index(int? month, int? year)
        {
          
            var userEmail = User.Identity?.Name;
       
            int staffId = 1; 

          
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            ViewBag.DailyOrders = _repo.GetOrderCount(staffId, "day");
            ViewBag.DailyRevenue = _repo.GetRevenue(staffId, "day");

            ViewBag.WeeklyOrders = _repo.GetOrderCount(staffId, "week");
            ViewBag.WeeklyRevenue = _repo.GetRevenue(staffId, "week");

            ViewBag.MonthlyOrders = _repo.GetOrderCount(staffId, "month");
            ViewBag.MonthlyRevenue = _repo.GetRevenue(staffId, "month");

            ViewBag.ChartData = _repo.GetDailyChartData(staffId, selectedMonth, selectedYear);
            ViewBag.CustomerChart = _repo.GetCustomerGrowthChart(staffId, selectedMonth, selectedYear);
            ViewBag.ProductChart = _repo.GetTopProductsChart(staffId, selectedMonth, selectedYear);

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;

           
            return View();
        }
    }
}
