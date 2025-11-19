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


    }
}
