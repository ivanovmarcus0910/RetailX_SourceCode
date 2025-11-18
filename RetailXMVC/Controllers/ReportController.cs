using System;
using System.Linq;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public IActionResult Index(string startDateStr, string endDateStr)
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = new DateTime(endDate.Year, endDate.Month, 1);

            if (DateTime.TryParse(startDateStr, out DateTime start) && DateTime.TryParse(endDateStr, out DateTime end))
            {
                startDate = start;
                endDate = end;
            }

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

            var reports = _reportRepository.GetRevenueReports(startDate, endDate);

            return View(reports);
        }

        [HttpPost]
        public IActionResult Generate(DateTime startDate, DateTime endDate)
        {
            int staffIdCreator = 2; // Giả định Accounter ID

            try
            {
                _reportRepository.GenerateRevenueReport(startDate, endDate, staffIdCreator);
                TempData["SuccessMessage"] = $"Đã tạo báo cáo thành công cho kỳ từ {startDate.ToShortDateString()} đến {endDate.ToShortDateString()}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo báo cáo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { startDateStr = startDate.ToString("yyyy-MM-dd"), endDateStr = endDate.ToString("yyyy-MM-dd") });
        }

        public IActionResult ExportSales(DateTime startDate, DateTime endDate)
        {
            var orders = _reportRepository.GetOrdersForExport(startDate, endDate);

            var csvContent = GenerateCsvContent(orders);

            return File(
                new System.Text.UTF8Encoding().GetBytes(csvContent),
                "text/csv",
                $"Sales_Report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv"
            );
        }

        // Hàm hỗ trợ tạo CSV
        private string GenerateCsvContent(List<Order> orders)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("OrderID,CustomerName,OrderDate,TotalAmount,StaffName");

            foreach (var order in orders)
            {
                decimal total = order.OrderDetails?.Sum(od => od.Quantity * od.Product.Price) ?? 0;
                sb.AppendLine($"{order.OrderId},{order.Customer?.CustomerName},{order.CreateDate:yyyy-MM-dd},{total},{order.Staff?.StaffName}");
            }
            return sb.ToString();
        }
    }
}