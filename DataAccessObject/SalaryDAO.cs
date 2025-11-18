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

        public List<Salary> GetSalariesByMonthAndYear(int month, int year)
        {
           
            return _context.Salaries
                           .Where(s => s.Month == month && s.Year == year)
                           .Include(s => s.Staff)
                           .ToList(); 
        }

        public Salary GetSalaryById(int salaryId)
        {
            return _context.Salaries
                           .Include(s => s.Staff)
                           .FirstOrDefault(s => s.SalaryId == salaryId);
        }

        public void InsertSalary(Salary salary)
        {
            _context.Salaries.Add(salary);
            _context.SaveChanges();
        }

        public bool UpdateSalary(Salary salary)
        {
            var existingSalary = _context.Salaries.FirstOrDefault(s => s.SalaryId == salary.SalaryId);

            if (existingSalary == null) return false;

            // Cập nhật các trường
            existingSalary.Bonus = salary.Bonus;
            existingSalary.Deduction = salary.Deduction;
            existingSalary.Amount = salary.Amount;
            existingSalary.Status = salary.Status;
            existingSalary.DayPayment = salary.DayPayment;

            int changes = _context.SaveChanges();
            return changes > 0;
        }
    }
}