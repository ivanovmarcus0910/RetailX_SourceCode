using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportRevenueDAO _reportDao;
        private readonly Tenant0Context _context;
        public ReportRepository(Tenant0Context context, ReportRevenueDAO reportDao)
        {
            _context = context;
            _reportDao = reportDao;
        }

        public bool GenerateRevenueReport(DateTime startDate, DateTime endDate, int staffIdCreator)
        {
            // 1. TÍNH DOANH THU
            decimal? totalRevenue = _context.Orders
                .Where(o => o.CreateDate >= startDate && o.CreateDate <= endDate && o.Status == 1)
                .SelectMany(o => o.OrderDetails)
                .Sum(od => (decimal?)(od.Quantity * od.Product.Price));

            // 2. TÍNH CHI PHÍ NHẬP HÀNG
            decimal? totalCost = _context.PurchaseOrders
                .Where(po => po.CreateDate >= startDate && po.CreateDate <= endDate && po.Status == 1)
                .SelectMany(po => po.PurchaseOrderDetails)
                .Sum(pod => (decimal?)(pod.Quantity * pod.Price));

            // 3. TÍNH CHI PHÍ LƯƠNG
            decimal? totalSalary = 0;

            for (var date = startDate; date <= endDate; date = date.AddMonths(1))
            {
                int month = date.Month;
                int year = date.Year;

               
                decimal? monthlySalary = _context.Salaries
                    .Where(s => s.Month == month && s.Year == year && s.Status == 1)
                    .Sum(s => s.Amount);

                totalSalary += monthlySalary.GetValueOrDefault();
            }

            // 4. TÍNH LỢI NHUẬN
            decimal profit = totalRevenue.GetValueOrDefault() - totalCost.GetValueOrDefault() - totalSalary.GetValueOrDefault();

            // 5. LƯU BÁO CÁO
            var report = new ReportRevenue
            {
                DayStart = startDate.ToShortDateString(),
                DayEnd = endDate.ToShortDateString(),
                Month = startDate.Month,
                Year = startDate.Year,
                CreateDate = DateTime.Now,
                StaffId = staffIdCreator,
                AmountRevenue = totalRevenue,
                AmountCost = totalCost,
                AmountSalary = totalSalary,
                Profit = profit
            };

            _reportDao.InsertReport(report);
            return true;
        }

        public List<ReportRevenue> GetRevenueReports(DateTime startDate, DateTime endDate)
        {
            return _reportDao.GetReportsByPeriod(startDate, endDate);
        }

        public List<Order> GetOrdersForExport(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Where(o => o.CreateDate >= startDate && o.CreateDate <= endDate && o.Status == 1)
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Product)
                .ToList();
        }
    }
}
