using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class InventoryDashboardController : Controller
    {
        private readonly IInventoryDashBoardRepository _repo;
        private readonly ICategoryRepository _repoCategory;
        private readonly ISupplierRepository _repoSupplier;

        public InventoryDashboardController(IInventoryDashBoardRepository repo, ICategoryRepository _repoCate, ISupplierRepository _repoSup  )
        {
            _repo = repo;
            _repoCategory = _repoCate;
            _repoSupplier = _repoSup;
        }

        public IActionResult Index()
        {
            ViewBag.TotalProducts = _repo.GetTotalProducts();
            ViewBag.TotalCategories = _repo.GetTotalCategories();
            ViewBag.TotalSuppliers = _repo.GetTotalSuppliers();
            ViewBag.TotalCustomers = _repo.GetTotalCustomers();

            ViewBag.InventoryValue = _repo.GetInventoryValue();
            ViewBag.LowStock = _repo.GetLowStock();
            ViewBag.MonthlyImport = _repo.GetMonthlyImport();

            return View();
        }

        public IActionResult ImportReport(int? year = null, int? month = null, int? supplierId = null, int? categoryId = null)
        {
            // Lấy dữ liệu báo cáo chi tiết
            var report = _repo.GetImportReportDetail(year, month, supplierId, categoryId);

            // Lấy danh sách dropdown
            var currentYear = DateTime.Now.Year;
            ViewBag.Years = new SelectList(Enumerable.Range(currentYear - 5, 6), year); // 5 năm trước đến hiện tại
            ViewBag.Months = new SelectList(Enumerable.Range(1, 12), month);
            ViewBag.Suppliers = new SelectList(_repoSupplier.GetAllSuppliers(), "SupplierId", "SupplierName", supplierId);
            ViewBag.Categories = new SelectList(_repoCategory.GetAll(), "CategoryId", "CategoryName", categoryId);

            return View(report);
        }
        [HttpGet]
        public IActionResult ExportImportReport(int? year, int? month, int? supplierId, int? categoryId)
        {
            // Lấy data cùng logic với màn hình ImportReport
            var data = _repo.GetImportReportDetail(year, month, supplierId, categoryId);

            if (data == null || !data.Any())
            {
                TempData["ErrorMessage"] = "Không có dữ liệu để xuất.";
                return RedirectToAction("ImportReport");
            }

            var sb = new StringBuilder();

            // Header
            sb.AppendLine("Year,Month,Product Name,Quantity,Price,Total");

            // Group theo Year + TimeGroup (Month)
            foreach (var group in data.GroupBy(m => new { m.Year, m.TimeGroup }))
            {
                decimal totalGroup = group.Sum(x => (decimal)x.Total);

                foreach (var item in group)
                {
                    sb.AppendLine(
                        $"{item.Year}," +
                        $"{item.TimeGroup}," +
                        $"{Escape(item.ProductName)}," +
                        $"{item.Quantity}," +
                        $"{item.Price}," +
                        $"{item.Total}"
                    );
                }

                sb.AppendLine($",,, ,Total {group.Key.TimeGroup}-{group.Key.Year},{totalGroup}");
            }

            // Xuất file CSV UTF-8 tránh lỗi font
            var preamble = Encoding.UTF8.GetPreamble();
            var bodyBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileBytes = new byte[preamble.Length + bodyBytes.Length];

            Buffer.BlockCopy(preamble, 0, fileBytes, 0, preamble.Length);
            Buffer.BlockCopy(bodyBytes, 0, fileBytes, preamble.Length, bodyBytes.Length);

            string fileName = $"ImportReport_{DateTime.Now:yyyyMMddHHmm}.csv";
            return File(fileBytes, "text/csv", fileName);
        }

        private string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Contains(",") || text.Contains("\""))
            {
                text = text.Replace("\"", "\"\"");
                return $"\"{text}\"";
            }
            return text;
        }


    }
}
