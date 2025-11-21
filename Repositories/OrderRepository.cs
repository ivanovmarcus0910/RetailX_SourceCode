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

        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToList();
        }

        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);
        }


        public void Insert(Order order)
        {
            _orderDao.AddOrder(order);
            _context.SaveChanges();
        }

        public void Delete(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null) return;

            foreach (var d in order.OrderDetails)
            {
                d.Product.Quantity += d.Quantity;
            }

            order.Status = 0;

            _context.SaveChanges();
        }


        public Order CreateOrder(Order order, List<OrderDetail> details)
        {

            _orderDao.AddOrder(order);
            _context.SaveChanges();  

            _context.SaveChanges();
            return order;
        }

        public Order? GetOrderById(int id) => _orderDao.GetOrderById(id);

        public List<Order> GetOrdersByStaff(int staffId)
            => _orderDao.GetOrdersByStaff(staffId);

        public (List<Order>, int) GetOrders(string keyword, DateTime? fromDate, DateTime? toDate, int page, int size)
        {
            return _orderDao.GetOrders(keyword, fromDate, toDate, page, size);
        }




    }
}
