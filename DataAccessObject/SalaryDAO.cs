using System.Collections.Generic;
using System.Linq;
using BusinessObject;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class SalaryDAO
    {
        private readonly Tenant0Context _context;

        public SalaryDAO(Tenant0Context context)
        {
            _context = context;
        }

        public async Task<List<Salary>> GetSalariesByMonthAndYear(int month, int year)
        {
            // Truy vấn lương theo tháng/năm, bao gồm thông tin Staff
            return await _context.Salaries
                                 .Where(s => s.Month == month && s.Year == year)
                                 .Include(s => s.StaffId)
                                 .ToListAsync();
        }

        public async Task<Salary> GetSalaryById(int salaryId)
        {
            return await _context.Salaries
                         .Include(s => s.SalaryNavigation) 
                         .FirstOrDefaultAsync(s => s.SalaryId == salaryId);
        }

        public async Task InsertSalary(Salary salary)
        {
            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateSalary(Salary salary)
        {
            var existingSalary = await _context.Salaries.FirstOrDefaultAsync(s => s.SalaryId == salary.SalaryId);

            if (existingSalary == null) return false;

            // Cập nhật các trường
            existingSalary.Bonus = salary.Bonus;
            existingSalary.Deduction = salary.Deduction;
            existingSalary.Amount = salary.Amount;
            existingSalary.Status = salary.Status;
            existingSalary.DayPayment = salary.DayPayment;

            int changes = await _context.SaveChangesAsync();
            return changes > 0;
        }
    }
}