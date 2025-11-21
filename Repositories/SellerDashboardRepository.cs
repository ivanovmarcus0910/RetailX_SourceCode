using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SellerDashboardRepository : ISellerDashboardRepository
    {
        private readonly SellerDashboardDAO _dao;

        public SellerDashboardRepository(SellerDashboardDAO dao)
        {
            _dao = dao;
        }

        public int GetOrderCount(int staffId, string period) => _dao.GetOrderCount(staffId, period);
        public decimal GetRevenue(int staffId, string period) => _dao.GetRevenue(staffId, period);
        public List<Order> GetRecentOrders(int staffId) => _dao.GetRecentOrders(staffId);
        public List<dynamic> GetDailyChartData(int staffId, int month, int year) => _dao.GetDailyChartData(staffId, month, year);
        // Implement 2 hàm
        public List<dynamic> GetCustomerGrowthChart(int staffId, int month, int year)
        {
            return _dao.GetCustomerGrowthChart(staffId, month, year);
        }

        public List<dynamic> GetTopProductsChart(int staffId, int month, int year)
        {
            return _dao.GetTopProductsChart(staffId, month, year);
        }
    }
}
