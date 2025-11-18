using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefault(o => o.OrderId == id);
        }

        public List<Order> GetOrdersByStaff(int staffId)
        {
            return _context.Orders
                .Where(o => o.StaffId == staffId)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreateDate)
                .ToList();
        }
    }
}
