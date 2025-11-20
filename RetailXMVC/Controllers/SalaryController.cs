using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace RetailXMVC.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ISalaryRepository _salaryRepository;
        private const int PageSize = 10; 

        public SalaryController(ISalaryRepository salaryRepository)
        {
            _salaryRepository = salaryRepository;
        }

        // --- 1. Xem Danh sách Lương 
        public IActionResult Index(int? month, int? year, int page = 1)
        {
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            // Lấy tất cả lương trong tháng
            var allSalaries = _salaryRepository.GetSalaries(currentMonth, currentYear);

     
            var totalSalaries = allSalaries.Count;
            var totalPages = (int)Math.Ceiling(totalSalaries / (double)PageSize);

 
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Lấy dữ liệu cho trang hiện tại
            var pagedSalaries = allSalaries
                .OrderBy(s => s.Staff.StaffId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

 
            ViewBag.Month = currentMonth;
            ViewBag.Year = currentYear;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalSalaries = totalSalaries;
            ViewBag.AllSalaries = allSalaries; 

            return View(pagedSalaries);
        }

        // --- 2. Xử lý Tính toán Bảng lương 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Process(int month, int year)
        {
            try
            {
                _salaryRepository.ProcessMonthlySalaries(month, year);
                TempData["SuccessMessage"] = $"Đã xử lý và tạo/cập nhật bảng lương tháng {month}/{year} thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xử lý bảng lương: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { month, year, page = 1 });
        }

        // --- 3. Điều chỉnh Thưởng/Khấu trừ 
        public IActionResult Adjust(int salaryId)
        {
            var salary = _salaryRepository.GetSalaryById(salaryId);
            if (salary == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi lương.";
                return RedirectToAction(nameof(Index));
            }
            return View(salary);
        }

        // POST: /Salary/Adjust
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Adjust(int salaryId, decimal bonus, decimal deduction)
        {
            try
            {
                
                var salary = _salaryRepository.GetSalaryById(salaryId);

                if (salary == null)
                {
                    TempData["ErrorMessage"] = "❌ Không tìm thấy bản ghi lương.";
                    return RedirectToAction(nameof(Index));
                }

                
                bool success = _salaryRepository.UpdateBonusDeduction(
                    salary.StaffId,  
                    salary.Month,         
                    salary.Year,          
                    bonus,              
                    deduction           
                );

                if (success)
                {
                    TempData["SuccessMessage"] = $"✅ Đã điều chỉnh lương cho {salary.Staff?.StaffName} thành công!";
                    return RedirectToAction(nameof(Index), new { month = salary.Month, year = salary.Year });
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ Lỗi khi cập nhật lương vào database.";
                    return View(salary);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Lỗi: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // --- 4. Xác nhận Thanh toán 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pay(int salaryId, int month, int year)
        {
            int dayPayment = DateTime.Now.Day;

            bool success = _salaryRepository.UpdatePaymentStatus(salaryId, 1, dayPayment);

            if (success)
            {
                TempData["SuccessMessage"] = $"Đã xác nhận thanh toán lương thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi: Không thể xác nhận thanh toán.";
            }

            return RedirectToAction(nameof(Index), new { month, year });
        }
    }
}