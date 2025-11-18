using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject
{
    public class OrderDAO
    {
        private readonly Tenant0Context _context;

        public OrderDAO(Tenant0Context context)
        {
            _context = context;
        }

        // ========== CREATE ==========
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        // ========== GET ALL ==========
        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }

        // ========== GET BY ID ==========
        public Order? GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.OrderId == id);
        }

        // ========== GET BY STAFF ==========
        public List<Order> GetOrdersByStaff(int staffId)
        {
            return _context.Orders
                .Where(o => o.StaffId == staffId)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }

        // ========== DELETE ==========
        public void DeleteOrder(int id)
        {
            var order = _context.Orders
                                .Include(o => o.OrderDetails)
                                .FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
                _context.Orders.Remove(order);
            }
        }
    }
}
