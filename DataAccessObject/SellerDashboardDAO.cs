using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
namespace DataAccessObject
{
    public class SellerDashboardDAO
    {
        private readonly Tenant0Context _context;

        public SellerDashboardDAO(Tenant0Context context)
        {
            _context = context;
        }

        // --- HELPER: Lấy Query theo thời gian ---
        private IQueryable<Order> GetOrdersByPeriod(int staffId, string period)
        {
            var today = DateTime.Now.Date;
            var query = _context.Orders.Where(o => o.StaffId == staffId);

            switch (period)
            {
                case "day":
                    return query.Where(o => o.CreateDate.Date == today);
                case "week":
                    var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    var startOfWeek = today.AddDays(-1 * diff);
                    return query.Where(o => o.CreateDate.Date >= startOfWeek);
                case "month":
                    var startOfMonth = new DateTime(today.Year, today.Month, 1);
                    return query.Where(o => o.CreateDate.Date >= startOfMonth);
                default:
                    return query;
            }
        }

        // 1. Đếm số đơn hàng (Trả về int)
        public int GetOrderCount(int staffId, string period)
        {
            return GetOrdersByPeriod(staffId, period).Count();
        }

        // 2. Tính tổng doanh số (Trả về decimal)
        public decimal GetRevenue(int staffId, string period)
        {
            var query = GetOrdersByPeriod(staffId, period);

            // Tính tổng: Số lượng * Giá bán
            return query.SelectMany(o => o.OrderDetails)
                        .Sum(od => od.Quantity * (od.Product.Price ?? 0));
        }

        // 3. Lấy danh sách đơn hàng gần đây (Trả về List<Order> chuẩn Model)
        public List<Order> GetRecentOrders(int staffId)
        {
            return _context.Orders
                .Where(o => o.StaffId == staffId)
                .Include(o => o.Customer) // Include để hiện tên khách
                .OrderByDescending(o => o.CreateDate)
                .Take(5)
                .ToList();
        }

        // 4. Lấy dữ liệu biểu đồ (Trả về List object/dynamic giống InventoryDAO)
        public List<dynamic> GetDailyChartData(int staffId, int month, int year)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var rawData = _context.Orders
                .Where(o => o.StaffId == staffId
                            && o.CreateDate >= startOfMonth
                            && o.CreateDate < endOfMonth)
                .SelectMany(o => o.OrderDetails, (order, detail) => new
                {
                    Day = order.CreateDate.Day,
                    Total = detail.Quantity * (detail.Product.Price ?? 0)
                })
                .ToList();

            return rawData
                .GroupBy(x => x.Day)
                .Select(g =>
                {
                    dynamic obj = new System.Dynamic.ExpandoObject();
                    obj.Day = g.Key.ToString(); // Trả về số ngày (string) cho gọn
                    obj.Revenue = g.Sum(x => x.Total);
                    return obj;
                })
                .OrderBy(x => int.Parse(x.Day))
                .Cast<dynamic>()
                .ToList();
        }

        // Trong DataAccessObject/SellerDashboardDAO.cs

        // ... (Các hàm cũ giữ nguyên) ...

        // 4. [MỚI] Lấy số lượng khách hàng mua theo ngày (Cho biểu đồ Cột)
        public List<dynamic> GetCustomerGrowthChart(int staffId, int month, int year)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var data = _context.Orders
                .Where(o => o.StaffId == staffId
                            && o.CreateDate >= startOfMonth
                            && o.CreateDate < endOfMonth)
                .Select(o => new { Day = o.CreateDate.Day, CustomerId = o.CustomerId })
                .ToList(); // Lấy về RAM xử lý GroupBy Distinct cho dễ

            return data
                .GroupBy(x => x.Day)
                .Select(g =>
                {
                    dynamic obj = new System.Dynamic.ExpandoObject();
                    obj.Day = "Ngày " + g.Key;
                    // Đếm số khách duy nhất trong ngày (1 ông mua 2 lần chỉ tính 1)
                    obj.CustomerCount = g.Select(x => x.CustomerId).Distinct().Count();
                    return obj;
                })
                .OrderBy(x => int.Parse(x.Day.Replace("Ngày ", "")))
                .Cast<dynamic>()
                .ToList();
        }

        // 5. [MỚI] Top sản phẩm bán chạy theo số lượng (Cho biểu đồ Tròn)
        public List<dynamic> GetTopProductsChart(int staffId, int month, int year)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);

            var data = _context.Orders
                .Where(o => o.StaffId == staffId
                            && o.CreateDate >= startOfMonth
                            && o.CreateDate < endOfMonth)
                .SelectMany(o => o.OrderDetails) // Bung chi tiết đơn hàng
                .GroupBy(d => d.Product.ProductName) // Nhóm theo tên SP
                .Select(g => new
                {
                    Name = g.Key,
                    TotalQty = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalQty) // Lấy bán nhiều nhất
                .Take(5) // Top 5
                .ToList();

            return data.Select(x =>
            {
                dynamic obj = new System.Dynamic.ExpandoObject();
                obj.ProductName = x.Name;
                obj.Quantity = x.TotalQty;
                return obj;
            }).Cast<dynamic>().ToList();
        }
    }
}
