using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool ProcessMonthlySalaries(int month, int year)
        {
            var allStaffs = _staffDao.GetAll();

            var staffsToProcess = allStaffs.Where(s => s.BaseSalary.HasValue).ToList();

            bool success = true;

            foreach (var staff in staffsToProcess)
            {
                // 1. Lấy bản ghi lương cũ (nếu có)
                var existingSalaries = _salaryDao.GetSalariesByMonthAndYear(month, year);
                var salaryRecord = existingSalaries.FirstOrDefault(s => s.StaffId == staff.StaffId);

                // 2. Tính toán tổng lương
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
                    _salaryDao.InsertSalary(newSalary);
                }
                else
                {
                    salaryRecord.Amount = totalAmount;
                    success &= _salaryDao.UpdateSalary(salaryRecord);
                }
            }

            return success;
        }

        public bool UpdateBonusDeduction(int staffId, int month, int year, decimal bonus, decimal deduction)
        {
            var existingSalaries = _salaryDao.GetSalariesByMonthAndYear(month, year);
            var salaryRecord = existingSalaries.FirstOrDefault(s => s.StaffId == staffId);

            if (salaryRecord != null)
            {
                salaryRecord.Bonus = bonus;
                salaryRecord.Deduction = deduction;

                // Lấy thông tin Staff để tính lại lương cơ bản
                var staff = _staffDao.GetById(staffId);
                decimal baseSalary = staff?.BaseSalary ?? 0;
                salaryRecord.Amount = baseSalary + bonus - deduction;

                return _salaryDao.UpdateSalary(salaryRecord);
            }
            return false;
        }

        public bool UpdatePaymentStatus(int salaryId, byte status, int dayPayment)
        {
            var salaryRecord = _salaryDao.GetSalaryById(salaryId);
            if (salaryRecord != null && status == 1)
            {
                salaryRecord.Status = 1;
                salaryRecord.DayPayment = dayPayment;
                return _salaryDao.UpdateSalary(salaryRecord);
            }
            return false;
        }

        public List<Salary> GetSalaries(int month, int year)
        {
            return _salaryDao.GetSalariesByMonthAndYear(month, year);
        }

        public Salary GetSalaryById(int salaryId)
        {
            return _salaryDao.GetSalaryById(salaryId);
        }
    }
}