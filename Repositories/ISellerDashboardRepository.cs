using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISellerDashboardRepository
    {
        int GetOrderCount(int staffId, string period);
        decimal GetRevenue(int staffId, string period);
        List<Order> GetRecentOrders(int staffId); // Sử dụng Model Order gốc
        List<dynamic> GetDailyChartData(int staffId, int month, int year);
        // Thêm 2 dòng này
        List<dynamic> GetCustomerGrowthChart(int staffId, int month, int year);
        List<dynamic> GetTopProductsChart(int staffId, int month, int year);
    }
}
