using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class OrderDetailDAO
    {
        private readonly Tenant0Context _context;

        public OrderDetailDAO(Tenant0Context context)
        {
            _context = context;
        }

        public OrderDetail? GetById(int id)
            => _context.OrderDetails.FirstOrDefault(x => x.OrderDetailId == id);

        public List<OrderDetail> GetByOrder(int orderId)
            => _context.OrderDetails.Where(x => x.OrderId == orderId).ToList();

        public void Add(OrderDetail detail)
            => _context.OrderDetails.Add(detail);

        public void Update(OrderDetail detail)
        {
            var old = _context.OrderDetails
                .FirstOrDefault(x => x.OrderDetailId == detail.OrderDetailId);

            if (old == null) return;

            old.ProductId = detail.ProductId;
            old.Quantity = detail.Quantity;
        }


        public void Delete(OrderDetail detail)
            => _context.OrderDetails.Remove(detail);
    }

}
