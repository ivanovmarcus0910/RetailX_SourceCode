using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class PurchaseOrderDetailDAO
    {
        private readonly Tenant0Context _context;

        public PurchaseOrderDetailDAO(Tenant0Context context)
        {
            _context = context;
        }

        public List<PurchaseOrderDetail> GetByOrder(int orderId)
        {
            return _context.PurchaseOrderDetails
                .Include(d => d.Product)
                .Where(d => d.PurchaseOrderId == orderId)
                .ToList();
        }

        public void Add(PurchaseOrderDetail detail)
        {
            _context.PurchaseOrderDetails.Add(detail);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var d = _context.PurchaseOrderDetails.Find(id);
            if (d != null)
            {
                _context.PurchaseOrderDetails.Remove(d);
                _context.SaveChanges();
            }
        }

        public void Update(PurchaseOrderDetail detail)
        {
            var existing = _context.PurchaseOrderDetails.Find(detail.PurchaseOrderDetailId);
            if (existing != null)
            {
                existing.ProductId = detail.ProductId;
                existing.Quantity = detail.Quantity;
                existing.Price = detail.Price;
                _context.SaveChanges();
            }
        }
        public PurchaseOrderDetail GetById(int detailId)
        {
            return _context.PurchaseOrderDetails
                           .Include(d => d.Product)
                           .FirstOrDefault(d => d.PurchaseOrderDetailId == detailId);
        }
    }
}
