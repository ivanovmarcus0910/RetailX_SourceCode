using BusinessObject.Models;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDetailDAO _dao;
        private readonly Tenant0Context _context;

        public OrderDetailRepository(Tenant0Context context)
        {
            _context = context;
            _dao = new OrderDetailDAO(context);
        }

        public OrderDetail GetById(int id)
        {
            return _context.OrderDetails
                .Include(d => d.Order)
                    .ThenInclude(o => o.Customer)
                .Include(d => d.Product)
                .FirstOrDefault(d => d.OrderDetailId == id);
        }

        public List<OrderDetail> GetByOrder(int orderId) => _dao.GetByOrder(orderId);

        public void Add(OrderDetail detail)
        {
            _dao.Add(detail);
            _context.SaveChanges();
        }

        public void Update(OrderDetail detail)
        {
            var existing = _context.OrderDetails
                .FirstOrDefault(x => x.OrderDetailId == detail.OrderDetailId);

            if (existing == null)
                return;

      
            existing.ProductId = detail.ProductId;
            existing.Quantity = detail.Quantity;

            _context.SaveChanges();
        }



        public void Delete(int detailId)
        {
            var d = _dao.GetById(detailId);
            if (d != null)
            {
                _dao.Delete(d);
                _context.SaveChanges();
            }
        }
    }

}
