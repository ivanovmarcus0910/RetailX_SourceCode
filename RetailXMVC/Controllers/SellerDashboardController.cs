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
            // --- 1. Lấy Staff ID từ User đăng nhập ---
            var userEmail = User.Identity?.Name;
            // int staffId = 1; // Fallback
            // if (!string.IsNullOrEmpty(userEmail)) {
            //      var staff = _staffRepo.GetStaffListForOwner().FirstOrDefault(s => s.Email == userEmail);
            //      if (staff != null) staffId = staff.StaffId;
            // }
            int staffId = 1; // Tạm thời hardcode để test

            // --- 2. Xử lý bộ lọc thời gian ---
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            // --- 3. Lấy số liệu thống kê (ViewBag) ---
            // Các hàm này vẫn lấy theo logic "hiện tại" (Today, Week)
            ViewBag.DailyOrders = _repo.GetOrderCount(staffId, "day");
            ViewBag.DailyRevenue = _repo.GetRevenue(staffId, "day");

            ViewBag.WeeklyOrders = _repo.GetOrderCount(staffId, "week");
            ViewBag.WeeklyRevenue = _repo.GetRevenue(staffId, "week");

            // Riêng phần Monthly thì lấy theo tháng được chọn hay tháng hiện tại cũng được
            // Ở đây mình để logic cũ là "Tháng hiện tại" cho thẻ Card
            ViewBag.MonthlyOrders = _repo.GetOrderCount(staffId, "month");
            ViewBag.MonthlyRevenue = _repo.GetRevenue(staffId, "month");

            // --- 4. Lấy dữ liệu BIỂU ĐỒ (Theo tháng được chọn) ---
            ViewBag.ChartData = _repo.GetDailyChartData(staffId, selectedMonth, selectedYear);
            ViewBag.CustomerChart = _repo.GetCustomerGrowthChart(staffId, selectedMonth, selectedYear);
            ViewBag.ProductChart = _repo.GetTopProductsChart(staffId, selectedMonth, selectedYear);

            // --- 5. Lưu trạng thái lọc để hiển thị lại trên View ---
            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;

            // --- 6. Danh sách đơn hàng (Model chính) ---
            // Không lấy top 5 nữa, mà lấy list rỗng hoặc list theo tháng tùy bạn
            // Ở View mới mình đã bỏ bảng này để thay bằng Chart tròn nên truyền null cũng được
            return View();
        }
    }
}
