using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class InventoryDashboardRepository : IInventoryDashBoardRepository
    {
        private readonly Tenant0Context _context;

        public InventoryDashboardRepository(Tenant0Context context)
        {
            _context = context;
        }

        public int GetTotalProducts()
        {
            return _context.Products.Where(p => p.IsActive == true).Count();
        }

        public int GetTotalCategories()
        {
            return _context.Categories.Where(p => p.IsActive == true).Count();
        }

        public int GetTotalSuppliers()
        {
            return _context.Suppliers.Where(p => p.IsActive == true).Count();
        }

        public int GetTotalCustomers()
        {
            return _context.Customers.Count();
        }

        public decimal GetInventoryValue()
        {
            return _context.Products
                .Sum(p => p.Quantity  * (p.Price ?? 0));
        }

        public List<Product> GetLowStock()
        {
            return _context.Products
                .Include(p => p.Category)
        .Include(p => p.Supplier)
                .Where(p => p.Quantity  <= 5)
                .ToList();
        }

        // ⭐⭐ MONTHLY IMPORT — KHÔNG tạo model mới, dùng dynamic ⭐⭐
        public List<dynamic> GetMonthlyImport()
        {
            var detailsWithMonth = _context.PurchaseOrderDetails
                 .Include(d => d.PurchaseOrder)   
                .Select(d => new
                {
                    Month = d.PurchaseOrder.CreateDate.Month,
                    Total = d.Quantity * d.Price
                })
                .ToList();

            // Nhóm theo tháng và tính tổng
            var grouped = detailsWithMonth
                .GroupBy(x => x.Month)
                .Select(g =>
                {
                    dynamic obj = new ExpandoObject();
                    obj.Month = g.Key;
                    obj.TotalImport = g.Sum(x => x.Total);
                    return obj;
                })
                .OrderBy(x => x.Month)
                .ToList();

            return grouped;
        }
        public List<dynamic> GetImportReportDetail(int? year = null, int? month = null, int? supplierId = null, int? categoryId = null)
        {
            var query = _context.PurchaseOrderDetails
                .Include(d => d.PurchaseOrder)  
                .Include(d => d.Product).AsQueryable();

            if (year.HasValue)
                query = query.Where(d => d.PurchaseOrder.CreateDate.Year == year.Value);

            if (month.HasValue)
                query = query.Where(d => d.PurchaseOrder.CreateDate.Month == month.Value);

            if (supplierId.HasValue)
                query = query.Where(d => d.Product.SupplierId == supplierId.Value);

            if (categoryId.HasValue)
                query = query.Where(d => d.Product.CategoryId == categoryId.Value);

            var list = query
                .Select(d => new
                {
                    ProductName = d.Product.ProductName,
                    Month = d.PurchaseOrder.CreateDate.Month,
                    Year = d.PurchaseOrder.CreateDate.Year,
                    Quantity = d.Quantity,
                    CreateDate = d.PurchaseOrder.CreateDate, // thêm dòng này
                    Price = d.Price,
                    Total = d.Quantity * d.Price
                })
                .ToList();

            var result = list
                .Select(d =>
                {
                    dynamic obj = new ExpandoObject();
                    obj.CreateDate = d.CreateDate; // thêm ngày tạo
                    obj.Year = d.Year;
                    obj.TimeGroup = "Tháng " + d.Month;
                    obj.ProductName = d.ProductName;
                    obj.Quantity = d.Quantity;
                    obj.Price = d.Price;
                    obj.Total = d.Total;
                    return obj;
                })
                .OrderBy(d => d.Year)
                .ThenBy(d => d.TimeGroup)
                .ThenBy(d => d.ProductName)
                .ToList();

            return result;
        }
        public List<dynamic> GetCategoryStockDistribution()
        {
            var data = _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalQty = g.Sum(x => x.Quantity)
                })
                .ToList();

            var list = new List<dynamic>();

            foreach (var item in data)
            {
                dynamic obj = new System.Dynamic.ExpandoObject();
                obj.CategoryName = item.CategoryName;
                obj.TotalQty = item.TotalQty;
                list.Add(obj);
            }

            return list;
        }



    }
}
