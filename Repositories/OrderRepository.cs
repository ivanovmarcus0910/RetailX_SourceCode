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

        // Lấy tất cả đơn
        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToList();
        }

        // Lấy 1 đơn theo id
        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);
        }

        // Insert đơn đơn giản (không truyền sẵn list details)
        public void Insert(Order order)
        {
            _orderDao.AddOrder(order);
            _context.SaveChanges();
        }

        // Xoá đơn
        public void Delete(int id)
        {
            var o = _orderDao.GetOrderById(id);
            if (o != null)
            {
                _context.Orders.Remove(o);
                _context.SaveChanges();
            }
        }

        // Tạo đơn + list OrderDetail (dùng khi bạn muốn truyền details riêng)
        public Order CreateOrder(Order order, List<OrderDetail> details)
        {
            // thêm Order trước
            _orderDao.AddOrder(order);
            _context.SaveChanges();   // để có OrderId

            // thêm OrderDetail
            foreach (var d in details)
            {
                d.OrderId = order.OrderId;
                _detailDao.AddOrderDetail(d);
            }

            _context.SaveChanges();
            return order;
        }


        // Lấy các đơn theo StaffId (nếu sau này Seller xem đơn của chính mình)
        public Order? GetOrderById(int id) => _orderDao.GetOrderById(id);

        public List<Order> GetOrdersByStaff(int staffId)
            => _orderDao.GetOrdersByStaff(staffId);

        // public void DeleteOrderDetails(int orderId) => _orderDao.DeleteOrderDetails(orderId);

        public void Update(Order order, List<OrderDetail> newDetails)
        {
            var dbOrder = _context.Orders
                .Include(o => o.OrderDetails)
                .First(o => o.OrderId == order.OrderId);

            // Update order info
            dbOrder.CustomerId = order.CustomerId;
            dbOrder.StaffId = order.StaffId;
            dbOrder.Status = order.Status;

            // Xóa toàn bộ chi tiết cũ
            _context.OrderDetails.RemoveRange(dbOrder.OrderDetails);

            // Thêm chi tiết mới
            foreach (var d in newDetails)
            {
                d.OrderId = dbOrder.OrderId;
                _context.OrderDetails.Add(d);
            }

            _context.SaveChanges();
        }





    }
}
