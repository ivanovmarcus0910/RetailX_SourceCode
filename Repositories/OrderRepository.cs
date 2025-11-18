using BusinessObject.Models;
using DataAccessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Tenant0Context _context;
        private readonly OrderDAO _orderDao;
        private readonly OrderDetailDAO _detailDao;

        public OrderRepository(Tenant0Context context)
        {
            _context = context;
            _orderDao = new OrderDAO(context);
            _detailDao = new OrderDetailDAO(context);
        }

        public Order CreateOrder(Order order, List<OrderDetail> details)
        {
            _orderDao.AddOrder(order);
            _context.SaveChanges(); // Sinh OrderId

            foreach (var detail in details)
            {
                detail.OrderId = order.OrderId;
                _detailDao.AddOrderDetail(detail);
            }

            _context.SaveChanges();

            return order;
        }

        public Order? GetOrderById(int id) => _orderDao.GetOrderById(id);

        public List<Order> GetOrdersByStaff(int staffId)
            => _orderDao.GetOrdersByStaff(staffId);
    }
}
