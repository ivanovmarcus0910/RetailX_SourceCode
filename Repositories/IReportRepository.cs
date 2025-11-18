using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace Repositories
{
    public interface IReportRepository
    {
        bool GenerateRevenueReport(DateTime startDate, DateTime endDate, int staffIdCreator);
        List<ReportRevenue> GetRevenueReports(DateTime startDate, DateTime endDate);
        List<Order> GetOrdersForExport(DateTime startDate, DateTime endDate);
    }
}
