using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class PurchaseOrderDAO
    {
        private readonly Tenant0Context _context;

        public PurchaseOrderDAO(Tenant0Context context)
        {
            _context = context;
        }

        public List<PurchaseOrder> GetAll()
        {
            return _context.PurchaseOrders
                .Where(p => p.IsActive == true)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderDetails)
                .ToList();
        }

        public PurchaseOrder GetById(int id)
        {
            return _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderDetails)
                .FirstOrDefault(p => p.PurchaseOrderId == id);
        }

        public void Add(PurchaseOrder order)
        {
            _context.PurchaseOrders.Add(order);
            _context.SaveChanges();
        }

        public void Update(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var obj = _context.PurchaseOrders.Find(id);
            if (obj != null)
            {
                obj.IsActive = false;
                _context.Entry(obj).State = EntityState.Modified;

                _context.SaveChanges();
            }
        }

    }
}
