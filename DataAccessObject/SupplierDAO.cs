using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace DataAccessObject
{
    public class SupplierDAO
    {
        private readonly Tenant0Context _context;

        public SupplierDAO(Tenant0Context context)
        {
            _context = context;
        }

        // Lấy tất cả Supplier
        public IEnumerable<Supplier> GetAll()
        {
            return _context.Suppliers.ToList();
        }

        // Lấy Supplier theo ID
        public Supplier GetById(int id)
        {
            return _context.Suppliers.FirstOrDefault(s => s.SupplierId == id);
        }

        // Thêm mới Supplier
        public void Add(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }

        // Cập nhật Supplier
        public void Update(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
        }

        // Xoá Supplier
        public void Delete(int id)
        {
            var supplier = GetById(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                _context.SaveChanges();
            }
        }
    }
}
