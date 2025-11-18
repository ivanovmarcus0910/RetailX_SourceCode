using BusinessObject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISalaryRepository
    {
        bool ProcessMonthlySalaries(int month, int year);

        bool UpdateBonusDeduction(int staffId, int month, int year, decimal bonus, decimal deduction);

        bool UpdatePaymentStatus(int salaryId, byte status, int dayPayment);

        List<Salary> GetSalaries(int month, int year);

        Salary GetSalaryById(int salaryId);
    }
}