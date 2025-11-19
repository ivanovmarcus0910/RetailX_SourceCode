using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace DataAccessObject
{
    public class InventoryDashboardDAO
    {
        private readonly Tenant0Context _context;

        public InventoryDashboardDAO(Tenant0Context context)
        {
            _context = context;
        }

        // 1. Tổng số sản phẩm
        public int GetTotalProducts()
            => _context.Products.Count();

        // 2. Tổng danh mục
        public int GetTotalCategories()
            => _context.Categories.Count();

        // 3. Tổng nhà cung cấp
        public int GetTotalSuppliers()
            => _context.Suppliers.Count();

        // 4. Tổng khách hàng
        public int GetTotalCustomers()
            => _context.Customers.Count();

        // 5. Giá trị hàng tồn kho
        public decimal GetInventoryValue()
            => _context.Products
                .Sum(p => p.Quantity * (p.Price ?? 0));

        // 6. Low Stock Alert
        public List<Product> GetLowStock()
            => _context.Products
.Where(p => p.Quantity <= 5).ToList();

        // 7. Monthly Import (from PurchaseOrderDetail)
        public List<object> GetMonthlyImport()
        {
            return _context.PurchaseOrders
                .Join(_context.PurchaseOrderDetails,
                      o => o.PurchaseOrderId,
                      d => d.PurchaseOrderId,
                      (o, d) => new { o.CreateDate, d.Quantity })
                .GroupBy(x => x.CreateDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalImport = g.Sum(x => x.Quantity ?? 0)
                })
                .Cast<object>()
                .ToList();
        }
    }
    }
