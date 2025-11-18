using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly StaffDAO _staffDao;
        private readonly SalaryDAO _salaryDao;

        public SalaryRepository(StaffDAO staffDao, SalaryDAO salaryDao)
        {
            _staffDao = staffDao;
            _salaryDao = salaryDao;
        }

        public async Task<bool> ProcessMonthlySalaries(int month, int year)
        {
            var staffs = await _staffDao.GetAllActiveStaffs();
            bool success = true;

            foreach (var staff in staffs)
            {
                var existingSalaries = await _salaryDao.GetSalariesByMonthAndYear(month, year);
                var salaryRecord = existingSalaries.FirstOrDefault(s => s.StaffId == staff.StaffId);

                decimal bonus = salaryRecord?.Bonus ?? 0;
                decimal deduction = salaryRecord?.Deduction ?? 0;

                decimal baseSalary = staff.BaseSalary ?? 0;
                decimal totalAmount = baseSalary + bonus - deduction; 

                if (salaryRecord == null)
                {
                    var newSalary = new Salary
                    {
                        StaffId = staff.StaffId,
                        Bonus = bonus,
                        Deduction = deduction,
                        Amount = totalAmount,
                        Month = month,
                        Year = year,
                        Status = 0
                    };
                    await _salaryDao.InsertSalary(newSalary);
                }
                else
                {
                    salaryRecord.Amount = totalAmount;
                    success &= await _salaryDao.UpdateSalary(salaryRecord);
                }
            }

            return success;
        }

        public async Task<bool> UpdateBonusDeduction(int staffId, int month, int year, decimal bonus, decimal deduction)
        {
            var existingSalaries = await _salaryDao.GetSalariesByMonthAndYear(month, year);
            var salaryRecord = existingSalaries.FirstOrDefault(s => s.StaffId == staffId);

            if (salaryRecord != null)
            {
                salaryRecord.Bonus = bonus;
                salaryRecord.Deduction = deduction;

                // Tính toán lại Amount sau khi điều chỉnh
                var staff = await _staffDao.GetStaffById(staffId);
                decimal baseSalary = staff?.BaseSalary ?? 0;
                salaryRecord.Amount = baseSalary + bonus - deduction;

                return await _salaryDao.UpdateSalary(salaryRecord);
            }
            return false;
        }

        public async Task<bool> UpdatePaymentStatus(int salaryId, byte status, int dayPayment)
        {
            var salaryRecord = await _salaryDao.GetSalaryById(salaryId);
            if (salaryRecord != null && status == 1)
            {   
                salaryRecord.Status = 1;
                salaryRecord.DayPayment = dayPayment;
                return await _salaryDao.UpdateSalary(salaryRecord);
            }
            return false;
        }

        public async Task<List<Salary>> GetSalaries(int month, int year)
        {
            return await _salaryDao.GetSalariesByMonthAndYear(month, year);
        }

        public async Task<Salary> GetSalaryById(int salaryId)
        {
            return await _salaryDao.GetSalaryById(salaryId);
        }
    }
}