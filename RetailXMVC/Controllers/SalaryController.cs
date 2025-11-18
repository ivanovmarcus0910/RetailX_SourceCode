using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;
namespace RetailXMVC.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ISalaryRepository _salaryRepository;

        public SalaryController(ISalaryRepository salaryRepository)
        {
            _salaryRepository = salaryRepository;
        }

        // --- 1. Xem Danh sách Lương 
        public async Task<IActionResult> Index(int? month, int? year)
        {
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            var salaries = await _salaryRepository.GetSalaries(currentMonth, currentYear);

            ViewBag.Month = currentMonth;
            ViewBag.Year = currentYear;

            return View(salaries);
        }

        // --- 2. Xử lý Tính toán Bảng lương
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Process(int month, int year)
        {
            try
            {
                await _salaryRepository.ProcessMonthlySalaries(month, year);
                TempData["SuccessMessage"] = $"Đã xử lý và tạo/cập nhật bảng lương tháng {month}/{year} thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xử lý bảng lương: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { month, year });
        }

        // --- 3. Điều chỉnh Thưởng/Khấu trừ 
        public async Task<IActionResult> Adjust(int salaryId)
        {
            var salary = await _salaryRepository.GetSalaryById(salaryId);
            if (salary == null)
            {
                return NotFound();
            }
            return View(salary);
        }

        // POST: /Salary/Adjust
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(Salary model, decimal bonus, decimal deduction)
        {
            // Lấy thông tin tháng/năm từ model
            bool success = await _salaryRepository.UpdateBonusDeduction(
                model.StaffId,
                model.Month,
                model.Year,
                bonus, 
                deduction 
            );

            if (success)
            {
                // Cập nhật Amount sau khi điều chỉnh
                await _salaryRepository.ProcessMonthlySalaries(model.Month, model.Year);
                TempData["SuccessMessage"] = $"Đã điều chỉnh lương cho nhân viên {model.StaffId} thành công.";
                return RedirectToAction(nameof(Index), new { model.Month, model.Year });
            }
            TempData["ErrorMessage"] = "Lỗi khi điều chỉnh lương.";
            return View(model);
        }

        // --- 4. Xác nhận Thanh toán 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int salaryId, int month, int year)
        {
            int dayPayment = DateTime.Now.Day;

            bool success = await _salaryRepository.UpdatePaymentStatus(salaryId, 1, dayPayment);

            if (success)
            {
                TempData["SuccessMessage"] = $"Đã xác nhận thanh toán (ID: {salaryId}).";
            }
            else
            {
                TempData["ErrorMessage"] = $"Lỗi: Không thể xác nhận thanh toán cho ID {salaryId}.";
            }


            return RedirectToAction(nameof(Index), new { month, year });
        }
    }
}
