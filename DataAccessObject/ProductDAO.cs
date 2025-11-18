using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class ProductDAO
    {
        private readonly Tenant0Context _context;

        public ProductDAO(Tenant0Context context)
        {
            _context = context;
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products
                // SỬA: Sử dụng true/false trực tiếp
                .Where(p => p.IsActive == true)
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                // SỬA: Sử dụng true/false trực tiếp
                .FirstOrDefault(p => p.ProductId == id && p.IsActive == true);
        }

        public void AddProduct(Product product)
        {
            // Đảm bảo IsActive được đặt là true khi thêm mới
            product.IsActive = true;
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.IsActive = false;
                _context.Entry(product).State = EntityState.Modified;

                _context.SaveChanges();
            }
        }
    }
}
