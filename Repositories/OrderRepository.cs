using BusinessObject.Models;
using DataAccessObject;

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
        public List<Order> GetAll() => _orderDao.GetAllOrders();

        // Lấy 1 đơn theo id
        public Order? GetById(int id) => _orderDao.GetOrderById(id);

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
        public List<Order> GetOrdersByStaff(int staffId)
            => _orderDao.GetOrdersByStaff(staffId);
    }
}
