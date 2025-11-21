using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repositories;
using RetailXMVC.Utils;

namespace RetailXMVC.Controllers
{
    public class AIController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IConfiguration _configuration;

        public AIController(IReportRepository reportRepository, IConfiguration configuration)
        {
            _reportRepository = reportRepository;
            _configuration = configuration;
        }

        // ===== TRANG AI CHAT =====
        [HttpGet]
        public IActionResult Chat()
        {
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

        // ===== API: PHÂN TÍCH TỰ ĐỘNG CHO CHART =====
        [HttpPost]
        public async Task<IActionResult> GetAnalysis([FromBody] AnalysisRequest request)
        {
            try
            {
                if (request.Year <= 0)
                {
                    return Json(new { success = false, message = "Năm không hợp lệ" });
                }

                // Lấy dữ liệu từ repository
                var monthlyData = GetMonthlyFinancialData(request.Year);

                var totalRevenue = monthlyData.Sum(m => m.Revenue);
                var totalCost = monthlyData.Sum(m => m.Cost);
                var totalSalary = monthlyData.Sum(m => m.Salary);
                var totalProfit = monthlyData.Sum(m => m.Profit);

                // Tạo prompt
                var prompt = FinancialPromptBuilder.BuildAnalysisPrompt(
                    request.Year,
                    monthlyData,
                    totalRevenue,
                    totalCost,
                    totalSalary,
                    totalProfit
                );

                // Gọi Gemini AI
                var aiResponse = await GeminiAIUtil.AnalyzeFinancialData(prompt, _configuration);

                return Json(new { success = true, analysis = aiResponse });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // ===== API: CHAT VỚI AI =====
        [HttpPost]
        public async Task<IActionResult> ChatMessage([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Json(new { success = false, message = "Vui lòng nhập câu hỏi." });
                }

                int currentYear = DateTime.Now.Year;
                var monthlyData = GetMonthlyFinancialData(currentYear);

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

        // ===== PRIVATE HELPER METHOD =====
        private List<MonthlyFinancialData> GetMonthlyFinancialData(int year)
        {
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, 31);

            var allReports = _reportRepository.GetRevenueReports(startDate, endDate);

            return Enumerable.Range(1, 12).Select(month => new MonthlyFinancialData
            {
                Month = month,
                Revenue = allReports.Where(r => r.Month == month && r.Year == year)
                                   .Sum(r => r.AmountRevenue ?? 0),
                Cost = allReports.Where(r => r.Month == month && r.Year == year)
                                .Sum(r => r.AmountCost ?? 0),
                Salary = allReports.Where(r => r.Month == month && r.Year == year)
                                  .Sum(r => r.AmountSalary ?? 0),
                Profit = allReports.Where(r => r.Month == month && r.Year == year)
                                  .Sum(r => r.Profit ?? 0)
            }).ToList();
        }

        // ===== DTO CLASSES =====
        public class AnalysisRequest
        {
            public int Year { get; set; }
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}