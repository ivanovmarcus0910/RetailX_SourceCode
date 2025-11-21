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
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
        }
        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
        }
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
        public (List<Order>, int) GetOrders(string keyword, DateTime? fromDate, DateTime? toDate, int pageIndex, int pageSize)
        {

            var query = _context.Orders
                .Include(o => o.Customer) 
                .Include(o => o.Staff)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string k = keyword.ToLower().Trim();

                query = query.Where(o =>
                    o.OrderId.ToString().Contains(k) || 
                    (o.Customer != null && (
                        o.Customer.CustomerName.ToLower().Contains(k) ||
                        o.Customer.Phone.Contains(k)                     
                    ))
                );
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreateDate >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                var nextDay = toDate.Value.Date.AddDays(1);
                query = query.Where(o => o.CreateDate < nextDay);
            }

            int totalCount = query.Count();
            var list = query.OrderByDescending(o => o.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return (list, totalCount);
        }

    }
}
