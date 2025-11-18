using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class ReportRevenueDAO
    {
        private readonly Tenant0Context _context;

        public ReportRevenueDAO(Tenant0Context context)
        {
            _context = context;
        }
        public void InsertReport(ReportRevenue report)
        {
            _context.ReportRevenues.Add(report);
            _context.SaveChanges();
        }

        public List<ReportRevenue> GetReportsByPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.ReportRevenues
                           .Where(r => r.CreateDate >= startDate && r.CreateDate <= endDate)
                           .Include(r => r.Staff)
                           .ToList();
        }
    }
}
