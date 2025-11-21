using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Repositories;
using RetailXMVC.Utils;

namespace RetailXMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IConfiguration _configuration;
        private const int PageSize = 10;

        public ReportController(IReportRepository reportRepository, IConfiguration configuration)
        {
            _reportRepository = reportRepository;
            _configuration = configuration;
        }

        public IActionResult Index(string startDateStr, string endDateStr, int page = 1)
        {
            DateTime endDate = DateTime.Today;
            DateTime startDate = new DateTime(endDate.Year, endDate.Month, 1);

            if (DateTime.TryParse(startDateStr, out DateTime start) && DateTime.TryParse(endDateStr, out DateTime end))
            {
                startDate = start;
                endDate = end;
            }

            // Lấy tất cả báo cáo
            var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

            // Tính toán phân trang
            var totalReports = allReports.Count;
            var totalPages = (int)Math.Ceiling(totalReports / (double)PageSize);

            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Lấy dữ liệu cho trang hiện tại
            var pagedReports = allReports
                .OrderBy(r => r.ReportRevenueId) 
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Truyền dữ liệu sang View
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalReports = totalReports;
            ViewBag.AllReports = allReports; 

            return View(pagedReports);
        }

        [HttpPost]
        public IActionResult Generate(DateTime startDate, DateTime endDate)
        {
            
            var staffIdCreatorReport = User.FindFirst("StaffId")?.Value;

            if (string.IsNullOrEmpty(staffIdCreatorReport))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin nhân viên. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index");
            }

            int staffIdCreator = int.Parse(staffIdCreatorReport);

            try
            {
                _reportRepository.GenerateRevenueReport(startDate, endDate, staffIdCreator);
                TempData["SuccessMessage"] = $"Đã tạo báo cáo thành công cho kỳ từ {startDate.ToString("dd/MM/yyyy")} đến {endDate.ToString("dd/MM/yyyy")}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo báo cáo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index), new { startDateStr = startDate.ToString("yyyy-MM-dd"), endDateStr = endDate.ToString("yyyy-MM-dd"), page = 1 });
        }

        [HttpGet]
        public IActionResult ExportSales(string startDateStr, string endDateStr)
        {
            if (!DateTime.TryParse(startDateStr, out DateTime startDate) ||
                !DateTime.TryParse(endDateStr, out DateTime endDate))
            {
                TempData["ErrorMessage"] = "Tham số ngày không hợp lệ. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }

            try
            {
                var orders = _reportRepository.GetOrdersForExport(startDate, endDate);

                if (orders == null || !orders.Any())
                {
                    TempData["ErrorMessage"] = "Không có đơn hàng nào được tìm thấy trong kỳ này để xuất.";
                    return RedirectToAction("Index", new { startDateStr = startDate.ToString("yyyy-MM-dd"), endDateStr = endDate.ToString("yyyy-MM-dd") });
                }

                var csvContent = GenerateSalesCsvContent(orders);

                var preamble = Encoding.UTF8.GetPreamble();
                var bodyBytes = Encoding.UTF8.GetBytes(csvContent);
                var fileBytes = new byte[preamble.Length + bodyBytes.Length];

                Buffer.BlockCopy(preamble, 0, fileBytes, 0, preamble.Length);
                Buffer.BlockCopy(bodyBytes, 0, fileBytes, preamble.Length, bodyBytes.Length);

                var fileName = $"BaoCaoBanHang_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xuất file bán hàng: " + ex.Message;
                return RedirectToAction("Index", new { startDateStr = startDate.ToString("yyyy-MM-dd"), endDateStr = endDate.ToString("yyyy-MM-dd") });
            }
        }

        [HttpGet]
        public IActionResult ExportReportSummary(string startDateStr, string endDateStr)
        {
            if (!DateTime.TryParse(startDateStr, out DateTime startDate) ||
                !DateTime.TryParse(endDateStr, out DateTime endDate))
            {
                TempData["ErrorMessage"] = "Tham số ngày không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var reports = _reportRepository.GetRevenueReports(startDate, endDate);

                if (reports == null || !reports.Any())
                {
                    TempData["ErrorMessage"] = "Không có báo cáo nào trong kỳ này.";
                    return RedirectToAction("Index", new { startDateStr, endDateStr });
                }

                var csvContent = GenerateReportSummaryCsv(reports);

                var preamble = Encoding.UTF8.GetPreamble();
                var bodyBytes = Encoding.UTF8.GetBytes(csvContent);
                var fileBytes = new byte[preamble.Length + bodyBytes.Length];

                Buffer.BlockCopy(preamble, 0, fileBytes, 0, preamble.Length);
                Buffer.BlockCopy(bodyBytes, 0, fileBytes, preamble.Length, bodyBytes.Length);

                var fileName = $"BaoCaoTongHop_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xuất báo cáo tổng hợp: " + ex.Message;
                return RedirectToAction("Index", new { startDateStr, endDateStr });
            }
        }

        // ===== PRIVATE HELPER METHODS =====

        private string GenerateSalesCsvContent(List<Order> orders)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Mã Đơn,Ngày Đặt,Khách Hàng,Nhân Viên Bán,Mã SP,Tên Sản Phẩm,Giá Bán,Số Lượng,Thành Tiền");

            foreach (var order in orders)
            {
                var customerName = EscapeCsvField(order.Customer?.CustomerName ?? "N/A");
                var staffName = EscapeCsvField(order.Staff?.StaffName ?? "N/A");
                var orderDate = order.CreateDate.ToString("dd/MM/yyyy");

                if (order.OrderDetails == null || !order.OrderDetails.Any())
                {
                    sb.AppendLine($"{order.OrderId},{orderDate},{customerName},{staffName},,,0,0,0");
                    continue;
                }

                foreach (var detail in order.OrderDetails)
                {
                    var productPrice = detail.Product?.Price ?? 0;
                    var productName = EscapeCsvField(detail.Product?.ProductName ?? "N/A");
                    var lineTotal = detail.Quantity * productPrice;

                    sb.Append($"{order.OrderId},");
                    sb.Append($"{orderDate},");
                    sb.Append($"{customerName},");
                    sb.Append($"{staffName},");
                    sb.Append($"{detail.ProductId},");
                    sb.Append($"{productName},");
                    sb.Append($"{productPrice},");
                    sb.Append($"{detail.Quantity},");
                    sb.Append($"{lineTotal}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private string GenerateReportSummaryCsv(List<ReportRevenue> reports)
        {
            var sb = new StringBuilder();

            sb.AppendLine("BÁO CÁO TỔNG HỢP DOANH THU & LỢI NHUẬN");
            sb.AppendLine($"Từ ngày {reports.Min(r => r.DayStart)} đến {reports.Max(r => r.DayEnd)}");
            sb.AppendLine();
            sb.AppendLine("ID,Kỳ Báo Cáo,Doanh Thu (VNĐ),Chi Phí Nhập (VNĐ),Chi Phí Lương (VNĐ),Lợi Nhuận (VNĐ),Ngày Tạo,Người Tạo");

            foreach (var report in reports.OrderBy(r => r.ReportRevenueId))
            {
                sb.Append($"{report.ReportRevenueId},");
                sb.Append($"{report.DayStart:dd/MM/yyyy} - {report.DayEnd:dd/MM/yyyy},");
                sb.Append($"{report.AmountRevenue ?? 0},");
                sb.Append($"{report.AmountCost ?? 0},");
                sb.Append($"{report.AmountSalary ?? 0},");
                sb.Append($"{report.Profit ?? 0},");
                sb.Append($"{report.CreateDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"},");
                sb.Append($"{EscapeCsvField(report.Staff?.StaffName ?? "N/A")}");
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("TỔNG KẾT");
            sb.AppendLine($"Tổng Doanh Thu,{reports.Sum(r => r.AmountRevenue ?? 0)}");
            sb.AppendLine($"Tổng Chi Phí Nhập,{reports.Sum(r => r.AmountCost ?? 0)}");
            sb.AppendLine($"Tổng Chi Phí Lương,{reports.Sum(r => r.AmountSalary ?? 0)}");
            sb.AppendLine($"Tổng Lợi Nhuận,{reports.Sum(r => r.Profit ?? 0)}");

            return sb.ToString();
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }

        [HttpGet]
        public IActionResult Chart(int? year)  
        {
            int selectedYear = year ?? DateTime.Now.Year;

            DateTime startDate = new DateTime(2020, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year + 10, 12, 31);

            var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

            var monthlyData = Enumerable.Range(1, 12).Select(month => new
            {
                Month = month,
                Year = selectedYear,
                Revenue = allReports.Where(r => r.Month == month && r.Year == selectedYear)
                                   .Sum(r => r.AmountRevenue ?? 0),
                Cost = allReports.Where(r => r.Month == month && r.Year == selectedYear)
                                .Sum(r => r.AmountCost ?? 0),
                Salary = allReports.Where(r => r.Month == month && r.Year == selectedYear)
                                  .Sum(r => r.AmountSalary ?? 0),
                Profit = allReports.Where(r => r.Month == month && r.Year == selectedYear)
                                  .Sum(r => r.Profit ?? 0)
            }).ToList();

            var totalRevenue = monthlyData.Sum(m => m.Revenue);
            var totalCost = monthlyData.Sum(m => m.Cost);
            var totalSalary = monthlyData.Sum(m => m.Salary);
            var totalProfit = monthlyData.Sum(m => m.Profit);

            var availableYears = allReports
                .Where(r => r.Year.HasValue)
                .Select(r => r.Year.Value)
                .Distinct()
                .ToList();

            // Luôn có năm đang chọn
            if (!availableYears.Contains(selectedYear))
            {
                availableYears.Add(selectedYear);
            }

            if (!availableYears.Any())
            {
                availableYears.Add(DateTime.Now.Year);
            }

            availableYears = availableYears.OrderByDescending(y => y).ToList();

            //- GỌI GEMINI AI
            //string aiInsights = "";
            //try
            //{
            //    var monthlyDataForAI = monthlyData.Select(m => new MonthlyFinancialData
            //    {
            //        Month = m.Month,
            //        Revenue = m.Revenue,
            //        Cost = m.Cost,
            //        Salary = m.Salary,
            //        Profit = m.Profit
            //    }).ToList();

            //    var prompt = FinancialPromptBuilder.BuildAnalysisPrompt(
            //        selectedYear,
            //        monthlyDataForAI,
            //        totalRevenue,
            //        totalCost,
            //        totalSalary,
            //        totalProfit
            //    );

            //    aiInsights = await GeminiAIUtil.AnalyzeFinancialData(prompt, _configuration);
            //}
            //catch (Exception ex)
            //{
            //    aiInsights = $"⚠️ Không thể tải phân tích AI: {ex.Message}";
            //}
          

            ViewBag.SelectedYear = selectedYear;
            ViewBag.AvailableYears = availableYears;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCost = totalCost;
            ViewBag.TotalSalary = totalSalary;
            ViewBag.TotalProfit = totalProfit;
            ViewBag.MonthlyData = System.Text.Json.JsonSerializer.Serialize(monthlyData);
            //ViewBag.AIInsights = aiInsights;  

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAIAnalysis([FromBody] AIAnalysisRequest request)
        {
            try
            {
                if (request.Year <= 0)
                {
                    return Json(new { success = false, message = "Năm không hợp lệ" });
                }

                DateTime startDate = new DateTime(request.Year, 1, 1);
                DateTime endDate = new DateTime(request.Year, 12, 31);

                var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

                var monthlyData = Enumerable.Range(1, 12).Select(month => new MonthlyFinancialData
                {
                    Month = month,
                    Revenue = allReports.Where(r => r.Month == month && r.Year == request.Year)
                                       .Sum(r => r.AmountRevenue ?? 0),
                    Cost = allReports.Where(r => r.Month == month && r.Year == request.Year)
                                    .Sum(r => r.AmountCost ?? 0),
                    Salary = allReports.Where(r => r.Month == month && r.Year == request.Year)
                                      .Sum(r => r.AmountSalary ?? 0),
                    Profit = allReports.Where(r => r.Month == month && r.Year == request.Year)
                                      .Sum(r => r.Profit ?? 0)
                }).ToList();

                var totalRevenue = monthlyData.Sum(m => m.Revenue);
                var totalCost = monthlyData.Sum(m => m.Cost);
                var totalSalary = monthlyData.Sum(m => m.Salary);
                var totalProfit = monthlyData.Sum(m => m.Profit);

                var prompt = FinancialPromptBuilder.BuildAnalysisPrompt(
                    request.Year,
                    monthlyData,
                    totalRevenue,
                    totalCost,
                    totalSalary,
                    totalProfit
                );

                var aiResponse = await GeminiAIUtil.AnalyzeFinancialData(prompt, _configuration);

                return Json(new { success = true, analysis = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public class AIAnalysisRequest
        {
            public int Year { get; set; }
        }


        [HttpGet]
        public IActionResult AIChat()
        {
            // Lấy dữ liệu tổng quan để AI có context
            int currentYear = DateTime.Now.Year;
            DateTime startDate = new DateTime(currentYear, 1, 1);
            DateTime endDate = new DateTime(currentYear, 12, 31);

            var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

            var yearlyData = new
            {
                Year = currentYear,
                TotalRevenue = allReports.Sum(r => r.AmountRevenue ?? 0),
                TotalCost = allReports.Sum(r => r.AmountCost ?? 0),
                TotalSalary = allReports.Sum(r => r.AmountSalary ?? 0),
                TotalProfit = allReports.Sum(r => r.Profit ?? 0),
                ReportCount = allReports.Count
            };

            ViewBag.YearlyData = System.Text.Json.JsonSerializer.Serialize(yearlyData);

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChatWithAI([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Json(new { success = false, message = "Vui lòng nhập câu hỏi." });
                }

                // Lấy context dữ liệu
                int currentYear = DateTime.Now.Year;
                DateTime startDate = new DateTime(currentYear, 1, 1);
                DateTime endDate = new DateTime(currentYear, 12, 31);

                var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

                var monthlyData = Enumerable.Range(1, 12).Select(month => new MonthlyFinancialData
                {
                    Month = month,
                    Revenue = allReports.Where(r => r.Month == month && r.Year == currentYear)
                                       .Sum(r => r.AmountRevenue ?? 0),
                    Cost = allReports.Where(r => r.Month == month && r.Year == currentYear)
                                    .Sum(r => r.AmountCost ?? 0),
                    Salary = allReports.Where(r => r.Month == month && r.Year == currentYear)
                                      .Sum(r => r.AmountSalary ?? 0),
                    Profit = allReports.Where(r => r.Month == month && r.Year == currentYear)
                                      .Sum(r => r.Profit ?? 0)
                }).ToList();

                // Tạo prompt cho chatbot
                var prompt = FinancialPromptBuilder.BuildChatPrompt(
                    request.Message,
                    currentYear,
                    monthlyData
                );

                var aiResponse = await GeminiAIUtil.AnalyzeFinancialData(prompt, _configuration);

                return Json(new { success = true, response = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }

    }
}