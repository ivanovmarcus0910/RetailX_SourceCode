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

        public void AddOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
        }
        public void UpdateOrderDetail(OrderDetail detail)
        {
            _context.OrderDetails.Update(detail);
        }
        public void DeleteDetailsByOrder(int orderId)
        {
            var details = _context.OrderDetails
                .Where(d => d.OrderId == orderId)
                .ToList();

            _context.OrderDetails.RemoveRange(details);
        }
    }
}
