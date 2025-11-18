using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISalaryRepository
    {
        // Xử lý tạo/cập nhật bảng lương hàng loạt
        Task<bool> ProcessMonthlySalaries(int month, int year);

        // Cập nhật thưởng/khấu trừ cho một nhân viên
        Task<bool> UpdateBonusDeduction(int staffId, int month, int year, decimal bonus, decimal deduction);

        // Cập nhật trạng thái thanh toán
        Task<bool> UpdatePaymentStatus(int salaryId, byte status, int dayPayment);

        // Lấy danh sách lương
        Task<List<Salary>> GetSalaries(int month, int year);
        Task<Salary> GetSalaryById(int salaryId);
    }
}